using DoberDogBot.Domain.SeedWork;

namespace DoberDogBot.Domain.Events
{
    public class DomainResult : Enumeration
    {
        public static DomainResult Fail = new(1, "Fail");
        public static DomainResult Success = new(2, "Success");
        public static DomainResult Idempotent = new(3, "Idempotent");
        public static DomainResult Ignored = new(4, "Ignored");
        public static DomainResult NotMommy = new(5, "NotMommy");

        public DomainResult(int id, string name) : base(id, name)
        {
        }
    }
}
