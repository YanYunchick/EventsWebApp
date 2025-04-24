using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsWebApp.Infrastructure.Repository.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "3472791a-25b5-4804-ad2d-ad75ae671c57",
                Name = "Participant",
                NormalizedName = "PARTICIPANT"
            },
            new IdentityRole
            {
                Id = "fd9e896e-29dd-4d9a-89dd-853f462ff988",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            }
        );
    }
}
