using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AvatarService.Controllers;

public class AvatarController : ControllerBase
{
    private readonly IMediator _mediator;
    public AvatarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ping")]
    public Task<string> Ping() => _mediator.Send(new Ping());

    [HttpPost("generate")]
    public Task<GenerateAvatarResult> GenerateAvatar([FromBody] GenerateAvatar request) => _mediator.Send(request);

    [HttpPost("delete")]
    public Task<DeleteAvatarResult> DeleteAvatar([FromBody] DeleteAvatar request) => _mediator.Send(request);

    [HttpPost("upload")]
    public Task<UploadAvatarResult> UploadAvatar([FromForm] UploadAvatar request) => _mediator.Send(request);
}
