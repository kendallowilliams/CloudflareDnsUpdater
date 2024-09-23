using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudflareDnsUpdater.Exceptions
{
    public class CloudflareException : Exception
    {
        public CloudflareException(string message) : base(message) { }
    }
}
