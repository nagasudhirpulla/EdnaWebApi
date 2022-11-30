### Command to publish the web-api as a self-contained web server
https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained

```
dotnet publish -r win-x64 .\src\EdnaWebApi\EdnaWebApi.csproj
```

### run dotnet server at custom port
* https://stackoverflow.com/questions/37365277/how-to-specify-the-port-an-asp-net-core-application-is-hosted-on
* Using command line arguments, by starting your .NET application with --urls=[url]
```
dotnet run --urls=http://localhost:62448/
```
* Using appsettings.json, by adding a Urls node
```json
{
  "Urls": "http://localhost:62448"
}
```

* Note: Make sure both the files EzDnaApi64.dll and EzDNAApiNet64.dll files are present in the build folders like the Debug folder

### Swagger integration
* Official docs - https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
* swagger ui available at ```/swagger``` path of the web application