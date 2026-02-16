using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace ZenBlog.Presentation.Controllers.Abstraction;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class APIController : ControllerBase
{
    public readonly IMessageBus _bus;

    protected APIController(IMessageBus bus)
    {
        _bus = bus;
    }
}
