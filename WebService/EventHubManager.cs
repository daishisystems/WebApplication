using System;
using Microsoft.ServiceBus.Messaging;

namespace WebService
{
    /// <summary>
    ///     EventHubManager is a Singleton that ensures only 1 instance of
    ///     <see cref="EventHubClient" /> is leveraged throughout the application,
    ///     across all sessions. This in turn ensures that the same underlying TCP and
    ///     AMQP components are referenced across all running threads. The point is to
    ///     minimise the number of open connections, and to reuse any existing
    ///     connections insofar as possible.
    /// </summary>
    internal sealed class EventHubManager
    {
        private static readonly Lazy<EventHubManager> Lazy =
            new Lazy<EventHubManager>(() => new EventHubManager());

        /// <summary>
        ///     <see cref="EventHubClient" /> is the mechanism used to communicate with the
        ///     Event Hub.
        /// </summary>
        public EventHubClient EventHubClient;

        /// <summary>
        ///     <see cref="EventHubManager" /> is a private constructor, facilitating the
        ///     Singleton pattern.
        /// </summary>
        private EventHubManager()
        {
        }

        /// <summary>
        ///     <see cref="Instance" /> is a Singleton reference to this
        ///     <see cref="EventHubManager" />.
        /// </summary>
        public static EventHubManager Instance => Lazy.Value;

        /// <summary>
        ///     <see cref="PartitionKey" /> is the Partition-key pertaining to the Event
        ///     Hub. It facilitates the publishing of <see cref="EventData" /> instances to
        ///     specific Event Hub Partitions.
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        ///     <see cref="IsInitialised" /> determines whether or not this
        ///     <see cref="EventHubManager" /> is initialised.
        /// </summary>
        public bool IsInitialised => !string.IsNullOrEmpty(PartitionKey);

        /// <summary>
        ///     <see cref="IsConnected" /> determines whether the underlying Event Hub
        ///     connection is open.
        /// </summary>
        public bool IsConnected => EventHubClient != null && EventHubClient.IsClosed == false;

        /// <summary>
        ///     <see cref="Initialise" /> assigns a new <see cref="Guid" /> to
        ///     <see cref="PartitionKey" />.
        /// </summary>
        public void Initialise()
        {
            PartitionKey = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     <see cref="Connect" /> establishes a connection to an Event Hub.
        /// </summary>
        /// <param name="connectionString">
        ///     <see cref="connectionString" />is the Event Hub connection-string.
        /// </param>
        /// <param name="eventHubName">
        ///     <see cref="eventHubName" /> is the name of the Event Hub.
        /// </param>
        public void Connect(string connectionString, string eventHubName)
        {
            if (!IsInitialised)
            {
                Initialise();
            }

            if (!IsConnected)
            {
                EventHubClient = EventHubClient
                    .CreateFromConnectionString(connectionString, eventHubName);
            }
        }
    }
}