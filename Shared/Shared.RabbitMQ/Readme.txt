在 Producer 中：
主交换机的声明： 你声明了一个主交换机，类型为 Topic。
死信交换机和队列的声明： 你声明了一个死信交换机和对应的延迟队列，这个队列的参数中指定了主交换机和路由键作为死信目标。
发布消息： 你发布消息到交换机，使用的路由键是 message.EventName。如果消息有延迟，它会被发送到死信交换机，否则发送到主交换机。
在 Consumer 中：
主交换机的声明： 与 Producer 中相同的主交换机被声明。
队列的声明和绑定： 你声明了一个队列，并将其绑定到主交换机，使用的路由键是 eventName。
消费消息： 你创建了一个消费者来消费这个队列中的消息。
如何工作：
当 Producer 发布消息到主交换机时，RabbitMQ 会查看消息的路由键，并尝试将其路由到与该路由键匹配的所有队列。
在 Consumer 中，你已经将队列绑定到主交换机，并使用了与 Producer 相同的路由键 eventName。因此，任何使用该路由键发布到主交换机的消息都会被路由到这个队列。
如果消息是延迟的，它首先会被发送到死信交换机和延迟队列。当延迟时间到达后，消息会作为死信被发送到主交换机，然后再按照同样的路由规则路由到队列。
总的来说，虽然 Producer 并没有直接指定队列，但通过交换机和路由键的组合，以及 Consumer 中的队列绑定，消息可以正确路由到目标队列。这就是 RabbitMQ 的灵活之处，它允许你通过交换机和路由键的组合来灵活定义消息的路由路径。