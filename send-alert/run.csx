#r "Newtonsoft.Json"
#r "SendGrid"


using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;


// TODO: `runOnStartup` can be configured similar to `schedule` if it turns out it must be
//       disabled in PROD to prevent the timer from firing twice
//
// TODO: The alert email will more useful when it lists the played tracks (not only artists)
//
// TODO: Don't alert about the same scrobbles twice
//       1. `mbid + uts` seems to be a good return a unique scrobble ID
//       2. Table storage will be useful.
//
// TODO: Is any old Last.fm API with `removeScrobble` still available?
//       https://hackage.haskell.org/package/liblastfm-0.0.2.2/docs/Network-Lastfm-API-Library.html
//
// TODO: The function execution takes ~2000ms - why so long? Log the time of communication
//       with the Last.fm API.

public static void Run(TimerInfo timer, TraceWriter log, out SendGridMessage message)
{
    // when executed locally, logged to the console
    log.Info("Request processing started.");

    string recentTracksString = "";
    using (HttpClient client = new HttpClient())
    {
        // no work to be done during GetStringAsync execution, so waiting synchronously for an async method
        //   (the `await` operator can only be used within an async method; more: https://goo.gl/MPPkUv)
        recentTracksString = client.GetStringAsync(getRecentTracksUri()).Result;
    }

    var recentArtists = getRecentArtists(recentTracksString);
    var notMyArtists = getNotMyArtists();
    var notMyArtistsPlayedRecently = recentArtists.Intersect(notMyArtists).ToList();

    // debugging helper
    // notMyArtistsPlayedRecently.ToList().ForEach(x => Console.WriteLine(x));

    log.Info(
        string.Format("Recent artists:       {0} [{1}]", string.Join("; ", recentArtists), recentArtists.Count())
    );
    log.Info(
        string.Format("Blacklisted artists:  {0} [{1}]", string.Join("; ", notMyArtists), notMyArtists.Count())
    );

    if (notMyArtistsPlayedRecently.Count() > 0)
    {
        message = getAlertMessage(notMyArtistsPlayedRecently);
        log.Info("Request processing finished - alert sent.");
    }
    else
    {
        message = null;
        log.Info("Request processing finished - no alert needed.");
    }
}

private static SendGridMessage getAlertMessage(List<string> notMyArtistsPlayedRecently)
{
    // docs: https://github.com/sendgrid/sendgrid-csharp/blob/master/src/SendGrid/Helpers/Mail/MailHelper.cs#L137
    EmailAddress from = MailHelper.StringToEmailAddress(getLocalSetting("EmailFromAlert"));
    EmailAddress to = MailHelper.StringToEmailAddress(getLocalSetting("EmailToAlert"));
    
    string subject = "Check the Last.fm history";
    
    string htmlListItems = string.Join("</li><li>", notMyArtistsPlayedRecently);
    string htmlContent = $"The following artists were played recently: <ul><li>{htmlListItems}</li></ul>.";
    
    // TODO: Plain content should be created automatically by stripping tags from HTML context
    string plainTextContent = htmlContent;
    
    // docs: https://github.com/sendgrid/sendgrid-csharp/blob/master/src/SendGrid/Helpers/Mail/MailHelper.cs#L31
    var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
    message.TemplateId = getLocalSetting("SendGridTemplateId");

    return message;
}

private static List<string> getRecentArtists(string recentTracksString)
{
    JObject recentTracks = JObject.Parse(recentTracksString);
    // TODO: Can this be improved with a strongly typed model for the Last.fm JSON?
    //       Give "Paste JSON as Code" one more try (https://github.com/quicktype/quicktype-vscode/issues/8).
    var tracks = recentTracks["recenttracks"]["track"].ToList();

    return tracks
        .Select(
            x => x["artist"]["#text"].ToString().ToLower().Trim()
        )
        .Distinct()
        .ToList();
}

private static List<string> getNotMyArtists()
{
    // TODO: Replace with /api/not-my-artists?format=csv
    //       Should the API be returning a delimited string or rather a list to get rid of this `Split(';').ToList()`?

    using (HttpClient client = new HttpClient())
    {
        client.BaseAddress = new Uri(getFunctionBaseUrl());

        string notMyArtistsUri = "api/not-my-artists?format=csv";
        string notMyArtistsDelimited = client.GetStringAsync(notMyArtistsUri).Result;
    }

    return getLocalSetting("NotMyArtists")
        .Split(';')
        .ToList()
        .Select(
            x => x.ToLower().Trim()
            )
        .ToList();
}

private static string getFunctionBaseUrl()
{
    string functionHost = getLocalSetting("WEBSITE_HOSTNAME");
    string allZeros = "0.0.0.0";

    // HACK: Workaround for error during local execution:
    //         System.Net.NameResolution: IPv4 address 0.0.0.0 and IPv6 address ::0 are unspecified addresses that cannot be used as a target address.
    bool localExecution = functionHost.StartsWith(allZeros);
    if (localExecution)
    {
        functionHost = functionHost.Replace(allZeros, "localhost");
        return $"http://{functionHost}";
    }
    
    return $"https://{functionHost}";
}

private static string getRecentTracksUri()
{
    // TODO: Is is enough to fetch only 1 page of results?
    return string.Format(
        "http://ws.audioscrobbler.com/2.0/?method={0}&user={1}&api_key={2}&format=json",
        "user.getrecenttracks",
        getLocalSetting("LastFmUser"),
        getLocalSetting("LastFmKey")
    );
}

private static string getLocalSetting(string name)
{
    string value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    if (string.IsNullOrEmpty(value))
    {
        // docs: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated
        throw new NullReferenceException($"Environment variable {name} has no value or is not defined.");
    }
    return value;
}
