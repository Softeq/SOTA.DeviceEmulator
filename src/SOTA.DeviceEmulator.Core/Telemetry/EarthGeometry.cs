using System;
using GeoAPI.Geometries;
using NetTopologySuite;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public static class EarthGeometry
    {
        // World Geodetic System 1984 Code
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once InconsistentNaming
        private const int WGS84_EPSG = 4326;
        public static readonly IGeometryFactory GeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: WGS84_EPSG);

        // Earth radius in kilometers at equator.
        public static double EarthRadius = 6378;

        // Converts distance from kilometers to degrees;
        public static double ConvertDistanceToAngleSize(double distance)
        {
            return distance * 180 / (Math.PI * EarthRadius);
        }

        // Converts distance from kilometers to degrees;
        public static double ConvertAngleSizeToDistance(double angleSize)
        {
            return angleSize * Math.PI * EarthRadius / 180;
        }
    }
}
