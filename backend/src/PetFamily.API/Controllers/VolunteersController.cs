using Microsoft.AspNetCore.Mvc;
using PetFamily.Application.modules.CreateVolunteer;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromServices] CreateVolunteerHandler service,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.Execute(request, cancellationToken);
        
        // Отрефакторить метод для создания волонтера, чтобы он возвращал Result. В контроллере в зависимости от результата вернуть либо BadRequest, либо Ok.
    }
}