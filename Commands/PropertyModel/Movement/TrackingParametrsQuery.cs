using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Commands.PropertyModel.Movement
{
    public class TrackingParametrsQuery : IRequest<VTSData>
    {
        public VTSData.ArtMeshParameter? CustomParametrs { get; set; }
        public VTSData.ArtMeshParameter? DefaultParametrs { get; set; }

        public required Action<VTSData> OnCompleted { get; set; }

        public static implicit operator VTSData(TrackingParametrsQuery model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
                cfg.CreateMap<TrackingParametrsQuery, VTSData>()
                    .ForMember((data => data.CustomParametrs), opt => opt.MapFrom(x => x.CustomParametrs))
                    .ForMember((data => data.DefaultParametrs), opt => opt.MapFrom(x => x.DefaultParametrs))
            ));
            var data = map.Map<TrackingParametrsQuery, VTSData>(model);
            return data;
        }
    }

}