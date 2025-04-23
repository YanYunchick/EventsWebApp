using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsWebApp.Infrastructure.Repository.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(e => e.StartDateTime)
            .IsRequired();

        builder.Property(e => e.Location)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Category)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.MaxParticipants)
            .IsRequired();

        builder.Property(e => e.ImagePath)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasMany(e => e.UserEvents)
            .WithOne(ue => ue.Event)
            .HasForeignKey(ue => ue.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
