using Hospital_Management_System.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Repository.Configurations
{
    public class DoctorAvailabilityConfigurations : IEntityTypeConfiguration<DoctorAvailability>
    {
        public void Configure(EntityTypeBuilder<DoctorAvailability> builder)
        {

            builder.HasOne(x => x.doctor)
           .WithMany(d => d.DoctorAvailabilities)
           .HasForeignKey(x => x.doctorid)
           .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.AvailableFrom)
                .HasConversion(
                x => x.ToString(),
                x => TimeSpan.Parse(x));
            builder.Property(x => x.AvailableTo)
                .HasConversion(
                x => x.ToString(),
                x => TimeSpan.Parse(x));


        }
    }
}
