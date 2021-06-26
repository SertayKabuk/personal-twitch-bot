using System.Collections.Generic;

namespace DoberDogBot.UI
{
    public class InitialApplicationState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }
}
