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
    public class PatientConfigurations : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Age)
                .IsRequired();
            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);
            builder.Property(x => x.ChronicDiseases)
                .HasMaxLength(500);
          
          

        }
    }
}
