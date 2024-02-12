using Microsoft.Extensions.Options;
using Shared.Enums.RabbitMQ;
using Shared.Helpers;
using Shared.Models.RabbitMq;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using SqlSugar;

namespace WebhookService.Consumers.Workspace;

[Consumer("workspace.created", "workspace.created.handler")]
public class WorkspaceCreatedConsumer(
    IOptionsMonitor<RabbitMqOptions> options,
    IRabbitMqService rabbitMqService,
    ILogger<WorkspaceCreatedConsumer> logger,
    IMessageSerializer messageSerializer,
    ISqlSugarClient db)
    : TopicConsumer(options, rabbitMqService, logger, messageSerializer, db)
{
    private readonly DatabaseHelper _databaseHelper = new(db, logger);

    protected override async Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken)
    {
        var itemsDictionary = message.Items.ToDictionary(k => k.Key, v => v.Value);
        var twilioEventWorkspace = ObjectHelper.CopyPropertiesFromDictionary<TwilioEventWorkspace>(itemsDictionary);

        return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventWorkspace, cancellationToken);
    }
}