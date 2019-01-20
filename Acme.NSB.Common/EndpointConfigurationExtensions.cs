using System;
using NServiceBus;

namespace Acme.NSB.Common
{
    static class EndpointConfigurationExtensionMethods
    {
        public static RoutingSettings<RabbitMQTransport> ApplyCommonConfiguration(this EndpointConfiguration endpointConfiguration)
        {
            var connectionString = Environment.GetEnvironmentVariable("RabbitMQConnectionString");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("RabbitMQConnectionString environment has not been set.");
            }

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();

            transport.ConnectionString(connectionString);
            transport.UseConventionalRoutingTopology();


            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            endpointConfiguration.EnableMetrics()
                .SendMetricDataToServiceControl(
                    serviceControlMetricsAddress: "ServiceControl.Monitor",
                    interval: TimeSpan.FromSeconds(2));

            return transport.Routing();
        }
    }
}
