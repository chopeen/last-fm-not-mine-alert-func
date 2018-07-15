#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

#load "ArtistEntity.csx"


using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

// TODO: Find a way to have fewer exit points in this function.

public static IActionResult Run(HttpRequest req, CloudTable notMyArtistsTable, TraceWriter log)
{
    log.Info($"{req.Method} request processing started.");

    // always insert new value; duplicates may result
    if (req.Method == "POST")
    {
        string artistName = req.Query["name"];
        if (string.IsNullOrEmpty(artistName))
        {
            return new BadRequestObjectResult("Artist name not specified on the query string.");
        }

        // TODO: Extract the INSERT operation into a dedicated function.
        var entity = new ArtistEntity()
        {
            RowKey = Guid.NewGuid().ToString(),
            PartitionKey = "Partition0",
            ArtistName = artistName,
            ArtistNameNormalized = ArtistEntity.Normalize(artistName)
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
    else if (req.Method == "GET")
    {
        return GetArtists(notMyArtistsTable);
    }

    return new BadRequestObjectResult("Code path not yet implemented.");
}

public static IActionResult GetArtists(CloudTable notMyArtistsTable)
{
    // segment contains up to 1,000 entities, so no need to worry about query continuation for now
    var segmentResult = notMyArtistsTable.ExecuteQuerySegmentedAsync(new TableQuery<ArtistEntity>(), null).Result;
    return new OkObjectResult(segmentResult.ToList());
}
