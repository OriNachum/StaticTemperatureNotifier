using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThermalNotifierWS.Service.NotifyTemperatureConditionProviders
{
    public class NotifyOnBreachingAllowedRange : INotifyTemperatureProvider
    {
        private readonly double _minTemperature;
        private readonly double _maxTemperature;
        private DateTime? LastReminderTime;


        public NotifyOnBreachingAllowedRange(double minTemperature, double maxTemperature)
        {
            _minTemperature = minTemperature;
            _maxTemperature = maxTemperature;
        }

        public bool ShouldNotify(double temperature, double? previousTemperature)
        {
            bool shouldNotify = !TemperatureIsInRange(temperature) &&
                   (!previousTemperature.HasValue || TemperatureIsInRange(previousTemperature.Value) || ShouldRemind());

            if (shouldNotify)
            {
                LastReminderTime = DateTime.UtcNow;
            }

            return shouldNotify;
        }

        private bool TemperatureIsInRange(double temperature)
        {
            return temperature >= _minTemperature && temperature <= _maxTemperature;
        }

        private bool ShouldRemind()
        {
            return !LastReminderTime.HasValue || (DateTime.UtcNow - LastReminderTime.Value) > TimeSpan.FromHours(1);
        }
        public string GenerateMessage(double temperature) => $"Temperature out of allowed range: {temperature}℃";
    }
}
