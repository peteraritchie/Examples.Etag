using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Examples.Etag.WebApi.Common.DomainEssentials;
using Examples.Etag.WebApi.Common.Exceptions;

namespace Examples.Etag.WebApi;

public class ProblemDetailsExceptionFilterAttribute : ExceptionFilterAttribute
{
	public override void OnException(ExceptionContext context)
	{
		switch (context.Exception)
		{
			case OperationCanceledException:
				context.Result = new ObjectResult(new ProblemDetails
				{
					Title = "Client closed request.",
					Status = 499,
					Type = "https://httpstatus.in/499/"
				});
				break;
			case ConcurrencyException:
				context.Result = new ObjectResult(new ProblemDetails
				{
					Title = "Precondition Failed.",
					Status = StatusCodes.Status412PreconditionFailed,
					Type = "https://www.rfc-editor.org/rfc/rfc9110.html#status.412"
				});
				break;
			case ArgumentException argumentException:
				context.Result = new ObjectResult(new ProblemDetails
				{
					Title = "Bad request",
					Detail = argumentException.Message,
					Status = StatusCodes.Status400BadRequest,
					Type = "https://www.rfc-editor.org/rfc/rfc9110.html#status.400"
				});
				break;
			case System.ComponentModel.DataAnnotations.ValidationException validationException:
				context.Result = new ObjectResult(new ProblemDetails
				{
					Title = "Bad request",
					Detail = validationException.Message,
					Status = StatusCodes.Status400BadRequest,
					Type = "https://www.rfc-editor.org/rfc/rfc9110.html#status.400"
				});
				break;
			case EntityNotFoundException:
				context.Result = new NotFoundObjectResult(new ProblemDetails
				{
					Title = "Requested item is not found",
					Status = StatusCodes.Status404NotFound,
					Type = "https://www.rfc-editor.org/rfc/rfc9110.html#status.404"
				});
				break;
			case JsonPatchException jsonPatchException:
				context.Result = new ObjectResult(new ProblemDetails
				{
					Title = "Malformed patch document",
					Detail = jsonPatchException.Message,
					Status = StatusCodes.Status400BadRequest,
					Type = "https://www.rfc-editor.org/rfc/rfc9110.html#status.400"
				});
				break;
			default:
				base.OnException(context);
				break;
		}
	}
}
// asp.net domain essentials
