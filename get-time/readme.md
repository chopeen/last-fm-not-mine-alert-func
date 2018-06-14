## Essence of [Code and test Azure Functions locally](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local):

    func init my-function-proj
    func new
    func host start

    # execute in Azure CLI to switch to the version 2.x of the runtime
    az functionapp config appsettings set --name function-app-name --resource-group resource-group-name --settings FUNCTIONS_EXTENSION_VERSION=beta

    func azure functionapp publish my-function-app
    # add --publish-local-settings to migrate the settings from
    #   local.settings.json to Azure; they are added to "Application settings"
