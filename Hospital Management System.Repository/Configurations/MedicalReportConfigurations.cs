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
    public class MedicalReportConfigurations : IEntityTypeConfiguration<MedicalReport>
    {
        public void Configure(EntityTypeBuilder<MedicalReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.VisitDate)
                .IsRequired();
            builder.Property(x => x.Diagnosis)
                .IsRequired()
                .HasMaxLength(1000);
            builder.Property(x => x.Treatment)
                .IsRequired()
                .HasMaxLength(1000);
            builder.HasOne(x => x.Patient)
                .WithMany(x => x.MedicalReports)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x=>x.Doctor)
                .WithMany(x=>x.MedicalReports)
                .HasForeignKey(x=>x.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);








        }
    }
}
