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
    log.Info("C# HTTP trigger function processed a request.");

    // always insert new value; duplicates may result
    if (req.Method == "POST")
    {
        string name = req.Query["name"];
        if (string.IsNullOrEmpty(name))
        {
            return new BadRequestObjectResult("Artist name not specified on the query string.");
        }

        var entity = new ArtistEntity()
        {
            RowKey = Guid.NewGuid().ToString(),
            PartitionKey = "Partition0",
            ArtistName = name
        };
    
        var operation = TableOperation.Insert(entity);
        var task = notMyArtistsTable.ExecuteAsync(operation);
        Console.WriteLine(task);
        Console.WriteLine(task.Result);

        log.Info(string.Format("New artist inserted with key {0}.", entity.RowKey));

        return new OkObjectResult(entity);
    }

    return new BadRequestObjectResult("Code path not yet implemented.");
}
