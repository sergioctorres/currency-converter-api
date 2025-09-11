using Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Authorize(PolicyConstants.RequireUser)]
public abstract class ApiControllerBase : ControllerBase
{
}
