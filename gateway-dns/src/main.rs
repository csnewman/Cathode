#[macro_use]
extern crate diesel;
extern crate serde;
#[macro_use]
extern crate serde_derive;

mod authority;
mod models;
mod schema;
mod config;

use diesel::prelude::*;
use simplelog::{CombinedLogger, TermLogger, LevelFilter, TerminalMode, Config as LogConfig, ColorChoice};
use log::{info, error};
use tokio::runtime::{self};
use trust_dns_server::authority::Catalog;
use trust_dns_server::ServerFuture;
use std::net::{SocketAddr, IpAddr, Ipv4Addr};
use tokio::time::Duration;
use tokio::net::{UdpSocket, TcpListener};
use trust_dns_server::client::rr::{Name, LowerName};
use std::str::FromStr;
use crate::authority::CathodeAuthority;
use std::sync::{Arc, RwLock};
use crate::config::Config;
use diesel::r2d2::{ Pool, PooledConnection, ConnectionManager };

pub type PgPool = Pool<ConnectionManager<PgConnection>>;
pub type PgPooledConnection = PooledConnection<ConnectionManager<PgConnection>>;

fn main() {
    let config = Config::new().expect("Unable to parse settings");

    CombinedLogger::init(
        vec![
            TermLogger::new(LevelFilter::Info, LogConfig::default(), TerminalMode::Mixed, ColorChoice::Auto),
        ]
    ).unwrap();

    info!("Cathode - gateway-dns");

    let manager = ConnectionManager::<PgConnection>::new(&config.database.url);
    let pool = Pool::builder().build(manager).expect("Failed to create pool");

    // start up the server for listening
    let runtime = runtime::Builder::new_multi_thread()
        .enable_all()
        .worker_threads(config.dns.threads as usize)
        .thread_name("cathode-gateway-dns-runtime")
        .build()
        .expect("failed to initialize Tokio Runtime");

    let mut catalog: Catalog = Catalog::new();

    let servers_name = LowerName::from(Name::from_str(&config.dns.base_hostname).unwrap());
    catalog.upsert(servers_name.clone(), Box::new(Arc::new(RwLock::new(CathodeAuthority {
        origin: servers_name,
        pool,
    }))));

    let mut server = ServerFuture::new(catalog);

    let sockaddrs: Vec<SocketAddr> = vec![
        SocketAddr::new(IpAddr::V4(Ipv4Addr::UNSPECIFIED), config.dns.port),
    ];
    let tcp_request_timeout = Duration::from_secs(5);

    for udp_socket in &sockaddrs {
        info!("binding UDP to {:?}", udp_socket);
        let udp_socket = runtime
            .block_on(UdpSocket::bind(udp_socket))
            .unwrap_or_else(|_| panic!("could not bind to udp: {}", udp_socket));

        info!(
            "listening for UDP on {:?}",
            udp_socket
                .local_addr()
                .expect("could not lookup local address")
        );

        let _guard = runtime.enter();
        server.register_socket(udp_socket);
    }

    for tcp_listener in &sockaddrs {
        info!("binding TCP to {:?}", tcp_listener);
        let tcp_listener = runtime
            .block_on(TcpListener::bind(tcp_listener))
            .unwrap_or_else(|_| panic!("could not bind to tcp: {}", tcp_listener));

        info!(
            "listening for TCP on {:?}",
            tcp_listener
                .local_addr()
                .expect("could not lookup local address")
        );

        let _guard = runtime.enter();
        server.register_listener(tcp_listener, tcp_request_timeout);
    }

    info!("Started");

    match runtime.block_on(server.block_until_done()) {
        Ok(()) => {
            info!("Cathode gateway-dns stopping");
        }
        Err(e) => {
            let error_msg = format!(
                "Cathode gateway-dns has encountered an error: {}",
                e
            );

            error!("{}", error_msg);
            panic!("{}", error_msg);
        }
    };
}
