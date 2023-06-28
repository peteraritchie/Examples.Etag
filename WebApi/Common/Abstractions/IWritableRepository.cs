using Examples.Etag.WebApi.Common.DomainEssentials;
using Examples.Etag.WebApi.Common.Exceptions;

namespace Examples.Etag.WebApi.Common.Abstractions;

public interface IWritableRepository<in TDomainEntity>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="ConcurrencyException"></exception>
	Task<Guid> Add(TDomainEntity entity, CancellationToken cancellationToken = default);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="EntityNotFoundException"></exception>
	Task Remove(Guid id, CancellationToken cancellationToken = default);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="id"></param>
	/// <param name="entity"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="EntityNotFoundException"></exception>
	Task Replace(Guid id, TDomainEntity entity, CancellationToken cancellationToken = default);
}
