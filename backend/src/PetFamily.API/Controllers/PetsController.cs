using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts;
using PetFamily.API.Extensions;
using PetFamily.Application.modules.GetPets;
using PetFamily.Domain.Shared;
using PetFamily.Infrastructure.Read;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("pets")]
public class PetsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] GetPetsRequest request,
        [FromServices] IPetsReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        var result = await readRepository.GetAll(request, cancellationToken);

        return Ok(Envelope.Ok(result));
    }

    [HttpGet("{petId:guid}")]
    public async Task<ActionResult> GetById(
        Guid petId,
        [FromServices] IPetsReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        var result = await readRepository.GetById(petId, cancellationToken);

        if (result is null)
            return Error.NotFound("pet.not.found", "Pet was not found.").ToActionResult();

        return Ok(Envelope.Ok(result));
    }
}
