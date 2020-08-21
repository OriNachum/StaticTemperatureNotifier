using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThermalNotifierWS.Service.NotifyTemperatureConditionProviders
{
    public class NotifyOnRevertingToAllowedRange : INotifyTemperatureProvider
    {
        private readonly int MaximumTimeGapBetweenNotifications = 2;
        private readonly double _minTemperature;
        private readonly double _maxTemperature;

        public NotifyOnRevertingToAllowedRange(double minTemperature, double maxTemperature, int forceReminderTime)
        {
            _minTemperature = minTemperature;
            _maxTemperature = maxTemperature;
            MaximumTimeGapBetweenNotifications = forceReminderTime;
        }

        public bool ShouldNotify(double temperature, double? previousTemperature)
        {
            bool shouldNotify = TemperatureIsInRange(temperature) &&
                   (!previousTemperature.HasValue || !TemperatureIsInRange(previousTemperature.Value) || ShouldRemind());

            if (shouldNotify)
            {
                NotifyReminderHistory.LastReminderTime = DateTime.UtcNow;
            }
            return shouldNotify;
        }

        private bool TemperatureIsInRange(double temperature)
        {
            return temperature >= _minTemperature && temperature <= _maxTemperature;
        }

        private bool ShouldRemind()
        {
            DateTime? lastReminderTime = NotifyReminderHistory.LastReminderTime;
            return !lastReminderTime.HasValue || (DateTime.UtcNow - lastReminderTime.Value) > TimeSpan.FromHours(MaximumTimeGapBetweenNotifications);
        }

        public string GenerateMessage(double temperature) => $"Temperature back to allowed range: {temperature}℃";
    }
}
