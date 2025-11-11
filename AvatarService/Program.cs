using AvatarService.Services;
using MediatR;
using Microsoft.Extensions.FileProviders;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddControllers();

builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddSingleton<IAvatarStore, AvatarStore>();
builder.Services.AddSingleton<AvatarGenerationService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "public/avatars")),
    RequestPath = "/avatars"
});

app.MapControllers();

app.Run();
