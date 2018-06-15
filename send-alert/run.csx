#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static IActionResult Run(HttpRequest req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    string recentTracksString = "";
    using (HttpClient client = new HttpClient())
    {
        // using GetStringAsync in a way that makes a synchronous call; it simplifies the code,
        //   but let's see how soon it becomes a problem
        recentTracksString = client.GetStringAsync(getRecentTracksUri()).Result;
    }

    JObject recentTracks = JObject.Parse(recentTracksString);
    var attr = recentTracks["recenttracks"]["@attr"];
    
    return new OkObjectResult(attr.ToString());
}

private static string getRecentTracksUri()
{
    return String.Format(
        "http://ws.audioscrobbler.com/2.0/?method={0}&user={1}&api_key={2}&format=json",
        "user.getrecenttracks",
        getLocalSetting("LastFmUser"),
        getLocalSetting("LastFmKey")
    );
}

private static string getLocalSetting(string name)
{
    return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
