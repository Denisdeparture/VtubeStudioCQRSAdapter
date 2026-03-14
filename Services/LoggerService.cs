using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VtubeStudioAdapter.Services
{
    public class LoggerService(ILogger<LoggerService> logger)
    {
        public void LogInfo(string str, params object[] objects)
        {
            logger.LogInformation(str, objects);
        }
        public void LogImportant(string str, params object[] objects)
        {
            logger.LogWarning(str, objects);
        }
    }
}