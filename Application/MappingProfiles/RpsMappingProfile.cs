using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class RpsMappingProfile : Profile
{
    public RpsMappingProfile()
    {
        CreateMap<RPSGame, RpsGameDTO>()
                .ForMember(dest => dest.PlayedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.Games))
                .ForMember(dest => dest.Wins, opt => opt.MapFrom(src => src.Wins))
                .ForMember(dest => dest.Losses, opt => opt.MapFrom(src => src.Losses))
                .ForMember(dest => dest.Ties, opt => opt.MapFrom(src => src.Ties));
    }
}
