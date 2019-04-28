using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Multipart.XMixedReplace.Resources;

namespace Multipart.XMixedReplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(CancellationToken cancellationToken)
        {
            return new MultipartResult(1, cancellationToken);
        }
    }
}
