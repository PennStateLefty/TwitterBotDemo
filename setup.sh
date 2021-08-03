#!/bin/bash
#Azure CLI script to setup the TwitterBot Azure environment
resourceGroupName="twitterbot$RANDOM-rg"
azureregion="centralus"
functionruntime="dotnet-isolated"
functionappname="twitterbotdemo$RANDOM"
storagename="twitterbotstorage$RANDOM"
cosmosdbname = "twitterbotcosmos$RANDOM"

#Create a simple control structure
if [ -z $1 ]; then
    echo "An option (Create or Destroy) is required to run this script"
else
    echo "Option set to $1"
    if [ $1 == "Create" ] || [ $1 == "create" ]; then
        #Begin setup
        echo "Create a resource group"
        az group create --name $resourceGroupName --location $azureregion
        echo "Create a storage account"
        az storage account create --name $storagename --location $azureregion --resource-group $resourceGroupName --sku Standard_LRS
        echo "Create Function App"
        az functionapp create --resource-group $resourceGroupName --consumption-plan-location $azureregion --runtime $functionruntime --runtime-version 5.0 --functions-version 3 --name $functionappname --storage-account $storagename
        echo "Create CosmosDB"
        az cosmosdb create --name $cosmosdbname --resource-group $resourceGroupName --locations regionName=$azureregion isZoneRedundant=false
    elif [ $1 == "Destroy" ] || [ $1 == "destroy" ]; then
        az group delete --name $resourceGroupName 
    fi
fi