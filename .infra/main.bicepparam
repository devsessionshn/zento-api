using './main.bicep'

param location = 'eastus'
param appServicePlanName = 'zento-api-plan'
param appServicePlanSkuName = 'S1'
param appServicePlanSkuTier = 'Standard'
param webAppName = 'zento-api'
param environment = 'dev'
param resourceGroupName = 'zento-dev'
