using Microsoft.Extensions.Options;
using Shared.Enums.RabbitMQ;
using Shared.Helpers;
using Shared.Models.RabbitMq;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using SqlSugar;

namespace WebhookService.Consumers.TaskChannel;

[Consumer("task-channel.deleted", "task-channel.deleted.handler")]
public class TaskChannelDeletedConsumer(
    IOptionsMonitor<RabbitMqOptions> options,
    IRabbitMqService rabbitMqService,
    ILogger<TaskChannelDeletedConsumer> logger,
    IMessageSerializer messageSerializer,
    ISqlSugarClient db)
    : TopicConsumer(options, rabbitMqService, logger, messageSerializer, db)
{
    private readonly DatabaseHelper _databaseHelper = new(db, logger);

    protected override async Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken)
    {
        if (!message.Items.Any()) return MessageReceiveResult.Failed;

        var itemsDictionary = message.Items.ToDictionary(k => k.Key, v => v.Value);
        var twilioEventTaskChannel = ObjectHelper.CopyPropertiesFromDictionary<TwilioEventTaskChannel>(itemsDictionary);
        twilioEventTaskChannel.TaskChannelObj = ObjectHelper.CopyPropertiesFromDictionary<TwilioTaskChannelField>(itemsDictionary);

        return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventTaskChannel, cancellationToken);
    }
}