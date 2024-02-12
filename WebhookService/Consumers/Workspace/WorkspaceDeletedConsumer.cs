using Microsoft.Extensions.Options;
using Shared.Enums.RabbitMQ;
using Shared.Helpers;
using Shared.Models.RabbitMq;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using SqlSugar;

namespace WebhookService.Consumers.Workspace;

[Consumer("workspace.deleted", "workspace.deleted.handler")]
public class WorkspaceDeletedConsumer(
    IOptionsMonitor<RabbitMqOptions> options,
    IRabbitMqService rabbitMqService,
    ILogger<WorkspaceDeletedConsumer> logger,
    IMessageSerializer messageSerializer,
    ISqlSugarClient db)
    : TopicConsumer(options, rabbitMqService, logger, messageSerializer, db)
{
    private readonly DatabaseHelper _databaseHelper = new(db, logger);

    protected override async Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken)
    {
        if (!message.Items.Any()) return MessageReceiveResult.Failed;

        var itemsDictionary = message.Items.ToDictionary(k => k.Key, v => v.Value);
        var twilioEventWorkspace = ObjectHelper.CopyPropertiesFromDictionary<TwilioEventWorkspace>(itemsDictionary);

        return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventWorkspace, cancellationToken);
    }
}