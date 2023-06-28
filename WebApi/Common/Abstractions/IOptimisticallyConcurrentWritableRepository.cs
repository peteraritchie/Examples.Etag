using Examples.Etag.WebApi.Common.DomainEssentials;
using Examples.Etag.WebApi.Common.Exceptions;

namespace Examples.Etag.WebApi.Common.Abstractions;

public interface IOptimisticallyConcurrentWritableRepository<in TDomainEntity> : IWritableRepository<TDomainEntity>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="id"></param>
	/// <param name="token"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="ConcurrencyException"></exception>
	/// <exception cref="EntityNotFoundException"></exception>
	Task RemoveIfMatch(Guid id, string token, CancellationToken cancellationToken = default);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="id"></param>
	/// <param name="entity"></param>
	/// <param name="token"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="ConcurrencyException"></exception>
	/// <exception cref="EntityNotFoundException"></exception>
	Task ReplaceIfMatch(Guid id, TDomainEntity entity, string token, CancellationToken cancellationToken = default);
}
