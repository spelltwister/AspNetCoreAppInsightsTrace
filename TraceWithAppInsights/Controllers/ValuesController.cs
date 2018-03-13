using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceLib;

namespace TraceWithAppInsights.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static ServiceClass Service { get; } = new ServiceClass();

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await Service.RunAvailabilityTestsAsync().ConfigureAwait(false);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}