using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DoberDogBot.Infrastructure.AppDb.EntityConfigurations
{
    class BotEntityTypeConfiguration : IEntityTypeConfiguration<Bot>
    {
        public void Configure(EntityTypeBuilder<Bot> botConfiguration)
        {
            botConfiguration.ToTable("bots");

            botConfiguration.HasKey(o => o.Id);

            botConfiguration.Ignore(b => b.DomainEvents);

            botConfiguration.Property(o => o.Id)
            .UseHiLo("botseq");

            botConfiguration
            .Property<DateTime?>("_lastSleepTime")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("LastSleepTime")
            .IsRequired(false);

            botConfiguration
            .Property<DateTime?>("_wakeupTime")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("WakeupTime")
            .IsRequired(false);

            botConfiguration
            .Property<bool>("_wakeLock")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("WakeLock")
            .IsRequired(true)
            .HasDefaultValue(false);

            botConfiguration
            .Property<string>("_lastPokedChatterDisplayName")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("LastPokedChatterDisplayName")
            .IsRequired(false);

            var navigation = botConfiguration.Metadata.FindNavigation(nameof(Bot.Bans));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
