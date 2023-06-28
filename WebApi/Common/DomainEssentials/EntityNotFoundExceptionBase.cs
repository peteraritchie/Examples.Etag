namespace Examples.Etag.WebApi.Common.DomainEssentials;

public class EntityNotFoundExceptionBase<T>
	: Exception
{
	protected EntityNotFoundExceptionBase(T id)
	{
		Id = id;
	}

	/// <summary>
	/// The ID of the entity that is not available.
	/// </summary>
	public T Id { get; protected set; }
}
