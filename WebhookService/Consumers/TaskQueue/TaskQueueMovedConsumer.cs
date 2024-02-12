using Microsoft.Extensions.Options;
using Shared.Enums.RabbitMQ;
using Shared.Helpers;
using Shared.Models.RabbitMq;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using SqlSugar;

namespace WebhookService.Consumers.TaskQueue;

[Consumer("task-queue.moved", "task-queue.moved.handler")]
public class TaskQueueMovedConsumer(
    IOptionsMonitor<RabbitMqOptions> options,
    IRabbitMqService rabbitMqService,
    ILogger<TaskQueueMovedConsumer> logger,
    IMessageSerializer messageSerializer,
    ISqlSugarClient db)
    : TopicConsumer(options, rabbitMqService, logger, messageSerializer, db)
{
    private readonly DatabaseHelper _databaseHelper = new(db, logger);

    protected override async Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken)
    {
        if (!message.Items.Any()) return MessageReceiveResult.Failed;

        var itemsDictionary = message.Items.ToDictionary(k => k.Key, v => v.Value);
        var twilioEventTaskQueue = ObjectHelper.CopyPropertiesFromDictionary<TwilioEventTaskQueue>(itemsDictionary);
        twilioEventTaskQueue.TaskObj = ObjectHelper.CopyPropertiesFromDictionary<TwilioTaskField>(itemsDictionary);

        return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventTaskQueue, cancellationToken);
    }
}