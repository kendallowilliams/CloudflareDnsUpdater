using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudflareDnsUpdater
{
    public static class Enums
    {
        public enum DnsRecordTypes
        {
            A,
            AAAA,
            CAA,
            CERT,
            CNAME,
            DNSKEY,
            DS,
            HTTPS,
            LOC,
            MX,
            NAPTR,
            NS,
            OPENPGPKEY,
            PTR,
            SMIMEA,
            SRV,
            SSHFP,
            SVCB,
            TLSA,
            TXT,
            URI
        }
    }
}
