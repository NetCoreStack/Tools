dotnet restore NetCoreStack.Tools.sln
dotnet build NetCoreStack.Tools.sln
cd src/NetCoreStack.Hisar.WebCli.Tools
dotnet pack -o ../../nupkg /p:Version=2.1.1-preview1 -c Release
