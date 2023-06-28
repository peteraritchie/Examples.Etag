using Riok.Mapperly.Abstractions;
using Examples.Etag.WebApi.Domain;

namespace Examples.Etag.WebApi.Application.Translators;

[Mapper]
public partial class AppointmentRequestDtoTranslator
{
	public partial AppointmentRequest AppointmentRequestDtoToAppointmentRequest(AppointmentRequestDto dto);
	public partial AppointmentRequestDto AppointmentRequestToAppointmentRequestDto(AppointmentRequest entity);
}
