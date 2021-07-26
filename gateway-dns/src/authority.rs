use log::{info};
use trust_dns_server::authority::{AuthLookup, LookupError, LookupRecords, AnyRecords, Authority, ZoneType, MessageRequest, UpdateResult};
use std::net::{Ipv4Addr};
use trust_dns_server::client::rr::{RecordType, Name, LowerName, RecordSet};
use trust_dns_server::proto::rr::dnssec::SupportedAlgorithms;
use futures::{future, Future};
use trust_dns_server::client::op::{ResponseCode, LowerQuery};
use std::pin::Pin;
use crate::PgPool;
use std::borrow::Borrow;
use lazy_static::lazy_static;
use regex::Regex;
use trust_dns_server::proto::rr::{RData, Record};
use std::sync::Arc;
use anyhow::Result;
use trust_dns_server::proto::rr::rdata::{SOA, TXT};
use uuid::Uuid;
use crate::models::Node;

lazy_static! {
    static ref V4RE: Regex = Regex::new(r"^v4-(\d{1,3})_(\d{1,3})_(\d{1,3})_(\d{1,3})$").unwrap();
}

pub struct CathodeAuthority {
    pub origin: LowerName,
    pub pool: PgPool,
}

impl CathodeAuthority {
    fn perform_lookup(&self, name: &Name, rtype: RecordType) -> Result<Option<Arc<RecordSet>>> {
        info!("Lookup {} {}", name, rtype);

        // Check request is for zone
        if !self.origin.zone_of(&LowerName::from(name)) {
            return Ok(None);
        }

        if rtype == RecordType::SOA {
            let mut set = RecordSet::new(name, rtype, 0);
            set.insert(Record::from_rdata(Name::from(&self.origin), 60, RData::SOA(SOA::new(
                Name::from(&self.origin),
                Name::from(&self.origin),
                1,
                86400,
                7200,
                3600000,
                3600,
            ))), 0);
            return Ok(Some(Arc::new(set)));
        }

        // Ensure request in x.y.{base hostname} format
        if name.num_labels() != self.origin.num_labels() + 2 {
            return Ok(None);
        }

        let mut name_iter = name.iter();
        let ip_label = String::from_utf8_lossy(name_iter.next().unwrap());
        let node_label = String::from_utf8_lossy(name_iter.next().unwrap());
        let node_id = Uuid::parse_str(&node_label)?;

        let connection = self.pool.get()?;

        let node: Node = {
            use crate::schema::nodes::dsl::*;
            use diesel::prelude::*;
            nodes.find(node_id).first(&connection)?
        };

        // Ensure node is configured
        if node.control_token.is_none() {
            return Ok(None);
        }

        if ip_label == "_acme-challenge" && rtype == RecordType::TXT {
            // Return acme challenge content
            if let Some(challenge) = &node.acme_challenge {
                let mut set = RecordSet::new(&name, rtype, 0);
                let mut content = Vec::new();
                content.push(challenge.clone());
                set.insert(Record::from_rdata(name.clone(), 60, RData::TXT(TXT::new(content))), 0);
                return Ok(Some(Arc::new(set)));
            }
        } else if rtype == RecordType::A {
            // Check if we have a Ipv4 request
            if let Some(caps) = V4RE.captures(&ip_label) {
                let a = caps.get(1).unwrap().as_str().parse::<u8>()?;
                let b = caps.get(2).unwrap().as_str().parse::<u8>()?;
                let c = caps.get(3).unwrap().as_str().parse::<u8>()?;
                let d = caps.get(4).unwrap().as_str().parse::<u8>()?;

                let mut set = RecordSet::new(name, rtype, 0);
                set.insert(Record::from_rdata(name.clone(), 3600, RData::A(Ipv4Addr::new(a, b, c, d))), 0);
                return Ok(Some(Arc::new(set)));
            }
        }

        Ok(None)
    }

    fn handle_request(&self, name: &LowerName, rtype: RecordType, is_secure: bool, supported_algorithms: SupportedAlgorithms) -> Result<AuthLookup, LookupError> {
        match rtype {
            RecordType::AXFR | RecordType::ANY => {
                let result = AnyRecords::new(
                    is_secure,
                    supported_algorithms,
                    Vec::new(),
                    rtype,
                    name.clone(),
                );
                Ok(LookupRecords::AnyRecords(result))
            }
            _ => self.perform_lookup(name.borrow(), rtype)
                .map_or(
                    Err(LookupError::from(ResponseCode::NXDomain)),
                    |rr_set| rr_set.map_or(
                        Err(LookupError::from(ResponseCode::NXDomain)),
                        |rr_set| Ok(LookupRecords::new(is_secure, supported_algorithms, rr_set)),
                    ),
                )
        }.map(|answers| AuthLookup::answers(answers, None))
    }
}

impl Authority for CathodeAuthority {
    type Lookup = AuthLookup;
    type LookupFuture = future::Ready<Result<Self::Lookup, LookupError>>;

    fn zone_type(&self) -> ZoneType {
        ZoneType::Master
    }

    fn is_axfr_allowed(&self) -> bool {
        false
    }

    fn update(&mut self, _update: &MessageRequest) -> UpdateResult<bool> {
        Err(ResponseCode::NotImp)
    }

    fn origin(&self) -> &LowerName {
        &self.origin
    }

    fn lookup(&self, name: &LowerName, rtype: RecordType, is_secure: bool, supported_algorithms: SupportedAlgorithms) -> Pin<Box<dyn Future<Output=Result<Self::Lookup, LookupError>> + Send>> {
        Box::pin(future::ready(self.handle_request(name, rtype, is_secure, supported_algorithms)))
    }

    fn search(&self, query: &LowerQuery, is_secure: bool, supported_algorithms: SupportedAlgorithms) -> Pin<Box<dyn Future<Output=Result<Self::Lookup, LookupError>> + Send>> {
        let lookup_name = query.name();
        let record_type: RecordType = query.query_type();

        match record_type {
            RecordType::SOA => Box::pin(self.lookup(self.origin(), record_type, is_secure, supported_algorithms)),
            RecordType::AXFR => Box::pin(future::err(LookupError::from(ResponseCode::Refused))),
            _ => Box::pin(self.lookup(lookup_name, record_type, is_secure, supported_algorithms)),
        }
    }

    fn get_nsec_records(&self, _name: &LowerName, _is_secure: bool, _supported_algorithms: SupportedAlgorithms) -> Pin<Box<dyn Future<Output=Result<Self::Lookup, LookupError>> + Send>> {
        Box::pin(future::ok(AuthLookup::default()))
    }
}
