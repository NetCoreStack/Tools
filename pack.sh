#!/bin/bash

dotnet restore NetCoreStack.Tools.sln
dotnet build NetCoreStack.Tools.sln
cd src/NetCoreStack.Hisar.WebCli.Tools
dotnet pack -o ../../nupkg --version-suffix preview20 -c Release