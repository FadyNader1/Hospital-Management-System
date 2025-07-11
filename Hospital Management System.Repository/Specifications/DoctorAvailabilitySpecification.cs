using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Repository.Specifications
{
    public class DoctorAvailabilitySpecification:BaseSpecification<DoctorAvailability>
    {
        public DoctorAvailabilitySpecification()
        {
            Includes.Add(x => x.doctor);
        }
        public DoctorAvailabilitySpecification(int id) : base(x => x.Id == id)
        {
            Includes.Add(x => x.doctor);

        }
    }
    
    
}
