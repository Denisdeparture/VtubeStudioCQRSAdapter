using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VtubeStudioAdapter.Commands.PropertyModel.Movement;
using VtubeStudioAdapter.Models;
using VtubeStudioAdapter.Services;

namespace VtubeStudioAdapter.Handlers
{
    public class GetArtMeshesHandler(MovementService movementService, ILogger<GetArtMeshesHandler> logger) : IRequestHandler<ArtMeshModelQuery, VTSData>
    {
        public async Task<VTSData> Handle(ArtMeshModelQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(GetArtMeshesHandler), typeof(ArtMeshModelQuery).Name);
            var result = await movementService.GetArtMeshes();
            logger.LogInformation("Exiting {Handler}", nameof(GetArtMeshesHandler));
            return result;
        }
    }

    public class GetTrackingParametersHandler(MovementService movementService, ILogger<GetTrackingParametersHandler> logger) : IRequestHandler<TrackingParametrsQuery, VTSData>
    {
        public async Task<VTSData> Handle(TrackingParametrsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(GetTrackingParametersHandler), typeof(TrackingParametrsQuery).Name);
            var result = await movementService.GetTrackingParametrs();
            logger.LogInformation("Exiting {Handler}", nameof(GetTrackingParametersHandler));
            return result;
        }
    }

    public class ChangeParametersHandler(MovementService movementService, ILogger<ChangeParametersHandler> logger) : IRequestHandler<ChangeParametrsModelCommand, VTSData>
    {
        public async Task<VTSData> Handle(ChangeParametrsModelCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(ChangeParametersHandler), typeof(ChangeParametrsModelCommand).Name);
            var values = request.ParameterValues ?? System.Array.Empty<VTSData.Parametr>();
            await movementService.ChangeValueParametrs(values);
            logger.LogInformation("Exiting {Handler}", nameof(ChangeParametersHandler));
            return request;
        }
    }
}

