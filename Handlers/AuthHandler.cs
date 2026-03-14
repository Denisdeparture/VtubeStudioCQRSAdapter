using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VtubeStudioAdapter.Commands.Auth;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Handlers
{
    public class AuthHandler : IRequestHandler<AuthQuery, VTSData>
    {
        private readonly AuthService _service;
        private readonly ILogger<AuthHandler> _logger;

        public AuthHandler(AuthService service, ILogger<AuthHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<VTSData> Handle(AuthQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Entering {Handler} with request {RequestType}", nameof(AuthHandler), typeof(AuthQuery).Name);
            var result = await _service.AuthApp(request);
            _logger.LogInformation("Exiting {Handler}", nameof(AuthHandler));
            return result;
        }
    }
}