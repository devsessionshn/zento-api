targetScope = 'subscription'

param location string = 'eastus'
param appServicePlanName string = 'zento-api-plan'
param webAppName string = 'zento-api'
param environment string = 'dev'
param resourceGroupName string = 'zento-dev'

var appServicePlanFullName = '${appServicePlanName}-${environment}'
var webAppFullName = '${webAppName}-${environment}'
var commonTags = {
  environment: environment
  createdBy: 'github-actions'
  project: 'zento-api'
}

// Create Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: commonTags
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanFullName
  location: location
  tags: commonTags
  kind: 'linux'
  sku: {
    name: 'F1'
    tier: 'Free'
    capacity: 1
  }
  properties: {
    reserved: true
  }
  parent: rg
}

// Web App
resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppFullName
  location: location
  tags: commonTags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET|10.0'
      alwaysOn: false
      http20Enabled: true
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
  parent: rg
}

// App Settings
resource webAppSettings 'Microsoft.Web/sites/config@2023-01-01' = {
  name: 'appsettings'
  parent: webApp
  properties: {
    ASPNETCORE_ENVIRONMENT: environment
  }
}

output resourceGroupId string = rg.id
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output webAppName string = webApp.name
output appServicePlanId string = appServicePlan.id
output webAppResourceId string = webApp.id


