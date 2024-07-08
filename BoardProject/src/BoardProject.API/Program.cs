using FluentValidation.AspNetCore;
using BoardProject.API.Extensions;
using BoardProject.Infrastructure.Extensions;
using BoardProject.Infrastructure.Middleware;
using BoardProject.Core.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using BoardProject.Domain.Extensions;
using BoardProject.Core.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGeneration();

builder.Services.AddDatabaseContext(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddTokenAuthentication(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ResponseTimeMiddlewareAsync>();

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RetryPolicyMiddleware>();

app.MapControllers();

app.Run();
