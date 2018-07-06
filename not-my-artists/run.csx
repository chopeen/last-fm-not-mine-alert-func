#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

#load "ArtistEntity.csx"


using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;


public static IActionResult Run(HttpRequest req, CloudTable notMyArtistsTable, TraceWriter log)
{
    log.Info("Request processing started.");

    // always insert new value; duplicates may result
    if (req.Method == "POST")
    {
        string artistName = req.Query["name"];
        if (string.IsNullOrEmpty(artistName))
        {
            // TODO: Find a way to have fewer exit points in this function.
            return new BadRequestObjectResult("Artist name not specified on the query string.");
        }

        // TODO: Extract the INSERT operation into a dedicated function.
        var entity = new ArtistEntity()
        {
            RowKey = Guid.NewGuid().ToString(),
            PartitionKey = "Partition0",
            ArtistName = artistName,
            // TODO: Is there a way to store the normalized value without setting both properties?
            ArtistNameNormalized = artistName
        };
    
        var operation = TableOperation.Insert(entity);
        var task = notMyArtistsTable.ExecuteAsync(operation);

        int statusCode = task.Result.HttpStatusCode;
        bool success = statusCode >= 200 && statusCode < 300;
        if (success)
        {
            log.Info(string.Format("New artist inserted with key {0}.", entity.RowKey));
            return new OkObjectResult(entity);
        }

        return new BadRequestObjectResult(string.Format("INSERT failed {0}.", artistName));
    }

    return new BadRequestObjectResult("Code path not yet implemented.");
}
