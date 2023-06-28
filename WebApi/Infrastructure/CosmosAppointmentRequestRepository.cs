using Microsoft.Azure.Cosmos;
using Examples.Etag.WebApi.Common.Abstractions;
using Examples.Etag.WebApi.Domain;

namespace Examples.Etag.WebApi.Infrastructure;

/// <summary>
/// A Cosmos DB repository abstraction for <seealso cref="AppointmentRequest"/> and <seealso cref="AppointmentRequestEntity"/>
/// </summary>
public sealed class CosmosAppointmentRequestRepository : CosmosOptimisticallyConcurrentRepository<AppointmentRequest, AppointmentRequestEntity>
{
	public CosmosAppointmentRequestRepository(Container container, ITranslator<AppointmentRequest, AppointmentRequestEntity> translator)
		: base(container, translator, (entity, guid) => entity.Id = guid)
	{
	}
}
