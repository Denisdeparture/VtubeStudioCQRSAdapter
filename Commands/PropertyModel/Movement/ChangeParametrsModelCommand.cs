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
        public required string PluginName { get; set; }
        public ModeModel Mode { get; set; }

        public static implicit operator VTSData(ChangeParametrsModelCommand model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChangeParametrsModelCommand, VTSData>()
                    .ForMember((data => data.FaceFound), opt => opt.MapFrom(x => x.FaceFound))
                    .ForMember((data => data.Mode), opt => opt.MapFrom(x => x.Mode.ToString().ToLower()))
                    .ForMember((data => data.ParameterValues), opt => opt.MapFrom(x => x.ParameterValues));
            }));

            var data = map.Map<ChangeParametrsModelCommand, VTSData>(model);
            return data;
        }
    }
    public enum ModeModel
    {
        Set,
        Add
    }

}