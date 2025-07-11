using AutoMapper;
using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Entities.Identity;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.DTOs.IdentityDTO;

namespace Hospital_Management_System.Helper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Patient,PatientDTO>().ReverseMap();

            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dest => dest.doctorAvailability, opt => opt.MapFrom(src => src.DoctorAvailabilities ))
                .ReverseMap();

            CreateMap<DoctorAvailability, DoctorAvailabilityDTO>()
                .ReverseMap();

            CreateMap<Appointment, AppointmentDTO>().ReverseMap();

            CreateMap<Appointment, AppointmentDetailsDTO>()
                .ReverseMap();

            CreateMap<MedicalReport, MedicalReportDTO>().ReverseMap();
            CreateMap<UserApp, UserResponseDTO>();
            CreateMap<UserApp, NewPatientDTO>();
        }
    }
}
