using Microsoft.OpenApi.Models;
using Server.Silo.Api;
using Orleans.Providers;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .UseOrleans((ctx, builder) =>
    {
        builder.UseLocalhostClustering();
        builder.AddMemoryGrainStorageAsDefault();
        builder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>("MemoryStreams");
        builder.AddMemoryGrainStorage("PubSubStore");
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
            .ConfigureServices(services =>
            {
                services.AddControllers()
                    .AddApplicationPart(typeof(TodoController).Assembly);

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
                                    "https://polite-desert-0be8aa100.2.azurestaticapps.net")
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