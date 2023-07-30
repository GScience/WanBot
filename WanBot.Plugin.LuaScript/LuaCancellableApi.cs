using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.LuaScript
{
    internal class LuaCancellableApi
    {
        private CancellationToken _ct;
        public LuaCancellableApi(CancellationToken ct)
        {
            _ct = ct;
        }

        public string LuaHttpGet(string url)
        {
            try
            {
                return InnerHttpGet(url);
            }
            catch (Exception e)
            {
                if (e.InnerException is TaskCanceledException)
                    throw new TimeoutException($"Timeout while get {url}");
                throw;
            }
        }

        private string InnerHttpGet(string url)
        {
            var uri = new Uri(url);
            var scheme = uri.Scheme.ToLower();
            if (scheme != "http" && scheme != "https")
                throw new ArgumentException("Only allow http or https");
            if (!uri.IsDefaultPort)
                throw new ArgumentException("Only allow default port");
            if (uri.HostNameType == UriHostNameType.IPv4)
            {
                ThrowIfIpIntranet(IPAddress.Parse(uri.Host));
            }
            else if (uri.HostNameType == UriHostNameType.Dns)
            {
                var hostInfo = Dns.GetHostAddressesAsync(uri.DnsSafeHost, _ct).Result;
                foreach (var ip in hostInfo)
                    ThrowIfIpIntranet(ip);
            }
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.None
            };

            using var httpClient = new HttpClient(httpClientHandler);
            var result = httpClient
                .GetAsync(uri, _ct).Result
                .EnsureSuccessStatusCode().Content
                .ReadAsStringAsync(_ct).Result;
            return result;
        }

        private void ThrowIfIpIntranet(IPAddress ip)
        {
            var addrBytes = ip.GetAddressBytes();
            if ((addrBytes[0] == 10) ||
                (addrBytes[0] == 172 && addrBytes[1] >= 16 && addrBytes[1] <= 31) ||
                (addrBytes[0] == 192 && addrBytes[1] == 168) ||
                (addrBytes[0] == 127))
                throw new ArgumentException("Intranet is not allowed");
        }
    }
}
