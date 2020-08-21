using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThermalNotifierWS.Service.NotifyTemperatureConditionProviders
{
    public class NotifyOnBreachingAllowedRange : INotifyTemperatureProvider
    {
        private readonly int MaximumTimeGapBetweenNotifications = 60;
        private readonly double _minTemperature;
        private readonly double _maxTemperature;

        public NotifyOnBreachingAllowedRange(double minTemperature, double maxTemperature, int forceReminderTime)
        {
            _minTemperature = minTemperature;
            _maxTemperature = maxTemperature;
            MaximumTimeGapBetweenNotifications = forceReminderTime;
        }

        public bool ShouldNotify(double temperature, double? previousTemperature)
        {
            bool shouldNotify = !TemperatureIsInRange(temperature) &&
                   (!previousTemperature.HasValue || TemperatureIsInRange(previousTemperature.Value) || ShouldRemind());

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
            return !lastReminderTime.HasValue || (DateTime.UtcNow - lastReminderTime.Value) > TimeSpan.FromMinutes(MaximumTimeGapBetweenNotifications);
        }

        public string GenerateMessage(double temperature) => $"Temperature out of allowed range: {temperature}℃";
    }
}
