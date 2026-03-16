using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VtubeStudioAdapter.Commands.PropertyModel.Physics;
using VtubeStudioAdapter.Models;
using VtubeStudioAdapter.Models.PropertyModel.Physics;
using VtubeStudioAdapter.Services;

namespace VtubeStudioAdapter.Handlers
{
    public class GetPhysicsHandler(PhysicsService physicsService, ILogger<GetPhysicsHandler> logger) : IRequestHandler<GetPhysicsQuery, VTSData>
    {
        public async Task<VTSData> Handle(GetPhysicsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(GetPhysicsHandler), typeof(GetPhysicsQuery).Name);
            var result = await physicsService.GetPhysicsParametrs(request);
            logger.LogInformation("Exiting {Handler}", nameof(GetPhysicsHandler));
            return result;
        }
    }

    public class ChangePhysicsHandler(PhysicsService physicsService, ILogger<ChangePhysicsHandler> logger) : IRequestHandler<ChangePhysicsParametrs, VTSData>
    {
        public async Task<VTSData> Handle(ChangePhysicsParametrs request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(ChangePhysicsHandler), typeof(ChangePhysicsParametrs).Name);
            await physicsService.ChangePhysicParametr(request);
            logger.LogInformation("Exiting {Handler}", nameof(ChangePhysicsHandler));
            return request;
        }
    }
}

