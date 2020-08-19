using Microsoft.Extensions.Logging;
using SlackNotifierWS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ThermalNotifierWS.Service.NotifyTemperatureConditionProviders;
using ThermometerWS.Service;

namespace ThermalNotifierWS.Service
{
    public class ThermalNotifierService : IThermalNotifierService
    {
        private readonly ILogger _logger;
        private readonly IThermometerService _thermometerService;
        private readonly ISlackNotifierService _slackNotifierService;
        private const double MinTemperature = 25;
        private const double MaxTemperature = 28;
        private const double BufferTemperature = 0.1;

        public ThermalNotifierService(IThermometerService thermometerService, ISlackNotifierService slackNotifierService, ILogger logger)
        {
            _thermometerService = thermometerService;
            _slackNotifierService = slackNotifierService;

            _logger = logger;
        }

        public async Task<bool> AlertTemperatureAsync()
        {
            return await NotifyTemperatureIfNeeded(new INotifyTemperatureProvider[]
            {
                new NotifyOnBreachingAllowedRange(MinTemperature - BufferTemperature, MaxTemperature + BufferTemperature),
                new NotifyOnRevertingToAllowedRange(MinTemperature - BufferTemperature, MaxTemperature + BufferTemperature)
            });
        }

        public async Task<bool> NotifyTemperatureAsync()
        {
            return await NotifyTemperatureIfNeeded(new [] { new NotifyAlways() });
        }

        private async Task<bool> NotifyTemperatureIfNeeded(INotifyTemperatureProvider[] notifyTemperatureProviders)
        {
            double? temperature = await _thermometerService.GetTempratureAsync();
            if (!temperature.HasValue)
            {
                _logger.LogError($"thermometerService failed");
                return false;
            }

            double? previouslyKnownTemperature = ThermalNotifierServiceTemperatureHistory.LastKnownTemperature;
            ThermalNotifierServiceTemperatureHistory.LastKnownTemperature = temperature;

            INotifyTemperatureProvider notifyTemperatureProvider = FindNotificationProvider(notifyTemperatureProviders, temperature.Value, previouslyKnownTemperature);

            if (notifyTemperatureProvider == null)
            {
                _logger.LogDebug($"requestTemperature succeeded, but temperature is OK: {temperature}℃");
                return true;
            }
            string SlackEndpoint = "https://hooks.slack.com/services/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";
            var notificationRequestUrlBuilder = new UriBuilder(SlackEndpoint);

            string encodedMessage = UrlEncoder.Default.Encode(notifyTemperatureProvider.GenerateMessage(temperature.Value));
            notificationRequestUrlBuilder.Query = $"payload={encodedMessage}";
            bool success = await _slackNotifierService.NotifyAsync(SlackEndpoint, encodedMessage);

            if (!success)
            {
                ThermalNotifierServiceTemperatureHistory.LastKnownTemperature = previouslyKnownTemperature;
            }

            return true;
        }

        private static INotifyTemperatureProvider FindNotificationProvider(INotifyTemperatureProvider[] notifyTemperatureProviders, double temperature, double? previousTemperature)
        {
            foreach (INotifyTemperatureProvider notifyTemperatureProvider in notifyTemperatureProviders)
            {
                if (notifyTemperatureProvider.ShouldNotify(temperature, previousTemperature))
                {
                    return notifyTemperatureProvider;
                }
            }

            return null;
        }
    }
}
