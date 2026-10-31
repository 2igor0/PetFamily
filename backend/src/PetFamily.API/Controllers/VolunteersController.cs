using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts;
using PetFamily.API.Extensions;
using PetFamily.Application.modules.CreateVolunteer;
using PetFamily.Application.modules.DeleteVolunteer;
using PetFamily.Application.modules.GetVolunteers;
using PetFamily.Application.modules.UpdateVolunteerDonationDetails;
using PetFamily.Application.modules.UpdateVolunteerMainInfo;
using PetFamily.Application.modules.UpdateVolunteerSocialNetworks;
using PetFamily.Domain.Shared;
using PetFamily.Infrastructure.Read;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("volunteers")]
public class VolunteersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(request, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] GetVolunteersRequest request,
        [FromServices] IVolunteersReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        var result = await readRepository.GetAll(request, cancellationToken);

        return Ok(Envelope.Ok(result));
    }

    [HttpGet("{volunteerId:guid}")]
    public async Task<ActionResult> GetById(
        Guid volunteerId,
        [FromServices] IVolunteersReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        var result = await readRepository.GetById(volunteerId, cancellationToken);

        if (result is null)
            return Error.NotFound("volunteer.not.found", "Volunteer was not found.")
                .ToActionResult();

        return Ok(Envelope.Ok(result));
    }

    [HttpPut("{volunteerId:guid}/main-info")]
    public async Task<ActionResult> UpdateMainInfo(
        Guid volunteerId,
        [FromBody] UpdateVolunteerMainInfoRequest request,
        [FromServices] UpdateVolunteerMainInfoHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(volunteerId, request, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{volunteerId:guid}/social-networks")]
    public async Task<ActionResult> UpdateSocialNetworks(
        Guid volunteerId,
        [FromBody] UpdateVolunteerSocialNetworksRequest request,
        [FromServices] UpdateVolunteerSocialNetworksHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(volunteerId, request, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{volunteerId:guid}/donation-details")]
    public async Task<ActionResult> UpdateDonationDetails(
        Guid volunteerId,
        [FromBody] UpdateVolunteerDonationDetailsRequest request,
        [FromServices] UpdateVolunteerDonationDetailsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(volunteerId, request, cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{volunteerId:guid}")]
    public async Task<ActionResult> Delete(
        Guid volunteerId,
        [FromServices] DeleteVolunteerHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(volunteerId, cancellationToken);

        return result.ToActionResult();
    }
}
