use ::config::{ConfigError, File, Environment, FileFormat};

#[derive(Debug, Deserialize)]
pub struct Database {
    pub url: String,
}

#[derive(Debug, Deserialize)]
pub struct Dns {
    pub base_hostname: String,
    pub threads: u32,
    pub port: u16,
}

#[derive(Debug, Deserialize)]
pub struct Config {
    pub database: Database,
    pub dns: Dns,
}

impl Config {
    pub fn new() -> Result<Self, ConfigError> {
        let mut s = ::config::Config::default();
        s.merge(File::new("config.toml", FileFormat::Toml))?;
        s.merge(Environment::with_prefix("cathode"))?;
        s.try_into()
    }
}