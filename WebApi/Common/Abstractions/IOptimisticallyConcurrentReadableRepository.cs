using Examples.Etag.WebApi.Common.DomainEssentials;

namespace Examples.Etag.WebApi.Common.Abstractions;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TDomainEntity">The domain entity type that the repository contains</typeparam>
public interface IOptimisticallyConcurrentReadableRepository<TDomainEntity> : IReadableRepository<TDomainEntity>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="id"></param>
	/// <param name="concurrencyToken"></param>
	/// <param name="entity"></param>
	/// <returns>false if the entity has not been modified.</returns>
	/// <exception cref="EntityNotFoundException"></exception>
	bool TryGetIfModified(Guid id, string concurrencyToken, out TDomainEntity? entity);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="entity">The entity whose concurrency token is to be retrieved.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	string GetConcurrencyToken(TDomainEntity entity);
}
