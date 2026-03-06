param location string
param appServicePlanName string
param webAppName string
param environment string
param appServicePlanSkuName string = 'S1'
param appServicePlanSkuTier string = 'Standard'

var appServicePlanFullName = '${appServicePlanName}-${environment}'
var webAppFullName = '${webAppName}-${environment}'
var commonTags = {
  environment: environment
  createdBy: 'github-actions'
  project: 'zento-api'
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanFullName
  location: location
  tags: commonTags
  kind: 'linux'
  sku: {
    name: appServicePlanSkuName
    tier: appServicePlanSkuTier
    capacity: 1
  }
  properties: {
    reserved: true
  }
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
}

// App Settings
resource webAppSettings 'Microsoft.Web/sites/config@2023-01-01' = {
  name: 'appsettings'
  parent: webApp
  properties: {
    ASPNETCORE_ENVIRONMENT: environment
  }
}

output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output webAppName string = webApp.name
output appServicePlanId string = appServicePlan.id
output webAppResourceId string = webApp.id
