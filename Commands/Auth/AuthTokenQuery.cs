using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Commands.Auth
{
    public class AuthTokenQuery : IRequest<VTSData>
    {
        public string? AuthToken { get; set; }

        public string? PluginName { get; set; }

        public string? PluginDeveloper { get; set; }
        public required Action<VTSData> OnCompleted { get; set; }

        public static implicit operator VTSData(AuthTokenQuery model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
           cfg.CreateMap<AuthTokenQuery, VTSData>()
            .ForMember((data => data.AuthToken), opt => opt.MapFrom(x => x.AuthToken))
            .ForMember((data => data.PluginName), opt => opt.MapFrom(x => x.PluginName))
            .ForMember((data => data.PluginDeveloper), opt => opt.MapFrom(x => x.PluginDeveloper))
           ));

            var res = map.Map<AuthTokenQuery, VTSData>(model);

            return res;
        }
    }
}