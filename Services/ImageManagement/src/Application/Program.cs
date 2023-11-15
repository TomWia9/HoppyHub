using Application;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddApplicationServices(configuration);
    })
    .Build();

host.Run();