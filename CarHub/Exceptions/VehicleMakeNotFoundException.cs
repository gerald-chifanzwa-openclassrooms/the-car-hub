using System;

namespace CarHub.Exceptions
{
    public class VehicleMakeNotFoundException : Exception
    {
        public int MakeId { get; init; }
    }
}
