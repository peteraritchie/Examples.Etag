using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.JsonPatch;
using Examples.Etag.WebApi.Application.Translators;
using Examples.Etag.WebApi.Common.Exceptions;
using Examples.Etag.WebApi.Domain;
using Examples.Etag.WebApi.Domain.Abstractions;

namespace Examples.Etag.WebApi.Application.Services;

/// <summary>
/// An <see href="https://badia-kharroubi.gitbooks.io/microservices-architecture/content/patterns/tactical-patterns/domain-application-infrastructure-services-pattern.html#Domain-Application-InfrastructureServicespattern-ApplicationServices">
/// application service</see> that abstracts the collaborations between the UI/API and the domain (repository).
/// Responsible for translating to/from web DTO/Model and domain object.
/// </summary>
public class AppointmentRequestService
{
	private readonly AppointmentRequestDtoTranslator appointmentRequestDtoTranslator;
	private readonly IOptimisticallyConcurrentRepository<AppointmentRequest> repository;

	public AppointmentRequestService(AppointmentRequestDtoTranslator appointmentRequestDtoTranslator, IOptimisticallyConcurrentRepository<AppointmentRequest> repository)
	{
		this.appointmentRequestDtoTranslator = appointmentRequestDtoTranslator;
		this.repository = repository;
	}

	public async Task<(Guid, string)> CreateRequest(AppointmentRequestDto appointmentRequest, CancellationToken cancellationToken = default)
	{
		var entity = appointmentRequestDtoTranslator.AppointmentRequestDtoToAppointmentRequest(appointmentRequest);
		var guid = await repository.Add(entity, cancellationToken);

		return (guid, repository.GetConcurrencyToken(entity));
	}

	public async Task<(AppointmentRequestDto, string)> GetRequest(Guid id, CancellationToken cancellationToken = default)
	{
		var appointmentRequest = await repository.Get(id, cancellationToken);
		return (appointmentRequestDtoTranslator.AppointmentRequestToAppointmentRequestDto(appointmentRequest), repository.GetConcurrencyToken(appointmentRequest));
	}

	public Task<(AppointmentRequestDto, string)> GetRequest(Guid id, string etag, CancellationToken _ = default)
	{
		return repository.TryGetIfModified(id, etag, out var appointmentRequest)
			? Task.FromResult((
				appointmentRequestDtoTranslator.AppointmentRequestToAppointmentRequestDto(appointmentRequest!),
				repository.GetConcurrencyToken(appointmentRequest!)))
			: throw new ConcurrencyException();
	}

	public async IAsyncEnumerable<(AppointmentRequestDto, Guid, string)> GetRequests([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var result = repository.Get(cancellationToken);
		await foreach (var item in result.WithCancellation(cancellationToken))
		{
			yield return (appointmentRequestDtoTranslator.AppointmentRequestToAppointmentRequestDto(item), repository.GetId(item),
				repository.GetConcurrencyToken(item));
		}
	}

	public async Task RemoveRequest(Guid id, CancellationToken cancellationToken = default)
	{
		await repository.Remove(id, cancellationToken);
	}

	public async Task RemoveRequest(Guid id, string etag, CancellationToken cancellationToken = default)
	{
		await repository.RemoveIfMatch(id, etag, cancellationToken);
	}

	internal async Task<string> ReplaceRequest(Guid id, AppointmentRequestDto appointmentRequest,
		CancellationToken cancellationToken = default)
	{
		var entity = appointmentRequestDtoTranslator.AppointmentRequestDtoToAppointmentRequest(appointmentRequest);
		await repository.Replace(id, entity, cancellationToken);

		return repository.GetConcurrencyToken(entity);
	}

	internal async Task<string> ReplaceRequest(Guid id, AppointmentRequestDto appointmentRequest, string etag,
		CancellationToken cancellationToken = default)
	{
		var entity = appointmentRequestDtoTranslator.AppointmentRequestDtoToAppointmentRequest(appointmentRequest);
		await repository.ReplaceIfMatch(id, entity, etag, cancellationToken);

		return repository.GetConcurrencyToken(entity);
	}

	public async Task<(AppointmentRequestDto, string)> UpdateRequest(Guid id, JsonPatchDocument<AppointmentRequestDto> patchDocument,
		CancellationToken cancellationToken = default)
	{
		var current = await repository.Get(id, cancellationToken);
		var currentDto = appointmentRequestDtoTranslator.AppointmentRequestToAppointmentRequestDto(current);
		patchDocument.ApplyTo(currentDto);
		await repository.Replace(id, appointmentRequestDtoTranslator.AppointmentRequestDtoToAppointmentRequest(currentDto), cancellationToken);
		return (currentDto, repository.GetConcurrencyToken(current));
	}

	public async Task<(AppointmentRequestDto, string)> UpdateRequest(Guid id, JsonPatchDocument<AppointmentRequestDto> patchDocument,
		string etag, CancellationToken cancellationToken = default)
	{
		var current = await repository.Get(id, cancellationToken);
		var currentDto = appointmentRequestDtoTranslator.AppointmentRequestToAppointmentRequestDto(current);
		patchDocument.ApplyTo(currentDto);
		await repository.ReplaceIfMatch(id, appointmentRequestDtoTranslator.AppointmentRequestDtoToAppointmentRequest(currentDto), etag, cancellationToken);
		return (currentDto, repository.GetConcurrencyToken(current));
	}
}
