#![allow(unused)]
#![allow(clippy::all)]

use chrono::NaiveDateTime;
use uuid::Uuid;

use diesel::Queryable;

#[derive(Queryable, Debug)]
pub struct NodeConnectionInformation {
    pub id: Uuid,
    pub node_id: Uuid,
    pub address: String,
    pub priority: i32,
}

#[derive(Queryable, Debug)]
pub struct Node {
    pub id: Uuid,
    pub account_id: String,
    pub device_id: Uuid,
    pub lookup_token: String,
    pub authentication_token: String,
    pub control_token_challenge: Uuid,
    pub control_token: Option<String>,
    pub acme_challenge: Option<String>,
    pub first_seen: NaiveDateTime,
    pub last_seen: NaiveDateTime,
}

// #[derive(Queryable, Debug)]
// pub struct Setting {
//     pub id: String,
//     pub value: Jsonb,
// }

