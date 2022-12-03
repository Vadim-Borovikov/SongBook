﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SongBook.Web.Models;

namespace SongBook.Web;

internal static class Program
{
    public static void Main(string[] args)
    {
        Utils.LogManager.DeleteExceptionLog();
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            Config? config = Configure(builder);
            if (config is null)
            {
                throw new NullReferenceException("Can't load config.");
            }
            Utils.StartLogWith(config.TimeZoneIdLogs);

            IServiceCollection services = builder.Services;
            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSingleton<Manager>();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Utils.LogManager.LogException(ex);
        }
    }

    private static Config? Configure(WebApplicationBuilder builder)
    {
        ConfigurationManager configuration = builder.Configuration;
        Config? config = configuration.Get<Config>();
        if (config is null)
        {
            return null;
        }

        builder.Services.AddOptions<Config>().Bind(configuration).ValidateDataAnnotations();
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<Config>>().Value);

        return config;
    }
}