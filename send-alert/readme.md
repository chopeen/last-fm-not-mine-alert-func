### Essence of [Code and test Azure Functions locally](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local):

    func init my-function-proj
    func new
    func host start

    # execute in Azure CLI to switch to the version 2.x of the runtime
    az functionapp config appsettings set --name function-app-name \
      --resource-group resource-group-name \
      --settings FUNCTIONS_EXTENSION_VERSION=beta

    func azure functionapp publish my-function-app
    # add --publish-local-settings to migrate the settings from
    #   local.settings.json to Azure; they are added to "Application settings"

### Working with JSON data

 - https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm
 - https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm

### SendGrid email

 - https://sendgrid.com/docs/Integrate/Code_Examples/v3_Mail/csharp.html
 - https://docs.microsoft.com/en-us/azure/sendgrid-dotnet-how-to-send-email
 - Steps to install:
   1. ~~Add a SendGrid binding to `function.json`~~ -- **not needed**, because SendGrid is
      not used as a binding in this case
      - the function sends an email, but this email isn't its output; 
      the output is the HTTP response ([more precise explanation](https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings) +
      [even more](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-sendgrid))
      - this may change when I finally switch to the `Timer` trigger)
   1. ~~`func extensions install` to install all the required bindings~~ -- **how to install** the SendGrid
      package without the 1st step?
   1. Create a SendGrid resource through the Azure portal (pick a reasonable 
      name - it cannot be renamed later)
   1. Click `Manage` to navigate to https://app.sendgrid.com/ and get an API key from there
   1. Add the key to `local.settings.json`
