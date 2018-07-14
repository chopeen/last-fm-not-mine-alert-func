## POST without sending data

    curl -X POST -d {} http://localhost:7071/api/not-my-artists?name=Test2

## `HttpTrigger` does not return response content properly

When running locally, message from `BadRequestObjectResult` is displayed in `curl`'s output. After deploying to production,
only a generic "Bad Request" is returned. This is a known problem - https://stackoverflow.com/q/51221375/95.