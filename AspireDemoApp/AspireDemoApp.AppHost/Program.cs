using k8s.Models;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireDemoApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireDemoApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

// This registers your .NET MAUI app and adds a reference to the Web API you want to call
builder.AddProject<Projects.AspireDemoApp_Maui>("mauiapp")
    .WithReference(apiService);


builder.Build().Run();
