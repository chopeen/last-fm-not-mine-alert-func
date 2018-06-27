## Creating a new function

[Code and test Azure Functions locally](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)

    func init <function-project-name>
    func new

    # execute locally
    func host start

    # deploy to Azure
    func azure functionapp publish <function-app-name>

    # execute in Azure CLI to switch to the version 2.x of the runtime
    az functionapp config appsettings set --name <function-app-name> \
      --resource-group <resource-group-name> \
      --settings FUNCTIONS_EXTENSION_VERSION=beta

## Migrating the application settings

Copy values from `local.settings.json` to "Application settings" in Azure (as part of application deployment):

    func azure functionapp publish <function-app-name> --publish-local-settings

Copy values from Azure to `local.settings.json`:

    func azure functionapp fetch-app-settings <function-app-name>

## Working with JSON data

 - https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm
 - https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm

## SendGrid email

 - https://sendgrid.com/docs/Integrate/Code_Examples/v3_Mail/csharp.html
 - https://docs.microsoft.com/en-us/azure/sendgrid-dotnet-how-to-send-email
 - Steps to install:
   1. Add a SendGrid output binding to `function.json` (read
      [Sending Emails with SendGrid and Azure Functions](http://markheath.net/post/sending-emails-sendgrid-azure-functions)
      for a good explanation of the SendGrid binding and 
      [Azure Functions triggers and bindings concepts](https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings)
      for general information about triggers and bindings)
   2. `func extensions install` to install all the required bindings (Linux may require manual installation - see issue #2 for details)
   3. Create a SendGrid resource through the Azure portal (pick a reasonable 
      name - it cannot be renamed later)
   4. Click `Manage` to navigate to https://app.sendgrid.com/ and get an API key from there
   5. Add the key to `local.settings.json`

## Timer trigger

[CRON time zones](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer#cron-time-zones)

> The default time zone used with the CRON expressions is Coordinated Universal Time (UTC). 
> To have your CRON expression based on another time zone, create an app setting for your 
> function app named `WEBSITE_TIME_ZONE` (e.g. `Central European Standard Time`). 

[CRON expressions](https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/)

    {second} {minute} {hour} {day} {month} {day of the week}

    0 * * * * * 	every minute
    0 */2 * * * * 	every 2 minutes
    0 0 */2 * * *   every 2 hours
    0 0 14 * * * 	every day at 2 PM

## function.json

 - https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/azure-functions/functions-bindings-timer.md#configuration - `%TimerSchedule%` 
   used in `function.json` works fine, but is not defined in the [JSON schema](http://json.schemastore.org/function), so VS Code highlights it
   as an error
 - https://github.com/Azure/azure-functions-host/wiki/function.json
 - `name` vs `%name%` vs `{name}`:
   - `"apiKey": "SendGridKey"` - the [schema](http://json.schemastore.org/function) explictly defines this `sendGridBinding` property as
     _name of the app setting which contains your SendGrid api key_
   - `"schedule": "%TimerSchedule%"` - reading value of an arbitrary app setting
   - `"from": "{FromEmail}"` - I'm quoting the documentation below, but I have no idea how to specify the `FromEmail` value in the code
     - _Most expressions are identified by wrapping them in curly braces. For example, in a queue trigger function, {queueTrigger} resolves to the queue message text. If the path property for a blob output binding is container/{queueTrigger} and the function is triggered by a queue message HelloWorld, a blob named HelloWorld is created._
