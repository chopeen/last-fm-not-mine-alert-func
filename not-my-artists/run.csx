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

    string artistName = req.Query["name"];
    string resultFormat = req.Query["format"];

    // TODO: Check result and log success or failure for all paths
    //       example: log.Info(string.Format("New artist inserted with key {0}.", entity.RowKey));
    if (req.Method == "POST")
    {
        return InsertOne(notMyArtistsTable, artistName);
    }
    else if (req.Method == "GET" && string.IsNullOrEmpty(resultFormat))
    {
        return GetAll(notMyArtistsTable);
    }
    else if (req.Method == "GET" && resultFormat == "csv")
    {
        return GetAllAsDelimitedString(notMyArtistsTable);
    }

    return new BadRequestObjectResult("Code path not yet implemented.");
}

// Remark: This function always insert a new value; it may generate duplicates
private static IActionResult InsertOne(CloudTable notMyArtistsTable, string artistName)
{
        if (string.IsNullOrEmpty(artistName))
        {
            return new BadRequestObjectResult("Artist name not specified on the query string.");
        }

        var entity = new ArtistEntity()
        {
            RowKey = Guid.NewGuid().ToString(),
            PartitionKey = "Partition0",
            ArtistName = artistName,
            ArtistNameNormalized = ArtistEntity.Normalize(artistName)
        };

        var operation = TableOperation.Insert(entity);
        var task = notMyArtistsTable.ExecuteAsync(operation);

        // TODO: Would it be possible to use EnsureSuccessStatusCode here, when the execution is synchrounous?
        int statusCode = task.Result.HttpStatusCode;
        bool success = statusCode >= 200 && statusCode < 300;
        if (success)
        {
            return new OkObjectResult(entity);
        }

        return new BadRequestObjectResult(string.Format("INSERT failed {0}.", artistName));
}

private static IActionResult GetAll(CloudTable notMyArtistsTable)
{
    var allArtists = FetchTableData(notMyArtistsTable);
    return new OkObjectResult(allArtists);
}

private static IActionResult GetAllAsDelimitedString(CloudTable notMyArtistsTable)
{
    List<ArtistEntity> allArtists = FetchTableData(notMyArtistsTable);

    List<string> distinctNormalizedNames = allArtists.
        GroupBy(ae => ae.ArtistNameNormalized).
        Select(grp => grp.First().ArtistNameNormalized).
        OrderBy(str => str).
        ToList();

    string delimitedNormalizedNames = string.Join(";", distinctNormalizedNames);

    return new OkObjectResult(delimitedNormalizedNames);
}

// TODO: Segment contains up to 1,000 entities - implement query continuation
private static List<ArtistEntity> FetchTableData(CloudTable notMyArtistsTable)
{
    var selectAllQuery = new TableQuery<ArtistEntity>();
    var segmentResult = notMyArtistsTable.ExecuteQuerySegmentedAsync(selectAllQuery, null).Result;
    List<ArtistEntity> segmentList = segmentResult.ToList<ArtistEntity>();
    return segmentList;
}