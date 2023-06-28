namespace Examples.Etag.WebApi.Common.DomainEssentials;

/// <summary>
/// An exception class to signify to the application-level that an entity is not available in some context.
/// </summary>
public class EntityNotFoundException : EntityNotFoundExceptionBase<Guid>
{
	/// <summary>
	/// An entity ID is required if it cannot be found
	/// </summary>
	/// <param name="id"></param>
	public EntityNotFoundException(Guid id) : base(id)
	{
	}
}
