using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class TestController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
}
