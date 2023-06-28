using System.Net.Mime;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

using Examples.Etag.WebApi.Application.Services;
using Examples.Etag.WebApi.Common.Web;

namespace Examples.Etag.WebApi.Controllers;

/// <summary>
/// Appointment request controller
/// </summary>
[ApiController] [Route("[controller]")] [ProblemDetailsExceptionFilter]
public class AppointmentController : ControllerBase
{
	private readonly AppointmentRequestService appointmentRequestService;

	/// <summary>
	/// Initialize a new controller with an <paramref name="appointmentRequestService"/> object
	/// </summary>
	/// <param name="appointmentRequestService"></param>
	public AppointmentController(AppointmentRequestService appointmentRequestService)
	{
		this.appointmentRequestService = appointmentRequestService;
	}

	/// <summary>
	/// Get all appointment requests
	/// </summary>
	/// <returns>A collection of appointment requests</returns>
	[HttpGet(Name = "GetAppointmentRequests")]
	[ProducesResponseType(typeof(WebCollectionElement<AppointmentRequestDto>[]), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json"/*MediaTypeNames.Application.ProblemJson*/)]
	public async Task<IActionResult> GetMany(CancellationToken cancellationToken = default)
	{
		var resource = appointmentRequestService.GetRequests(cancellationToken);

		List<WebCollectionElement<AppointmentRequestDto>> items = new();
		foreach (var (dto, guid, concurrencyToken) in await resource.ToListAsync(cancellationToken: cancellationToken))
		{
			items.Add(
				new WebCollectionElement<AppointmentRequestDto>(dto, Url.Action(nameof(GetById), new { id = guid })!, etag: concurrencyToken));
		}

		return base.Ok(items);
	}

	/// <summary>
	/// Get a specific appointment request
	/// </summary>
	/// <param name="ifNoneMatch">The etag for concurrency control</param>
	/// <param name="id">the identity of the appointment to retrieve</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>The appointment request</returns>
	[HttpGet("{id}", Name = "GetAppointmentRequest")]
	[ProducesResponseType(typeof(AppointmentRequestDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(AppointmentRequestDto), StatusCodes.Status304NotModified)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json"/*MediaTypeNames.Application.ProblemJson*/)]
	public async Task<IActionResult> GetById(Guid id, [FromHeader(Name = "If-None-Match")] string? ifNoneMatch,
		CancellationToken cancellationToken = default)
	{
		var (resource, concurrencyToken) = string.IsNullOrWhiteSpace(ifNoneMatch) 
			? await appointmentRequestService.GetRequest(id, cancellationToken) 
			: await appointmentRequestService.GetRequest(id, ifNoneMatch, cancellationToken);

		HttpContext.Response.Headers.Add(HeaderNames.ETag, concurrencyToken);
		// TODO: 304?
		return Ok(resource);
	}

	/// <summary>
	/// Add a new appointment request
	/// </summary>
	/// <param name="appointmentRequest">The new appointment to add</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>Empty</returns>
	[HttpPost(Name = "CreateAppointmentRequest")]
	[Consumes(MediaTypeNames.Application.Json)]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Create([FromBody] AppointmentRequestDto appointmentRequest,
		CancellationToken cancellationToken = default)
	{
		var (id, concurrencyToken) = await appointmentRequestService.CreateRequest(appointmentRequest, cancellationToken);

		HttpContext.Response.Headers.Add(HeaderNames.ETag, concurrencyToken);

		return CreatedAtAction(nameof(GetById), routeValues: new { id }, value: null);
	}

	/// <summary>
	/// Replaces an appointment request
	/// </summary>
	/// <param name="ifMatch">The etag for concurrency control</param>
	/// <param name="id">The identity of the appointment to replace</param>
	/// <param name="appointmentRequest">The replacement appointment</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>No content</returns>
	[HttpPut("{id}", Name = "ReplaceAppointmentRequest")]
	[Consumes(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json"/*MediaTypeNames.Application.ProblemJson*/)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed, "application/problem+json"/*MediaTypeNames.Application.ProblemJson*/)]
	public async Task<IActionResult> Replace(Guid id, [FromBody] AppointmentRequestDto appointmentRequest,
		[FromHeader(Name = "If-Match")] string? ifMatch, CancellationToken cancellationToken = default)
	{
		var concurrencyToken = string.IsNullOrWhiteSpace(ifMatch)
			? await appointmentRequestService.ReplaceRequest(
				id, appointmentRequest, cancellationToken)
			: await appointmentRequestService.ReplaceRequest(
				id, appointmentRequest, ifMatch, cancellationToken);

		HttpContext.Response.Headers.Add(HeaderNames.ETag, concurrencyToken);

		return NoContent();
	}

	/// <summary>
	/// Update an Appointment Request
	/// </summary>
	/// <param name="id" example="ba7026f4-61bc-487e-b0fb-14cb624c7444">The ID of the appointment request</param>
	/// <param name="patchDocument">A JSON-Patch document.</param>
	/// <param name="ifMatch">The etag for concurrency control</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>Updated appointment request</returns>
	[HttpPatch("{id:guid}", Name = "UpdateAppointmentRequest")]
	[ProducesResponseType(typeof(AppointmentRequestDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json"/*MediaTypeNames.Application.ProblemJson*/)]
	[Consumes("application/json-patch+json")]
	public async Task<IActionResult> Update(Guid id, JsonPatchDocument<AppointmentRequestDto> patchDocument,
		[FromHeader(Name = "If-Match")] string? ifMatch, CancellationToken cancellationToken = default)
	{
		var (result, concurrencyToken) = string.IsNullOrWhiteSpace(ifMatch)
			? await appointmentRequestService.UpdateRequest(id, patchDocument, cancellationToken)
			: await appointmentRequestService.UpdateRequest(id, patchDocument, ifMatch, cancellationToken);

		HttpContext.Response.Headers.Add(HeaderNames.ETag, concurrencyToken);

		return Ok(result);
	}

	/// <summary>
	/// Remove an appointment request
	/// </summary>
	/// <param name="id">The identity of the appointment to remove</param>
	/// <param name="ifMatch">The etag for concurrency control</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>No content</returns>
	[HttpDelete("{id}", Name = "RemoveAppointmentRequest")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Remove(Guid id, [FromHeader(Name = "If-Match")] string? ifMatch,
		CancellationToken cancellationToken = default)
	{
		if(string.IsNullOrWhiteSpace(ifMatch))
			await appointmentRequestService.RemoveRequest(id, cancellationToken);
		else
			await appointmentRequestService.RemoveRequest(id, ifMatch, cancellationToken);

		return NoContent();
	}
}
