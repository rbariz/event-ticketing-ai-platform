using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Domain.Enums
{
    public enum TicketStatus
    {
        Active = 1,
        Consumed = 2,
        Expired = 3,
        Cancelled = 4
    }
}
