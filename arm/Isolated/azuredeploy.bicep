@description('The name of FunctionApp')
param siteName string

@description('Hosting plan name')
param hostingPlanName string

@description('Name of Storage Account')
param storageAccountName string

@description('Storage Account Type')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_ZRS'
  'Premium_LRS'
])
param storageAccountType string

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Name of XSLT File')
param xsltFileName string

@description('Root container for XSLT Transformations')
param xsltransformcontainer string

@description('Source file directory')
param sourceFolder string

@description('Transformed file directory')
param destinationFolder string

@description('App Insights name')
param applicationInsightsName string

resource site 'Microsoft.Web/sites@2022-03-01' = {
  name: siteName
  kind: 'functionapp'
  location: location
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED'
          value: '1'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'MapName'
          value: xsltFileName
        }
        {
          name: 'xsltransformcontainer'
          value: xsltransformcontainer
        }
        {
          name: 'sourceFolder'
          value: sourceFolder
        }
        {
          name: 'destinationFolder'
          value: destinationFolder
        }
      ]
    }
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: false
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: hostingPlanName
  location: location
  kind: 'Windows'
  properties: {
    reserved: true
  }
  sku: {
    tier: 'Dynamic'
    name: 'Y1'
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: storageAccountType
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

