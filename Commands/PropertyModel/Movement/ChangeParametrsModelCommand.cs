using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Commands.PropertyModel.Movement
{
    public class ChangeParametrsModelCommand : IRequest<VTSData>
    {
        public VTSData.Parametr[]? ParameterValues;
        public bool FaceFound { get; set; }
        public string? Mode { get; set; }

        public static implicit operator VTSData(ChangeParametrsModelCommand model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VTSData.Parametr, VTSData.Parametr>()
                    .ForMember((data => data.Id), opt => opt.MapFrom(x => x.Id))
                    .ForMember((data => data.Weight), opt => opt.MapFrom(x => x.Weight))
                    .ForMember((data => data.Value), opt => opt.MapFrom(x => x.Value));

                cfg.CreateMap<ChangeParametrsModelCommand, VTSData>()
                    .ForMember((data => data.FaceFound), opt => opt.MapFrom(x => x.FaceFound))
                    .ForMember((data => data.Mode), opt => opt.MapFrom(x => x.Mode))
                    .ForMember((data => data.ParameterValues), opt => opt.MapFrom(x => x.ParameterValues));
            }));

            var data = map.Map<ChangeParametrsModelCommand, VTSData>(model);
            return data;
        }
    }

}