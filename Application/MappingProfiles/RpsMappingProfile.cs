using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class RpsMappingProfile : Profile
{
    public RpsMappingProfile()
    {
        CreateMap<RPSGame, RpsGameDTO>()
            .ForMember(d => d.PlayedAt, c => c.MapFrom(s => s.CreatedAt));
    }
}
