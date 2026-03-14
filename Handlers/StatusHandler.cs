using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VtubeStudioAdapter.Commands;
using VtubeStudioAdapter.Models;
using VtubeStudioAdapter.Services;

namespace VtubeStudioAdapter.Handlers
{
    public class StatusHandler(StatusService statusService, ILogger<StatusHandler> logger) : IRequestHandler<StatusVTSModelQuery, VTSData>
    {
        public async Task<VTSData> Handle(StatusVTSModelQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(StatusHandler), typeof(StatusVTSModelQuery).Name);
            var result = await statusService.GetCurrentStatus();
            logger.LogInformation("Exiting {Handler}", nameof(StatusHandler));
            return result;
        }
    }
}

