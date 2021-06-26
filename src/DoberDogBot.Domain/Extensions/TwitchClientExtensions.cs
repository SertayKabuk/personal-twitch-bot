using TwitchLib.Client.Models;

namespace DoberDogBot.Domain.Extensions
{
    public static class TwitchClientExtensions
    {
        public static string GetTargetUserName(this ChatMessage chatMessage)
        {
            int startIndex = chatMessage.Message.IndexOf("@");
            int lastIndex = chatMessage.Message.IndexOf(" ");

            if (startIndex > 0 && lastIndex > 0)
            {
                return chatMessage.Message.Substring(startIndex + 1);
            }

            return string.Empty;
        }
    }
}