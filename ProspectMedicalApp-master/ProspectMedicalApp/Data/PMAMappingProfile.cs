using AutoMapper;
using ProspectMedicalApp.Data.Entities;
using ProspectMedicalApp.Data.Entity;
using ProspectMedicalApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Data
{
    public class PMAMappingProfile : Profile
    {
        public PMAMappingProfile()
        {
            CreateMap<Patient, PatientViewModel>()
                .ReverseMap();

            CreateMap<Doctor, DoctorViewModel>()
                .ReverseMap();

            CreateMap<Record, RecordViewModel>()
                .ReverseMap();

            CreateMap<Bill, BillViewModel>()
                .ReverseMap();

            CreateMap<Procedure, ProcedureViewModel>()
                .ReverseMap();

            CreateMap<Appointment, AppointmentViewModel>()
                .ReverseMap();
        }
    }
}
