using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarHub.Exceptions
{
    public class VehicleNotFoundException : Exception
    {
        public int VehicleId { get; init; }
    }
}
