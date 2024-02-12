using Microsoft.Extensions.Options;
using Shared.Enums.RabbitMQ;
using Shared.Helpers;
using Shared.Models.RabbitMq;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using SqlSugar;

namespace WebhookService.Consumers.Worker;

[Consumer("worker.channel.availability.update", "worker.channel.availability.update.handler")]
public class WorkerChannelAvailabilityUpdateConsumer(
    IOptionsMonitor<RabbitMqOptions> options,
    IRabbitMqService rabbitMqService,
    ILogger<WorkerChannelAvailabilityUpdateConsumer> logger,
    IMessageSerializer messageSerializer,
    ISqlSugarClient db)
    : TopicConsumer(options, rabbitMqService, logger, messageSerializer, db)
{
    private readonly DatabaseHelper _databaseHelper = new(db, logger);

    protected override async Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken)
    {
        if (!message.Items.Any()) return MessageReceiveResult.Failed;

        var itemsDictionary = message.Items.ToDictionary(k => k.Key, v => v.Value);
        var twilioEventWorker = ObjectHelper.CopyPropertiesFromDictionary<TwilioEventWorker>(itemsDictionary);
        twilioEventWorker.WorkerObj = ObjectHelper.CopyPropertiesFromDictionary<TwilioWorkerField>(itemsDictionary);
        twilioEventWorker.TaskChannelObj = ObjectHelper.CopyPropertiesFromDictionary<TwilioTaskChannelField>(itemsDictionary);
                
        return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventWorker, cancellationToken);
    }
}