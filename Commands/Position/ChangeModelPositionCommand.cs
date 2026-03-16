using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;
using static VtubeStudioAdapter.Models.VTSData;

namespace VtubeStudioAdapter.Commands.Position
{
    public class ChangeModelPositionCommand : IRequest<VTSData>
    {
        public string? PluginName { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public double Rotation { get; set; }

        public double Size { get; set; }
        public double TimeInSeconds { get; set; }

        public bool ValuesAreRelativeToModel { get; set; }

        public static implicit operator VTSData(ChangeModelPositionCommand model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
                cfg.CreateMap<ChangeModelPositionCommand, VTSData>()
                    .ForMember((data => data.PluginName), opt => opt.MapFrom(x => x.PluginName))
                    .ForMember((data => data.PositionX), opt => opt.MapFrom(x => x.PositionX))
                    .ForMember((data => data.PositionY), opt => opt.MapFrom(x => x.PositionY))
                    .ForMember((data => data.Size), opt => opt.MapFrom(x => x.Size))
                    .ForMember((data => data.TimeInSeconds), opt => opt.MapFrom(x => x.TimeInSeconds))
                    .ForMember((data => data.Rotation), opt => opt.MapFrom(x => x.Rotation))
                    .ForMember((data => data.ValuesAreRelativeToModel), opt => opt.MapFrom(x => x.ValuesAreRelativeToModel))
            ));
            var data = map.Map<ChangeModelPositionCommand, VTSData>(model);
            return data;
        }
    }
}