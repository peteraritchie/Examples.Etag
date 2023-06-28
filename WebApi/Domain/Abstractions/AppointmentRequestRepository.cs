using Examples.Etag.WebApi.Common.DomainEssentials;

namespace Examples.Etag.WebApi.Domain.Abstractions;

public abstract class AppointmentRequestRepository
{
	/// <summary>
	/// Get an entity by id
	/// </summary>
	/// <param name="id">the identity of the entity to retrieve</param>
	/// <returns>The retrieved entity</returns>
	/// <exception cref="EntityNotFoundException"/>
	public abstract Task<AppointmentRequest> Get(Guid id);

	/// <summary>
	/// save an entity
	/// </summary>
	/// <param name="id">the identity of the entity to save</param>
	/// <param name="request">The entity to save</param>
	public abstract Task Save(Guid id, AppointmentRequest request);

	/// <summary>
	/// Remove an entity by id
	/// </summary>
	/// <param name="id">the identity of the entity to remove</param>
	/// <exception cref="EntityNotFoundException"/>
	public abstract Task Remove(Guid id);

	/// <summary>
	/// Get all entities
	/// </summary>
	/// <returns>A sequence of entities</returns>
	public abstract Task<IEnumerable<AppointmentRequest>> Get();
}
