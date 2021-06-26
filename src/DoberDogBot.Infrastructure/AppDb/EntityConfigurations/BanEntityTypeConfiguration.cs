using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DoberDogBot.Infrastructure.AppDb.EntityConfigurations
{
    class BanEntityTypeConfiguration : IEntityTypeConfiguration<Ban>
    {
        public void Configure(EntityTypeBuilder<Ban> banConfiguration)
        {
            banConfiguration.ToTable("bans");

            banConfiguration.HasKey(o => o.Id);

            banConfiguration.Ignore(b => b.DomainEvents);

            banConfiguration.Property(o => o.Id)
                .UseHiLo("banseq");

            banConfiguration.Property<int>("BotId").IsRequired();

            banConfiguration
                .Property<DateTime>("_banDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("BanDate")
                .IsRequired();
        }
    }
}
