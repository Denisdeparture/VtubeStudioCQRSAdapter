using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Commands.PropertyModel.Movement
{
    public class ArtMeshModelQuery : IRequest<VTSData>
    {
        public bool ModelLoaded { get; set; }

        public int NumberOfArtMeshNames { get; set; }

        public int NumberOfArtMeshTags { get; set; }

        public string[]? ArtMeshNames { get; set; }

        public string[]? ArtMeshTags { get; set; }

        public static implicit operator VTSData(ArtMeshModelQuery model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
                cfg.CreateMap<ArtMeshModelQuery, VTSData>()
                    .ForMember((data => data.ModelLoaded), opt => opt.MapFrom(x => x.ModelLoaded))
                    .ForMember((data => data.NumberOfArtMeshNames), opt => opt.MapFrom(x => x.NumberOfArtMeshNames))
                    .ForMember((data => data.NumberOfArtMeshTags), opt => opt.MapFrom(x => x.NumberOfArtMeshTags))
                    .ForMember((data => data.ArtMeshNames), opt => opt.MapFrom(x => x.ArtMeshNames))
                    .ForMember((data => data.ArtMeshTags), opt => opt.MapFrom(x => x.ArtMeshTags))
            ));
            var data = map.Map<ArtMeshModelQuery, VTSData>(model);
            return data;
        }
    }
}