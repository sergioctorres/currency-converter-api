# Currency Converter API

This project is a simple currency converter API built with **.NET 9**.  
It uses the [Frankfurter API](https://www.frankfurter.dev/) as the external provider and adds support for **Redis distributed cache**.  
The project includes Docker support for easy setup and scaling.

---

## Requirements

- Docker and Docker Compose  
- .NET 9 SDK (only if you want to run outside Docker)

---

## Environment

Environment variables are stored in:

```
env/.env.development
```

Example:

```
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__RedisServer=redis:6379
```

---

## Run the application

### Development (single instance)

```sh
docker-compose up -d
```

---

### Scaled (2 replicas with Nginx load balancer)

```sh
docker-compose -f docker-compose.yml -f docker-compose.scale.yml up -d
```

- API: `http://localhost:8080`

---

## API Documentation

When the API is running, you can access the Swagger UI:

- `http://localhost:8080/swagger`

Swagger provides interactive documentation for all endpoints, request/response models, and example data.

---

## Postman Collection

A Postman collection with available endpoints is included in:

```
postman/CurrencyConverterAPI.postman_collection.json
```

You can import it in Postman.

---

## Test Coverage

When running with Docker Compose, the `test-runner` container will execute all tests and generate a coverage report automatically.

The report is available locally in the folder:

```
CodeCoverage/index.html
```

Open this file in your browser to view the detailed coverage results.

---

## Notes

- Latest and historical currency data are cached in Redis.  
- Some currencies are blocked by business rules and are not returned.  
- Nginx is only required when running with multiple replicas.  

## Next Steps

- Logging and Monitoring
    - Integrate Serilog with JSON output for structured logging.
    - Add CorrelationId to logs for request tracing across distributed systems.
    - Consider integration with Seq or ELK stack for centralized log visualization.
- Security and Access Control
    - Add API throttling / rate limiting to protect against abuse.
    - Enable HTTPS only in production.
- Observability
    - Configure metrics collection (e.g., Prometheus + Grafana) for monitoring performance and availability.
- Configuration and Secrets Management
    - Use User Secrets for local development instead of .env.
    - Use Azure Key Vault or AWS Secrets Manager in production.
- DevOps
    - Automate build, test, and coverage reports with GitHub Actions or Azure DevOps pipelines.