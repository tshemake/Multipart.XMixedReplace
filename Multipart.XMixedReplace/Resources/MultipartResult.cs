using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multipart.XMixedReplace.Resources
{
    public class MultipartResult : IActionResult
    {
        private static readonly string CRLF = "\r\n";
        private readonly int _delay;
        private readonly CancellationToken _cancellationToken;

        private readonly string _boundar = Guid.NewGuid().ToString();
        public string Boundar { get => "--" + _boundar; }

        public MultipartResult(int dalay, CancellationToken cancellationToken)
        {
            _delay = dalay;
            _cancellationToken = cancellationToken;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 200;
            var headers = response.GetTypedHeaders();
            headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true
            };
            var contentType = new MediaTypeHeaderValue("multipart/x-mixed-replace") { Boundary = _boundar };
            headers.ContentType = contentType;

            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var data = GetText();
                    StringBuilder sb = new StringBuilder(CRLF);
                    sb.Append(Boundar);
                    sb.Append(CRLF);
                    sb.Append("Content-Type: image/svg+xml; charset=utf-8");
                    sb.Append(CRLF);
                    sb.AppendFormat("Content-Length: {0}", data.Length);
                    sb.Append(CRLF);
                    sb.Append(CRLF);
                    byte[] partHeaders = Encoding.UTF8.GetBytes(sb.ToString());
                    await response.Body.WriteAsync(partHeaders, 0, partHeaders.Length);
                    await response.Body.WriteAsync(data, 0, data.Length, _cancellationToken);
                    await response.Body.FlushAsync(_cancellationToken);
                    await Task.Delay(TimeSpan.FromSeconds(_delay), _cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception)
            {
            }
        }

        public static byte[] GetText()
        {
            var str = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg width=""140"" height=""22"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"">
    <rect x=""0"" y=""0"" width=""140"" height=""20"" style=""fill: #fff"" />
    <text x=""0"" y=""15"" fill=""#000"" xml:space=""preserve""><tspan x=""0"" y=""15"" font-family=""Arial"" font-size=""15px"" font-style=""normal"" style=""line-height:22px"">";
            str += DateTime.Now;
            str += @"</tspan></text>
</svg>";
            return Encoding.ASCII.GetBytes(str);
        }
    }
}
