using System.Collections.Generic;
using Jil;

namespace WebService
{
    internal class ShieldEvent
    {
        [JilDirective(Name = "ipAddress")]
        public string IPAddress { get; set; }

        [JilDirective(Name = "occurredAt")]
        public string OccurredAt { get; set; }

        [JilDirective(Name = "pathAndQuery")]
        public string PathAndQuery { get; set; }

        internal static Dictionary<string, List<ShieldEvent>> CreateShieldEventIndex(
            IEnumerable<ShieldEvent> shieldEvents)
        {
            if (shieldEvents == null)
            {
                return new Dictionary<string, List<ShieldEvent>>();
            }

            var shieldEventIndex = new Dictionary<string, List<ShieldEvent>>();

            foreach (var shieldEvent in shieldEvents)
            {
                List<ShieldEvent> shieldEventsByIPAddress;

                var shieldEventIndexExists =
                    shieldEventIndex.TryGetValue(shieldEvent.IPAddress, out shieldEventsByIPAddress);

                if (shieldEventIndexExists)
                {
                    shieldEventsByIPAddress.Add(shieldEvent);
                }
                else
                {
                    shieldEventIndex.Add(shieldEvent.IPAddress, new List<ShieldEvent>
                    {
                        shieldEvent
                    });
                }
            }

            return shieldEventIndex;
        }
    }
}