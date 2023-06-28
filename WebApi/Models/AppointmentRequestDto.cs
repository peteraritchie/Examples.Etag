using System.ComponentModel.DataAnnotations;

using Examples.Etag.WebApi.Domain;

namespace Examples.Etag.WebApi;

/// <summary>
/// A request WebAPI DTO
/// </summary>
public class AppointmentRequestDto
{
	/// <summary>The date the request was first created</summary>
	/// <example>2023-06-07T17:49:12.9565268Z</example>
	[Required]
	public DateTime? CreationDate { get; set; }

	public IEnumerable<string>? Categories { get; set; }

	/// <summary>The description of the request</summary>
	/// <example>New Description</example>
	[Required]
	public string? Description { get; set; }

	public string? Notes { get; set; }

	[Required]
	public AppointmentRequestStatus? Status { get; set; }

	[Required]
	public MeetingDuration? Duration { get; set; }

	[Required]
	public IEnumerable<string>? Participants { get; set; }

	[Required]
	public IEnumerable<DateTime>? ProposedStartDateTimes { get; set; }
}
