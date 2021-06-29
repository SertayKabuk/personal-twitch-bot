using DoberDogBot.Domain.AggregatesModel.StreamerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoberDogBot.Infrastructure.AppDb.EntityConfigurations
{
    public class StreamerEntityTypeConfiguration : IEntityTypeConfiguration<Streamer>
    {
        public void Configure(EntityTypeBuilder<Streamer> streamerConfiguration)
        {
            streamerConfiguration.ToTable("streamers");

            streamerConfiguration.HasKey(o => o.Id);

            streamerConfiguration.Ignore(b => b.DomainEvents);

            streamerConfiguration.Property(o => o.Id)
            .UseHiLo("botseq");

            streamerConfiguration
            .Property<string>("_channel")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Channel")
            .IsRequired(true);

            streamerConfiguration
            .Property<string>("_channelId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("ChannelId")
            .IsRequired(true);

            var navigation = streamerConfiguration.Metadata.FindNavigation(nameof(Streamer.StreamerSessions));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
