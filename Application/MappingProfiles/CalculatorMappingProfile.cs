using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class CalculatorMappingProfile : Profile
{
    public CalculatorMappingProfile() 
    {
        CreateMap<CalculatorCalculation, CalculationDTO>()
            // mappa om FirstValue → Operand1
            .ForMember(dest => dest.Operand1,
                       opt => opt.MapFrom(src => src.FirstValue))
            // SecondValue → Operand2
            .ForMember(dest => dest.Operand2,
                       opt => opt.MapFrom(src => src.SecondValue))
            // Operator & Result heter samma namn, de mappas automatiskt
            // mappa CreatedAt → Date
            .ForMember(dest => dest.Date,
                       opt => opt.MapFrom(src => src.CreatedAt));
    }
}
