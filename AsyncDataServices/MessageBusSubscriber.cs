using CommandsService.EventProcessing;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandsService.AsyncDataServices
{
    public sealed class MessageBusSubscriber : BackgroundService
    {
        private readonly RabbitMQOptions configuration;
        private readonly IEventProcessor eventProcessor;
        private readonly ILogger<MessageBusSubscriber> logger;
        
        private readonly IConnection? connection = null;
        private readonly IModel? channel = null;
        private readonly string? queueName = null;
        private EventingBasicConsumer? consumer = null;

        public MessageBusSubscriber(IOptions<RabbitMQOptions> configuration, IEventProcessor eventProcessor, ILogger<MessageBusSubscriber> logger)
        {
            this.configuration = configuration.Value;
            this.eventProcessor = eventProcessor;
            this.logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = this.configuration.Host,
                Port = this.configuration.Port
            };

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

                connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                this.logger.LogInformation("Listening on the Message Bus...");
            }
            catch (Exception ex)
            {
                this.logger.LogError("Could not connect to the Message Bus: {ExceptionMessage}", ex.Message);
                throw;
            }
        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            logger.LogInformation("Event Received!");

            var body = e.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            eventProcessor.ProcessEvent(notificationMessage);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            logger.LogInformation("Message Bus connection Shutdown");
        }

        public override void Dispose()
        {
            if (consumer is not null)
            {
                consumer.Received -= Consumer_Received;
            }

            if (connection is not null && channel is not null) 
            {
                if (channel.IsOpen)
                {
                    channel.Close();
                    connection.Close();
                }

                connection.ConnectionShutdown -= RabbitMQ_ConnectionShutdown;
            }
            
            base.Dispose();
        }
    }
}
