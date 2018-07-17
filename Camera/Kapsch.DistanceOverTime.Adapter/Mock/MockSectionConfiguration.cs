using System;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Enums;
using Kapsch.RTE.Gateway.Models.Configuration.Dot;

namespace Kapsch.DistanceOverTime.Adapter.Mock
{
    public static class MockSectionConfiguration
    {
        public static SectionConfigurationModel GetMockSectionConfiguration(string sectionCodePointA, string sectionCodePointB)
        {
            SectionConfigurationModel config = new SectionConfigurationModel
            {
                SectionCode = sectionCodePointA + "_" + sectionCodePointB,
                CreatePhysicalInfringement = false,
                LevenshteinMatchDistance = 1,
                SectionDescription = "Mock Section",
                SectionDistanceInMeter = 10000,
            };

            return config;
        }
    }
}
