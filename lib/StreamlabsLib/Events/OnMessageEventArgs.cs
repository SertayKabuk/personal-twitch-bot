using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StreamlabsLib.Events
{
    public class OnMessageEventArgs : EventArgs
    {
        public OnMessageEventArgs(string wirePayload)
        {
            var m = eventRegex.Match(wirePayload);
            if (m.Success)
            {
                Nsp = m.Groups[1].Success ? m.Groups[0].Value : "/";
                Id = m.Groups[2].Success ? Convert.ToInt64(m.Groups[2].Value, CultureInfo.InvariantCulture) : null;
                Payload = m.Groups[3].Value;
            }
            else
            {
                Payload = string.Empty;
            }
        }

        static readonly Regex eventRegex = new Regex("^(?:(/[^,]*),)?([0-9]{1,19})?(.*)$");

        /// <summary>
        /// Namespace
        /// </summary>
        public string Nsp { get; private set; }

        /// <summary>
        /// Id for ACK according to the socket.io protocol (Not handled by this)
        /// </summary>
        public long? Id { get; private set; }

        /// <summary>
        /// JSON Payload
        /// </summary>
        public string Payload { get; private set; }
    }
}
