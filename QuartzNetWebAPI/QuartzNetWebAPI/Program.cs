using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using QuartzNetWebAPI.Controllers;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using QuartzNetWebAPI;



var builder = WebApplication.CreateBuilder(args);
var properties = new NameValueCollection
            {
                {"quartz.serializer.type", "json"},
                {"quartz.scheduler.instanceName", "TestScheduler"},
                {"quartz.scheduler.instanceId", "ABQuartzAdmin"},
                {"quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz"},
                {"quartz.threadPool.threadCount", "10"}
            };

builder.Services.AddControllers();

ISchedulerFactory sf = new StdSchedulerFactory(properties);
var scheduler = sf.GetScheduler().GetAwaiter().GetResult();
builder.Services.AddSingleton(scheduler);
//scheduler.Clear();

// Adding the Api
builder.Services.AddQuartzAdmin(scheduler);

builder.Services.AddQuartz(q =>
{
    // base Quartz configuration
    //q.UseMicrosoftDependencyInjectionJobFactory();
    q.UseSimpleTypeLoader();
    q.UseInMemoryStore();
    q.UseDefaultThreadPool(tp =>
    {
        tp.MaxConcurrency = 10;
    });
}
    );
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
