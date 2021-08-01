FROM ubuntu
WORKDIR /app
EXPOSE 53
RUN apt-get update
RUN apt-get install -y libpq-dev
COPY target/release/gateway-dns /app/
COPY config.toml /app/
ENTRYPOINT ["./gateway-dns"]
