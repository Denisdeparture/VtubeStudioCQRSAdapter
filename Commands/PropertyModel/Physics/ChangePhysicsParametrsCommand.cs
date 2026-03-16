using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using static VtubeStudioAdapter.Models.VTSData;

namespace VtubeStudioAdapter.Models.PropertyModel.Physics
{
    public class ChangePhysicsParametrs : IRequest<VTSData>
    {
        public required string? PluginName { get; set; }

        public PhysicParamter[]? Strength { get; set; }
        public PhysicParamter[]? Wind { get; set; }

        public static implicit operator VTSData(ChangePhysicsParametrs model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
                cfg.CreateMap<ChangePhysicsParametrs, VTSData>()
                    .ForMember((data => data.PluginName), opt => opt.MapFrom(x => x.PluginName))
                    .ForMember((data => data.Strength), opt => opt.MapFrom(x => x.Strength))
                    .ForMember((data => data.Wind), opt => opt.MapFrom(x => x.Wind))
            ));
            var data = map.Map<ChangePhysicsParametrs, VTSData>(model);
            return data;
        }
    }

}