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
    }
}