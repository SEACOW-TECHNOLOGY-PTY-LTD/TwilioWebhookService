namespace Shared.Constants;

public class EventType
{
    public const string ActivityCreated = "activity.created";                                   // An Activity is created
    public const string ActivityUpdated = "activity.updated";                                   // An Activity is updated
    public const string ActivityDeleted = "activity.deleted";                                   // An Activity is deleted
    
    public const string ReservationCreated = "reservation.created";                             // A task is assigned to a worker
    public const string ReservationAccepted = "reservation.accepted";                           // A task reservation is accepted by a worker
    public const string ReservationRejected = "reservation.rejected";                           // A task reservation is rejected by a worker
    public const string ReservationTimeout = "reservation.timeout";                             // Too much time passed without a task being accepted or rejected. The reservation is canceled.
    public const string ReservationCanceled = "reservation.canceled";                           // A task reservation is canceled before it has been accepted by a worker
    public const string ReservationRescinded = "reservation.rescinded";                         // Multi-reservation was used for a task and another worker has accepted one of the created reservations
    public const string ReservationCompleted = "reservation.completed";                         // A task reservation is completed. Aligns with task.completed.
    public const string ReservationFailed = "reservation.failed";                               // The reservation failed and was not assigned to an agent.
    public const string ReservationWrapUp = "reservation.wrapup";                               // The task reservation is being wrapped up by the agent; happens directly before task completion.
    
    public const string TaskCreated = "task.created";                                           // A task item is added to a Workspace
    public const string TaskUpdated = "task.updated";                                           // A task’s attributes are changed
    public const string TaskCanceled = "task.canceled";                                         // A task is canceled
    public const string TaskWrapUp = "task.wrapup";                                             // A task is moved to wrap up state.
    public const string TaskCompleted = "task.completed";                                       // A task is completed
    public const string TaskDeleted = "task.deleted";                                           // A task is deleted via API. Does not include auto-deleted Tasks after cancellation / completion.
    public const string TaskSystemDeleted = "task.system-deleted";                              // A task is deleted via the system, after the task reaches its TTL.
    public const string TaskTransferInitiated = "task.transfer-initiated";
    public const string TaskTransferAttemptFailed = "task.transfer-attempt-failed";
    public const string TaskTransferFailed = "task.transfer-failed";
    public const string TaskTransferCanceled = "task.transfer-canceled";
    public const string TaskTransferCompleted = "task.transfer-completed";
    
    public const string TaskChannelCreated = "task-channel.created";                            // A task channel is created
    public const string TaskChannelUpdated = "task-channel.updated";                            // A task channel is updated
    public const string TaskChannelDeleted = "task-channel.deleted";                            // A task channel is deleted
    
    public const string TaskQueueCreated = "task-queue.created";                                // A TaskQueue has been created
    public const string TaskQueueDeleted = "task-queue.deleted";                                // A TaskQueue has been deleted
    public const string TaskQueueEntered = "task-queue.entered";                                // A task enters a queue during workflow processing
    public const string TaskQueueTimeout = "task-queue.timeout";                                // A workflow routing step timed-out and a task is leaving a queue
    public const string TaskQueueMoved = "task-queue.moved";                                    // A task leaves its current queue to move to a new queue during workflow processing
    public const string TaskQueueExpressionUpdated = "task-queue.expression.updated";           // The queue expression has been updated
    
    public const string WorkerCreated = "worker.created";                                       // A worker is created
    public const string WorkerActivityUpdate = "worker.activity.update";                        // A worker’s activity is updated
    public const string WorkerAttributesUpdate = "worker.attributes.update";                    // A worker’s attributes are updated
    public const string WorkerCapacityUpdate = "worker.capacity.update";                        // A worker's channel's configured capacity has been updated
    public const string WorkerChannelAvailabilityUpdate = "worker.channel.availability.update"; // A worker's channel's availability has been updated
    public const string WorkerDeleted = "worker.deleted";                                       // A worker is deleted
    
    public const string WorkflowCreated = "workflow.created";                                   // A workflow is created
    public const string WorkflowUpdated = "workflow.updated";                                   // A workflow is updated
    public const string WorkflowDeleted = "workflow.deleted";                                   // A workflow is deleted
    public const string WorkflowTargetMatched = "workflow.target-matched";                      // A task item matches a workflow routing step
    public const string WorkflowEntered = "workflow.entered";                                   // A task enters a workflow
    public const string WorkflowTimeout = "workflow.timeout";                                   // A task reaches the end of a workflow without being accepted and is removed from the workspace
    public const string WorkflowSkipped = "workflow.skipped";                                   // A task skips-out of the workflow (because of skip_if in the final step) and is removed from the workspace
    
    public const string WorkspaceCreated = "workspace.created";                                 // A workspace is created
    public const string WorkspaceUpdated = "workspace.updated";                                 // A workspace is updated
    public const string WorkspaceDeleted = "workspace.deleted";                                 // A workspace is deleted
}