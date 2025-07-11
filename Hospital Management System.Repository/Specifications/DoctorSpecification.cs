using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Repository.Specifications
{
    public class DoctorSpecification:BaseSpecification<Doctor>
    {
        public DoctorSpecification(string? SearchBySpecialization)
        {
            if(string.IsNullOrEmpty(SearchBySpecialization))
            {
                Criteria = x => true;
            }
            else
            {
                Criteria = x => x.Specialization.ToLower().Contains(SearchBySpecialization.ToLower());
            }
            Includes.Add(x => x.DoctorAvailabilities);
            Includes.Add(x => x.Appointments);
            AddInclude("Appointments.Patient");
            Includes.Add(x => x.MedicalReports);
        }
        public DoctorSpecification(int id):base(x=>x.Id==id)
        {
            Includes.Add(x => x.DoctorAvailabilities);
            Includes.Add(x => x.Appointments);
            AddInclude("Appointments.Patient");
            Includes.Add(x => x.MedicalReports);

        }

    }
}
