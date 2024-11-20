@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param keyVaultName string

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource adventurebuilder 'Microsoft.DocumentDB/databaseAccounts@2024-08-15' = {
  name: take('adventurebuilder-${uniqueString(resourceGroup().id)}', 44)
  location: location
  properties: {
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
  }
  kind: 'GlobalDocumentDB'
  tags: {
    'aspire-resource-name': 'adventurebuilder'
  }
}

resource adventurebuilder 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-08-15' = {
  name: 'adventurebuilder'
  location: location
  properties: {
    resource: {
      id: 'adventurebuilder'
    }
  }
  parent: adventurebuilder
}

resource connectionString 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'connectionString'
  properties: {
    value: 'AccountEndpoint=${adventurebuilder.properties.documentEndpoint};AccountKey=${adventurebuilder.listKeys().primaryMasterKey}'
  }
  parent: keyVault
}