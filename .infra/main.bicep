targetScope = 'subscription'

param location string = 'canadacentral'
param appServicePlanName string = 'zento-api-plan'
param appServicePlanSkuName string = 'F1'
param appServicePlanSkuTier string = 'Free'
param webAppName string = 'zento-api'
param environment string = 'dev'
param resourceGroupName string = 'zento-dev'

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

// Deploy App Service resources into the Resource Group
module appService 'modules/appService.bicep' = {
  name: 'appServiceDeployment'
  scope: rg
  params: {
    location: location
    appServicePlanName: appServicePlanName
    appServicePlanSkuName: appServicePlanSkuName
    appServicePlanSkuTier: appServicePlanSkuTier
    webAppName: webAppName
    environment: environment
  }
}

output resourceGroupId string = rg.id
output webAppUrl string = appService.outputs.webAppUrl
output webAppName string = appService.outputs.webAppName
output appServicePlanId string = appService.outputs.appServicePlanId
output webAppResourceId string = appService.outputs.webAppResourceId


