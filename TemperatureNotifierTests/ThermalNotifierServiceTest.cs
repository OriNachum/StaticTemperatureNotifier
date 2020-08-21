using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SlackNotifierWS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TemperatureNotifierTests.Framework;
using ThermalNotifierWS.Service;
using ThermometerWS.Service;
using Xunit;

namespace TemperatureNotifierTests
{
    public class ThermalNotifierServiceTest
    {
        private readonly ILogger _logger;
        private readonly IThermometerService _thermometerService;
        private readonly ISlackNotifierService _slackNotifierService;
        private readonly IConfiguration _configuration;
        private readonly ThermalNotifierService _thermalNotifierService;

        public ThermalNotifierServiceTest()
        {
            _logger = A.Fake<ILogger>();
            _thermometerService = A.Fake<IThermometerService>();
            _slackNotifierService = A.Fake<ISlackNotifierService>();
            _configuration = A.Fake<IConfiguration>();
            _thermalNotifierService = new ThermalNotifierService(_thermometerService, _slackNotifierService, _configuration, _logger);
            ThermalNotifierServiceTemperatureHistory.LastKnownTemperature = null;
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 2, false)]
        [InlineData(2, 2, true)]
        public async Task NotifyTemperature_OkResponses_CallsAtleastOneMoreAndTrueOnSuccess(int numberOfOKResponses, int numberOfCalls, bool result)
        {
            //foreach (int index in Enumerable.Range(0, numberOfOKResponses))
            //{
            //    _mockHttpMessageHandler.EnqueueNextResponse("32.45", HttpStatusCode.OK);
            //}
            //foreach (int index in Enumerable.Range(numberOfOKResponses, 2))
            //{
            //    _mockHttpMessageHandler.EnqueueNextResponse("32.45", HttpStatusCode.NotFound);
            //}
            bool actualResult = await _thermalNotifierService.NotifyTemperatureAsync();
            //Assert.Equal(numberOfCalls, _mockHttpMessageHandler.NumberOfCalls);
            Assert.Equal(result, actualResult);
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 2, false)]
        public async Task AlertTemperature_FailFlow_CallsAtleastOneMoreAndTrueOnSuccess(int numberOfOKResponses, int numberOfCalls, bool result)
        {
            //foreach (int index in Enumerable.Range(0, numberOfOKResponses))
            //{
            //    _mockHttpMessageHandler.EnqueueNextResponse("32.45", HttpStatusCode.OK);
            //}
            //foreach (int index in Enumerable.Range(numberOfOKResponses, 2))
            //{
            //    _mockHttpMessageHandler.EnqueueNextResponse("32.45", HttpStatusCode.NotFound);
            //}
            bool actualResult = await _thermalNotifierService.AlertTemperatureAsync();
            //Assert.Equal(numberOfCalls, _mockHttpMessageHandler.NumberOfCalls);
            Assert.Equal(result, actualResult);
        }

        [Theory]
        [InlineData(25, 26, false)]
        [InlineData(25, 32, true)]
        [InlineData(32, 25, true)]
        [InlineData(32, 33, false)]
        [InlineData(25, 10, true)]
        [InlineData(10, 25, true)]
        [InlineData(10, 12, false)]
        public async Task AlertTemperature_SuccessFlow_NotifiesSlackIfNeeded(double initialTemperature, double newTemperature, bool alertSent)
        {
            //_mockHttpMessageHandler.EnqueueNextResponse(initialTemperature.ToString(), HttpStatusCode.OK);
            //_mockHttpMessageHandler.EnqueueNextResponse("Success", HttpStatusCode.OK);
            //await _thermalNotifierService.NotifyTemperatureAsync();
            //_mockHttpMessageHandler.EnqueueNextResponse(newTemperature.ToString(), HttpStatusCode.OK);
            //_mockHttpMessageHandler.EnqueueNextResponse("Success", HttpStatusCode.OK);
            await _thermalNotifierService.AlertTemperatureAsync();
            if (alertSent)
            {
                //Assert.Equal(4, _mockHttpMessageHandler.NumberOfCalls);
            }
            else
            {
                //Assert.Equal(3, _mockHttpMessageHandler.NumberOfCalls);
            }
        }
    }
}
