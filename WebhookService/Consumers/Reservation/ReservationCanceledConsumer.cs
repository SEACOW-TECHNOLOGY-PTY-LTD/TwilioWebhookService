using Microsoft.Extensions.Options;
using Shared.Models.RabbitMq;
using Shared.Models.TwilioEvent;
using Shared.RabbitMQ;
using SqlSugar;
using Shared.Enums.RabbitMQ;
using Shared.Helpers;

namespace WebhookService.Consumers.Reservation;

[Consumer("reservation.canceled", "reservation.canceled.handler")]
public class ReservationCanceledConsumer(
    IOptionsMonitor<RabbitMqOptions> options,
    IRabbitMqService rabbitMqService,
    ILogger<ReservationCanceledConsumer> logger,
    IMessageSerializer messageSerializer,
    ISqlSugarClient db)
    : TopicConsumer(options, rabbitMqService, logger, messageSerializer, db)
{
    private readonly DatabaseHelper _databaseHelper = new(db, logger);

    protected override async Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken)
    {
        if (!message.Items.Any()) return MessageReceiveResult.Failed;

        var itemsDictionary = message.Items.ToDictionary(k => k.Key, v => v.Value);
        var twilioEventReservation = ObjectHelper.CopyPropertiesFromDictionary<TwilioEventReservation>(itemsDictionary);
        twilioEventReservation.TaskObj = ObjectHelper.CopyPropertiesFromDictionary<TwilioTaskField>(itemsDictionary);
        twilioEventReservation.WorkerObj = ObjectHelper.CopyPropertiesFromDictionary<TwilioWorkerField>(itemsDictionary);

        var canceledReasonCode = itemsDictionary.GetValueOrDefault("CanceledReasonCode");

        if (canceledReasonCode == null) return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventReservation, cancellationToken);
        
        return await _databaseHelper.InsertObjectToDatabaseAsync(twilioEventReservation, cancellationToken);
    }
}