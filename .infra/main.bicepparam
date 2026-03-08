using './main.bicep'

param location = 'canadacentral'
param appServicePlanName = 'zento-api-plan'
param appServicePlanSkuName = 'F1'
param appServicePlanSkuTier = 'Free'
param webAppName = 'zento-api'
param environment = 'dev'
param resourceGroupName = 'zento-dev'
