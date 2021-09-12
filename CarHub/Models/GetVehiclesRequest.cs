using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarHub.Models
{
    public record GetVehiclesRequest
    {
        public int? Make { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public bool IsUserAuthenticated { get; init; }
    }
}
