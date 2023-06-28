namespace Examples.Etag.WebApi.Domain;

/// <summary>
/// An appointment request abstraction
/// </summary>
public class AppointmentRequest
{
	public AppointmentRequest(DateTime creationDate, string description, string notes, AppointmentRequestStatus status, MeetingDuration duration, IEnumerable<DateTime> proposedStartDateTimes)
	{
		CreationDate = creationDate;
		Description = description;
		Notes = notes;
		Status = status;
		Duration = duration;
		ProposedStartDateTimes = proposedStartDateTimes;
	}

	public AppointmentRequest(string description, string notes, AppointmentRequestStatus status, MeetingDuration duration, IEnumerable<DateTime> proposedStartDateTimes)
	{
		Description = description;
		Notes = notes;
		Status = status;
		Duration = duration;
		ProposedStartDateTimes = proposedStartDateTimes;
	}

	public AppointmentRequest(string description, string notes, MeetingDuration duration, IEnumerable<DateTime> proposedStartDateTimes)
		: this(description, notes, AppointmentRequestStatus.Proposed, duration, proposedStartDateTimes)
	{
	}

	public AppointmentRequest(string description, string notes, IEnumerable<DateTime> proposedStartDateTimes)
		: this(description, notes, AppointmentRequestStatus.Proposed, MeetingDuration.Hour, proposedStartDateTimes)
	{
	}

	/// <summary>The date the request was first created</summary>
	/// <example>2023-06-07T17:49:12.9565268Z</example>
	public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
	public IEnumerable<string> Categories { get; } = new List<string>();

	/// <summary>The description of the request</summary>
	/// <example>New Description</example>
	public string Description { get; private set; }
	public string Notes { get; private set;  }
	public AppointmentRequestStatus Status { get; private set; }
	public MeetingDuration Duration { get; private set; }
	public IEnumerable<string> Participants { get; private set; } = new List<string>();
	public IEnumerable<DateTime> ProposedStartDateTimes { get; private set; }
}
