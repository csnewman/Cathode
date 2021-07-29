FROM ubuntu
WORKDIR /app
EXPOSE 52
COPY ["target/release/gateway-dns", "/app/"]
ENTRYPOINT ["gateway-dns"]
