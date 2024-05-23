#!/bin/bash

dotnet test src/CRA.DataMigrator.UnitTests/ -c Release --no-restore --no-build /p:CollectCoverage=true /p:CoverletOutput=../../coverage/ /p:MergeWith=../../coverage/coverage.json