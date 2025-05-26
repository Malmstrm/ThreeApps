using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class CalculatorMappingProfile : Profile
{
    public CalculatorMappingProfile() 
    {
        CreateMap<CalculatorCalculation, CalculationDTO>()
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.CreatedAt)); // Om CreatedAt är DateOnly → DateOnly→DateOnly, annars DateTime→DateTime
    }
}
