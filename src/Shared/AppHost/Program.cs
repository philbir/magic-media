var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MagicMedia_Worker>("worker");

builder.Build().Run();
