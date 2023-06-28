using Examples.Etag.WebApi.Domain;

namespace Examples.Etag.WebApi.Infrastructure;

/// <summary>
/// An appointment request abstraction for database serialization
/// </summary>
public class AppointmentRequestEntity : CosmosEntityBase
{
	/// <summary>The date the request was first created</summary>
	/// <example>2023-06-07T17:49:12.9565268Z</example>
	public DateTime? CreationDate { get;  set; }

	public IEnumerable<string>? Categories { get; set; }

	/// <summary>The description of the request</summary>
	/// <example>New Description</example>
	public string? Description { get;  set; }
	public string? Notes { get;  set;  }
	public AppointmentRequestStatus? Status { get;  set; }
	public MeetingDuration? Duration { get;  set; }
	public IEnumerable<string>? Participants { get; set; }
	public IEnumerable<DateTime>? ProposedStartDateTimes { get;  set; }
}
