using Microsoft.AspNetCore.Mvc.Routing;
using Shared.Data;
using Shared.Extensions;
using Shared.Middlewares;
using Shared.Models.Log;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using Shared.Services;
using Twilio.AspNet.Core;
using WebhookService.BackgroundServices;
using WebhookService.Producers;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment()) 
    builder.Services.AddHostedService<TunnelService>();

// 注册版本约束类型
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("apiVersion", typeof(ApiVersionRouteConstraint));
});

// 配置端口
builder.WebHost.ConfigureServerOptions(builder.Environment, builder.Configuration,5500, 5501);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddCustomCors(builder.Environment);

// 连接数据库
builder.Services.AddSqlSugarService(builder.Configuration);

builder.Services.AddSingleton<ExceptionService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RabbitMQ
builder.Services.AddRabbitMqServices(builder.Configuration);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddScoped<IMessageSerializer, MessageSerializer>();
builder.Services.AddScoped<ITopicProducer, TopicProducer>();
builder.Services.AddScoped<TwilioEventProducer>();

builder.Services.AddHostedService<ConsumerHostedService>();

builder.Services.AddScoped<SqlSugarHelper>();

// Twilio
builder.Services.AddTwilioClient();
builder.Services.AddTwilioRequestValidation();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction() || app.Environment.IsStaging())
{
    app.UseHttpsRedirection();
}

app.UseCors("EnableCORS");
app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

// 初始化数据库
app.EnsureDbInitialization<TwilioEventMessageLog>("DialerV2");
app.EnsureDbInitialization<TwilioEventActivity>("DialerV2");
app.EnsureDbInitialization<TwilioEventReservation>("DialerV2");
app.EnsureDbInitialization<TwilioEventTask>("DialerV2");
app.EnsureDbInitialization<TwilioEventTaskChannel>("DialerV2");
app.EnsureDbInitialization<TwilioEventTaskQueue>("DialerV2");
app.EnsureDbInitialization<TwilioEventWorker>("DialerV2");
app.EnsureDbInitialization<TwilioEventWorkflow>("DialerV2");
app.EnsureDbInitialization<TwilioEventWorkspace>("DialerV2");

app.EnsureDbInitialization<TwilioCallStatus>("DialerV2");
app.EnsureDbInitialization<TwilioRecordingStatus>("DialerV2");

app.Run();