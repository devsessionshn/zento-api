using './main.bicep'

param location = 'eastus'
param appServicePlanName = 'zento-api-plan'
param appServicePlanSkuName = 'B1'
param appServicePlanSkuTier = 'Basic'
param webAppName = 'zento-api'
param environment = 'dev'
param resourceGroupName = 'zento-dev'
