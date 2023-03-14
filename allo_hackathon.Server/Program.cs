using Microsoft.OpenApi.Models;
using Server.Silo.Api;
using Orleans.Providers;
using Microsoft.Extensions.Hosting;
using Orleans.Persistence.AzureStorage;
using System.Configuration;

//private IConfiguration _configuration;

await Host.CreateDefaultBuilder(args)
    .UseOrleans((ctx, builder) =>
    {
        builder.UseLocalhostClustering();
        builder.AddMemoryGrainStorageAsDefault();
        builder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>("MemoryStreams");
        builder.AddMemoryGrainStorage("PubSubStore");
        builder.AddAzureTableGrainStorage(
        name: "strage",
        configureOptions: options =>
        {
            //options.UseJson = true;
            //var sss = ctx.Configuration["CONNECTION_STRING"];
            options.ConfigureTableServiceClient(
            System.Configuration.ConfigurationManager.AppSettings["CONNECTION_STRING"] ?? ctx.Configuration["CONNECTION_STRING"]);
        });
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
            .ConfigureServices(services =>
            {
                services.AddControllers()
                    .AddApplicationPart(typeof(UserController).Assembly);

                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = nameof(Server), Version = "v1" });
                });

                services.AddCors(options =>
                {
                    options.AddPolicy("ApiService",
                        builder =>
                        {
                            builder
                                .WithOrigins(
                                    "http://localhost:62653",
                                    "http://localhost:62654",
                                    "https://polite-desert-0be8aa100.2.azurestaticapps.net",
                                    "https://yellow-forest-0e4103300.2.azurestaticapps.net"
                                    )
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                });
            })
            .Configure(app =>
            {
                app.UseCors("ApiService");
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", nameof(Server));
                });

                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            })
            .UseUrls("http://localhost:5000");

    })
    .ConfigureServices(services =>
    {
        services.Configure<ConsoleLifetimeOptions>(options =>
        {
            options.SuppressStatusMessages = true;
        });
    })
    .RunConsoleAsync();