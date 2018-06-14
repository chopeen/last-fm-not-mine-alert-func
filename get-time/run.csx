#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

public static IActionResult Run(HttpRequest req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    /*
        string name = req.Query["name"];

        string requestBody = new StreamReader(req.Body).ReadToEnd();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name = name ?? data?.name;

        return name != null
            ? (ActionResult)new OkObjectResult($"Hello, {name}")
            : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
    */
    /*
        HttpClient client = new HttpClient();

        client.BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    */


    string result = getLocalSetting("FooValue") + Environment.NewLine + DateTime.Now.ToString();
    return new OkObjectResult(result);
}

private static string getLocalSetting(string name)
{
    return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
