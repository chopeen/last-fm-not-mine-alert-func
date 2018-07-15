## Input table and output table

The same Azure table is declared as a binding twice, with different directions. This is my understanding so far:

 - input table `notMyArtistsData` allows to fetch data from the storage,
 - output table `notMyArtistsOperations` is an interface for DDL and DML operations.

The class `CloudStorage` (the type of the latter) includes a number of `ExecuteQuery` overloads, so probably
it could also  be used to fetch the data. However, for now I will stick to the implementation inspired by `azfunc-crud`
([function.json](https://github.com/thiagospassos/azfunc-crud/blob/cb86e2aa65fc7c18e3a21f9b8c5b5dfc953640ab/customer/function.json),
 [run.csx](https://github.com/thiagospassos/azfunc-crud/blob/cb86e2aa65fc7c18e3a21f9b8c5b5dfc953640ab/customer/run.csx)).

### Error *Can't bind Table to type 'System.Linq.IQueryable* (see 761e67e97c6f3caf1631a5d59bd443589ff6f193)

https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table#input---c-example-2

> IQueryable isn't supported in the Functions v2 runtime. An alternative is to use a CloudTable paramName method
> parameter to read the table by using the Azure Storage SDK.

## POST without sending data

    curl -X POST -d {} http://localhost:7071/api/not-my-artists?name=Test2

## `HttpTrigger` does not return response content properly

When running locally, message from `BadRequestObjectResult` is displayed in `curl`'s output. After deploying to production,
only a generic "Bad Request" is returned. This is a known problem - https://stackoverflow.com/q/51221375/95.