using Examples.Etag.WebApi.Common.Abstractions;

namespace Examples.Etag.WebApi.Domain.Abstractions;

public interface IOptimisticallyConcurrentRepository<TDomainEntity>
	: IOptimisticallyConcurrentReadableRepository<TDomainEntity>, IOptimisticallyConcurrentWritableRepository<TDomainEntity>
{
}
