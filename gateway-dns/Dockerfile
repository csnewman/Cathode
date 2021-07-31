FROM ubuntu
WORKDIR /app
EXPOSE 53
COPY target/release/gateway-dns /app/
COPY config.toml /app/
ENTRYPOINT ["./gateway-dns"]
