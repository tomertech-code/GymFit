using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymFit.Application.DTOs;
using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymFit.Application.Mappings
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Member, MemberDto>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.User.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.User.LastName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User.PhoneNumber))
                .ForMember(d => d.AssignedTrainerName, o => o.MapFrom(s =>
                    s.AssignedTrainer != null ?
                    s.AssignedTrainer.User.FirstName + " " + s.AssignedTrainer.User.LastName : null));

            CreateMap<MemberViewModel, Member>();
            CreateMap<ContactViewModel, ContactMessage>();
        }
    }
}
