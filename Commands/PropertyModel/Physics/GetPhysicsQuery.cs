using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;
using static VtubeStudioAdapter.Models.VTSData;
using static VtubeStudioAdapter.Models.VTSData.PhysicParamter;

namespace VtubeStudioAdapter.Commands.PropertyModel.Physics
{
    public class GetPhysicsQuery : IRequest<VTSData>
    {
        public required string? PluginName { get; set; }

        public bool ModelHasPhysics { get; set; }

        public bool PhysicsSwitchedOn { get; set; }

        public bool UsingLegacyPhysics { get; set; }

        public int PhysicsFPSSetting { get; set; }

        public int BaseStrength { get; set; }

        public int BaseWind { get; set; }

        public bool ApiPhysicsOverrideActive { get; set; }

        public string? ApiPhysicsOverridePluginName { get; set; }
        public List<PhysicsGroup>? PhysicsGroups { get; set; }

        public required Action<VTSData> OnCompleted { get; set; }

        public static implicit operator VTSData(GetPhysicsQuery model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
                cfg.CreateMap<GetPhysicsQuery, VTSData>()
                    .ForMember((data => data.PluginName), opt => opt.MapFrom(x => x.PluginName))
                    .ForMember((data => data.ModelHasPhysics), opt => opt.MapFrom(x => x.ModelHasPhysics))
                    .ForMember((data => data.PhysicsSwitchedOn), opt => opt.MapFrom(x => x.PhysicsSwitchedOn))
                    .ForMember((data => data.UsingLegacyPhysics), opt => opt.MapFrom(x => x.UsingLegacyPhysics))
                    .ForMember((data => data.PhysicsFPSSetting), opt => opt.MapFrom(x => x.PhysicsFPSSetting))
                    .ForMember((data => data.BaseStrength), opt => opt.MapFrom(x => x.BaseStrength))
                    .ForMember((data => data.BaseWind), opt => opt.MapFrom(x => x.BaseWind))
                    .ForMember((data => data.ApiPhysicsOverrideActive), opt => opt.MapFrom(x => x.ApiPhysicsOverrideActive))
                    .ForMember((data => data.ApiPhysicsOverridePluginName), opt => opt.MapFrom(x => x.ApiPhysicsOverridePluginName))
                    .ForMember((data => data.PhysicsGroups), opt => opt.MapFrom(x => x.PhysicsGroups))
            ));
            var data = map.Map<GetPhysicsQuery, VTSData>(model);
            return data;
        }
    }

}