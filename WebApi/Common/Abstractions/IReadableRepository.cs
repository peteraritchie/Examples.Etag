using Examples.Etag.WebApi.Common.DomainEssentials;

namespace Examples.Etag.WebApi.Common.Abstractions;

public interface IReadableRepository<TDomainEntity>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="EntityNotFoundException"></exception>
	Task<TDomainEntity> Get(Guid id, CancellationToken cancellationToken = default);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	IAsyncEnumerable<TDomainEntity> Get(CancellationToken cancellationToken = default);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="entity">The entity whose ID is to be retrieved.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	Guid GetId(TDomainEntity entity);
}
