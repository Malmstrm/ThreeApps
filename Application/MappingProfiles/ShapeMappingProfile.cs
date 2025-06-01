using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class ShapeMappingProfile : Profile
{
    public ShapeMappingProfile()
    {
        CreateMap<ShapeCalculation, ShapeCalculationDTO>()
            .ForMember(dest => dest.Date,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Parameters,
                opt => opt.MapFrom(src =>
                    src.ShapeParameters.Select(sp => new ParameterDTO()
                    {
                        ParameterType = sp.ParameterType,
                        Value = sp.Value,
                    })
                    .ToList()
                )
            );
    }
}
