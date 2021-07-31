table! {
    node_connection_information (id) {
        id -> Uuid,
        node_id -> Uuid,
        address -> Text,
        priority -> Int4,
    }
}

table! {
    nodes (id) {
        id -> Uuid,
        account_id -> Text,
        device_id -> Uuid,
        lookup_token -> Text,
        authentication_token -> Text,
        control_token_challenge -> Uuid,
        control_token -> Nullable<Text>,
        acme_challenge -> Nullable<Text>,
        first_seen -> Timestamp,
        last_seen -> Timestamp,
    }
}

// table! {
//     settings (id) {
//         id -> Text,
//         value -> Jsonb,
//     }
// }

joinable!(node_connection_information -> nodes (node_id));

allow_tables_to_appear_in_same_query!(node_connection_information, nodes,);

// settings,
