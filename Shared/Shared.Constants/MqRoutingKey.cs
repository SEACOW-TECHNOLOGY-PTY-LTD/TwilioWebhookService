namespace Shared.Constants;

public static class MqRoutingKey
{
    public const string UserCreated = "user.created";
    public const string UserUpdated = "user.updated";
    public const string UserDeleted = "user.deleted";
    
    public const string LeadPushed = "lead.pushed";

    public const string AgentAvailable = "agent.available";
    
    public const string AssignLead = "assign.lead";
    
    public const string DispositionAdvisedToStay = "disposition.advised.to.stay";
    public const string DispositionContactInFuture = "disposition.contact.in.future";
    public const string DispositionContactInFutureQualified = "disposition.contact.in.future.qualified";
    public const string DispositionDirectToFund = "disposition.direct.to.fund";
    public const string DispositionDuplicatedLead = "disposition.duplicated.lead";
    public const string DispositionEmailOnly = "disposition.email.only";
    public const string DispositionIneligible = "disposition.ineligible";
    public const string DispositionNoAnswer = "disposition.no.answer";
    public const string DispositionNotInterested = "disposition.not.interested";
    public const string DispositionQuote = "disposition.quote";
    public const string DispositionSale = "disposition.sale";
    public const string DispositionTransferredLtOnly = "disposition.transferred.lt.only";
    public const string DispositionWrongNumber = "disposition.wrong.number";
}