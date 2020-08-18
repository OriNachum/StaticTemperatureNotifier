using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThermalNotifierWS.Service;

namespace Bishop.Service
{
    public class BishopService : IBishopService
    {
        private readonly ILogger _logger;
        private readonly IThermalNotifierService _thermalNotifierService;
        private readonly TimeSpan SleepTime = TimeSpan.FromSeconds(10);
        private DateTime? LastRun;
        private bool disposedValue;

        public BishopService(IThermalNotifierService thermalNotifierService, ILogger logger)
        {
            _thermalNotifierService = thermalNotifierService;
            _logger = logger;
        }

        public async Task PerformNextActionAsync()
        {
            if (!LastRun.HasValue || (DateTime.UtcNow - LastRun) > SleepTime )
            {
                await NextCycleActionAsync();
                LastRun = DateTime.UtcNow;
            }
        }

        private async Task NextCycleActionAsync()
        {
            bool success = false;
            string request = "https://localhost:8001/ThermalAlert";
            _logger.LogDebug($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - httpClient Request {request}");
            try
            {
                bool response = await _thermalNotifierService.AlertTemperatureAsync();
                _logger.LogDebug($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - httpClient response {response}");

                if (!response)
                {
                    _logger.LogError($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - Error in request for next action. request: { request } response: {response}");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - Error with httpClinet");
            }

            if (!success)
            {
                _logger.LogInformation($"{nameof(BishopService)} - {nameof(NextCycleActionAsync)} - no action to perform");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
