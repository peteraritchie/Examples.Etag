using Riok.Mapperly.Abstractions;
using Examples.Etag.WebApi.Common.Abstractions;
using Examples.Etag.WebApi.Domain;

namespace Examples.Etag.WebApi.Infrastructure;

[Mapper]
public partial class AppointmentRequestEntityTranslator : ITranslator<AppointmentRequest, AppointmentRequestEntity>
{
	[MapperIgnoreSource(nameof(AppointmentRequestEntity.Id))]
	[MapperIgnoreSource(nameof(AppointmentRequestEntity.ResourceId))]
	[MapperIgnoreSource(nameof(AppointmentRequestEntity.ETag))]
	[MapperIgnoreSource(nameof(AppointmentRequestEntity.SelfUri))]
	[MapperIgnoreSource(nameof(AppointmentRequestEntity.TimestampText))]
	public partial AppointmentRequest ToDomain(AppointmentRequestEntity data);

	[MapperIgnoreTarget(nameof(AppointmentRequestEntity.Id))]
	[MapperIgnoreTarget(nameof(AppointmentRequestEntity.ResourceId))]
	[MapperIgnoreTarget(nameof(AppointmentRequestEntity.ETag))]
	[MapperIgnoreTarget(nameof(AppointmentRequestEntity.SelfUri))]
	[MapperIgnoreTarget(nameof(AppointmentRequestEntity.TimestampText))]
	public partial AppointmentRequestEntity ToData(AppointmentRequest domain);
}
