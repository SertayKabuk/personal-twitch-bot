using DoberDogBot.Domain.AggregatesModel.StreamerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoberDogBot.Infrastructure.AppDb.EntityConfigurations
{
    public class StreamerSessionEntityTypeConfiguration : IEntityTypeConfiguration<StreamerSession>
    {
        public void Configure(EntityTypeBuilder<StreamerSession> streamerSessionConfiguration)
        {
            streamerSessionConfiguration.ToTable("streamersessions");

            streamerSessionConfiguration.HasKey(o => o.Id);

            streamerSessionConfiguration.Ignore(b => b.DomainEvents);

            streamerSessionConfiguration.Property(o => o.Id)
                .UseHiLo("banseq");

            streamerSessionConfiguration.Property<string>("SessionId").IsRequired();

            streamerSessionConfiguration
             .Property<int>("_playDelay")
             .UsePropertyAccessMode(PropertyAccessMode.Field)
             .HasColumnName("PlayDelay")
             .IsRequired(true);

            streamerSessionConfiguration
            .Property<string>("_streamStart")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("StreamStart")
            .IsRequired(true);

            streamerSessionConfiguration
            .Property<string>("_streamEnd")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("StreamEnd")
            .IsRequired(false);
        }
    }
}
