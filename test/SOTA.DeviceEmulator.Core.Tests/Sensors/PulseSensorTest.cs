﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Sensors
{
    public class PulseSensorTest
    {
        private static DateTime _measuringTime;
        private const double ProcedureResult = 70;
        private static Mock<ITimeFunction<double>> _procedureMock;
        private static PulseSensor _pulseSensor;

        public static IEnumerable<object[]> GenerateData()
        {
            _measuringTime = new DateTime(2019, 10, 10);
            _procedureMock = new Mock<ITimeFunction<double>>(MockBehavior.Strict);
            _procedureMock.Setup(i => i.GetValue(_measuringTime)).Returns(ProcedureResult);

            _pulseSensor = new PulseSensor(_procedureMock.Object);

            var noiseFactors = new[] { 1, 2, 3, 4, 5, 10, 20, 30 };
            var testData = noiseFactors.Select(factor =>
            {
                _pulseSensor.NoiseFactor = factor;
                return new object[]
                {
                    factor,
                    Enumerable.Range(0, 20).Select(i => _pulseSensor.GetValue(_measuringTime)).ToList()
                };
            });

            return testData;
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void Returns_ValidRandomValue_When_RangePassed(int noiseFactor, IEnumerable<int> pulses)
        {
            foreach (var pulse in pulses)
            {
                pulse.Should().BeInRange((int)ProcedureResult - noiseFactor, (int)ProcedureResult + noiseFactor);
            }
        }
    }
}