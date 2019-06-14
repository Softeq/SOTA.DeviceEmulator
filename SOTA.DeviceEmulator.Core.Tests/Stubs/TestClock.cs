using System;
using EnsureThat;

namespace SOTA.DeviceEmulator.Core.Tests.Stubs
{
    public class TestClock : IClock
    {
        public TestClock()
        {
            UtcNow = DateTime.UtcNow;
        }

        public DateTime UtcNow { get; private set; }

        public DateTime MoveForward(TimeSpan time)
        {
            Ensure.Comparable.IsGt(time, TimeSpan.Zero, nameof(time));

            UtcNow += time;
            return UtcNow;
        }
    }
}
