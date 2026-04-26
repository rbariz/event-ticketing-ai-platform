using Android.Telephony.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Mobile.Scanner.Services
{
    public sealed class AppSettings
    {
        public ApiSettings Api { get; set; } = new();
    }

}
