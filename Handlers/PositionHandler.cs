using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VtubeStudioAdapter.Commands.Position;
using VtubeStudioAdapter.Models;
using VtubeStudioAdapter.Services;
using VtubeStudioAdapter.Сommands.Position;

namespace VtubeStudioAdapter.Handlers
{
    public class ChangePositionHandler(PositionService positionService, ILogger<ChangePositionHandler> logger) : IRequestHandler<ChangeModelPositionCommand, VTSData>
    {
        public async Task<VTSData> Handle(ChangeModelPositionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(ChangePositionHandler), typeof(ChangeModelPositionCommand).Name);
            var result = await positionService.ChangePosition(request);
            logger.LogInformation("Exiting {Handler}", nameof(ChangePositionHandler));
            return result;
        }
    }

    public class GetCurrentModelHandler(PositionService positionService, ILogger<GetCurrentModelHandler> logger) : IRequestHandler<GetCurrentModelQuery, VTSData>
    {
        public async Task<VTSData> Handle(GetCurrentModelQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(GetCurrentModelHandler), typeof(GetCurrentModelQuery).Name);
            var result = await positionService.GetCurrentModelData(request);
            logger.LogInformation("Exiting {Handler}", nameof(GetCurrentModelHandler));
            return result;
        }
    }
}