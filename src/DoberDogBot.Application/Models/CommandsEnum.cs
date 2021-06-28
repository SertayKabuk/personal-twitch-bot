using DoberDogBot.Domain.SeedWork;

namespace DoberDogBot.Application.Models
{
    public class CommandsEnum : Enumeration
    {
        public static CommandsEnum Attac = new(1, "!attac");
        public static CommandsEnum Protec = new(2, "!protec");
        public static CommandsEnum BotOnJoined = new(3, "BotOnJoined");
        public static CommandsEnum Sleep = new(4, "!sleep");
        public static CommandsEnum Wake = new(5, "!wake");
        public static CommandsEnum AutoSleep = new(6, "AutoSleep");
        public static CommandsEnum AutoWake = new(7, "AutoWake");
        public static CommandsEnum Bit= new(8, "Bit");
        public static CommandsEnum Tip = new(9, "Tip");
        public static CommandsEnum Subscriber = new(10, "Subscriber");
        public static CommandsEnum Pet = new(11, "!pet");
        public static CommandsEnum SubWelcome = new(12, "SubWelcome");
        public static CommandsEnum PokeChatter = new(13, "PokeChatter");
        public static CommandsEnum StreamStart = new(14, "StreamStart");
        public static CommandsEnum StreamEnd = new(15, "StreamEnd");

        public CommandsEnum(int id, string name) : base(id, name)
        {
        }
    }
}
