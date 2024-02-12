using Microsoft.Extensions.Options;
using Shared.Enums.RabbitMQ;
using Shared.Helpers;
using Shared.Models.RabbitMq;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using SqlSugar;

namespace WebhookService.Consumers.Task;

[Consumer("task.completed", "task.completed.handler")]
public class TaskCompletedConsumer(
    IOptionsMonitor<RabbitMqOptions> options,
    IRabbitMqService rabbitMqService,
    ILogger<TaskCompletedConsumer> logger,
    IMessageSerializer messageSerializer,
    ISqlSugarClient db)
    : TopicConsumer(options, rabbitMqService, logger, messageSerializer, db)
{
    private readonly DatabaseHelper _databaseHelper = new(db, logger);

    protected override async Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken)
    {
        if (!message.Items.Any()) return MessageReceiveResult.Failed;
        
        var itemsDictionary = message.Items.ToDictionary(k => k.Key, v => v.Value);
        var twilioEventTask = ObjectHelper.CopyPropertiesFromDictionary<TwilioEventTask>(itemsDictionary);
        twilioEventTask.TaskObj = ObjectHelper.CopyPropertiesFromDictionary<TwilioTaskField>(itemsDictionary);
        
        return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventTask, cancellationToken);
    }
}