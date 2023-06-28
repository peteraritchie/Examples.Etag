using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Examples.Etag.WebApi.Common.Abstractions;
using Examples.Etag.WebApi.Common.DomainEssentials;
using Examples.Etag.WebApi.Common.Exceptions;
using Examples.Etag.WebApi.Domain.Abstractions;

namespace Examples.Etag.WebApi.Infrastructure;

/// <summary>
/// A generic infrastructure service that abstracts the collaboration with a Azure Cosmos DB.
/// Responsible for translating to/from domain entity and database entity (serialization class).
/// </summary>
/// <typeparam name="TDomainEntity">The domain entity type</typeparam>
/// <typeparam name="TDbEntity">The database serialization entity type</typeparam>
public class CosmosOptimisticallyConcurrentRepository<TDomainEntity, TDbEntity> 
	: IOptimisticallyConcurrentRepository<TDomainEntity>
	where TDomainEntity : class
	where TDbEntity : CosmosEntityBase
{
	///// <summary>
	///// 
	///// </summary>
	///// <param name="entity"></param>
	///// <param name="token"></param>
	///// <exception cref="InvalidOperationException"></exception>
	///// <returns></returns>
	//public bool IsModified(TDomainEntity entity, string token)
	//{
	//	var currentToken = GetConcurrencyToken(entity);
	//	return currentToken != token;
	//}

	/// <summary>
	/// Abstraction of meta information about an entity instance.
	/// </summary>
	private class EntityContext
	{
		public EntityContext(Guid id, string concurrencyToken)
		{
			Id = id;
			ConcurrencyToken = concurrencyToken;
		}

		/// <summary>
		/// The ID of the entity in the database
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// The concurrency token (Etag) of the entity in the database)
		/// </summary>
		public string ConcurrencyToken { get; }
	}

	private readonly Container container;
	private readonly ConditionalWeakTable<TDomainEntity, EntityContext> conditionalWeakTable = new();
	private readonly ITranslator<TDomainEntity, TDbEntity> translator;
	private readonly Action<TDbEntity, Guid> setDbEntityId;

	protected CosmosOptimisticallyConcurrentRepository(Container container, ITranslator<TDomainEntity, TDbEntity> translator,
		Action<TDbEntity, Guid> setDbEntityId)
	{
		this.container = container;
		this.translator = translator;
		this.setDbEntityId = setDbEntityId;
	}

	/// <inheritdoc/>
	public async Task<TDomainEntity> Get(Guid id, CancellationToken cancellationToken = default)
	{
		var idText = id.ToString("D");
		try
		{
			var result = await container.ReadItemAsync<TDbEntity>(idText, new PartitionKey(idText), cancellationToken: cancellationToken);
			var entity = translator.ToDomain(result.Resource);
			conditionalWeakTable.Add(entity, new EntityContext(id, result.ETag));
			return entity;
		}
		catch (CosmosException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
		{
			throw new EntityNotFoundException(id);
		}
	}

	/// <inheritdoc/>
	public async IAsyncEnumerable<TDomainEntity> Get([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var iterator = container.GetItemQueryIterator<TDbEntity>();
		while (iterator.HasMoreResults)
		{
			var set = await iterator.ReadNextAsync(cancellationToken);
			foreach (var e in set)
			{
				var entity = translator.ToDomain(e);
				conditionalWeakTable.Add(entity, new EntityContext(e.Id, e.ETag!));
				yield return entity;
			}
		}
	}

	/// <inheritdoc/>
	public Guid GetId(TDomainEntity entity)
	{
		return !conditionalWeakTable.TryGetValue(entity, out var context)
			? throw new InvalidOperationException("Domain entity was unknown to repository")
			: context.Id;
	}

	/// <inheritdoc/>
	public bool TryGetIfModified(Guid id, string concurrencyToken, out TDomainEntity? entity)
	{
		var idText = id.ToString("D");
		try
		{
			var result = container.ReadItemAsync<TDbEntity>(
					idText,
					new PartitionKey(idText),
					requestOptions: new ItemRequestOptions { IfNoneMatchEtag = concurrencyToken })
				.Result;

			entity = translator.ToDomain(result.Resource);
			conditionalWeakTable.Add(entity, new EntityContext(id, result.ETag));
			return true;
		}
		catch (AggregateException aggregateException) when (aggregateException.InnerExceptions.Count == 1 &&
		                                                    aggregateException.InnerExceptions.Single() is
			                                                    CosmosException
			                                                    {
				                                                    StatusCode: HttpStatusCode.NotModified
			                                                    })
		{
			entity = default;
			return false;
		}
		catch (AggregateException aggregateException) when (aggregateException.InnerExceptions.Count == 1 &&
		                                                    aggregateException.InnerExceptions.Single() is
			                                                    CosmosException
			                                                    {
				                                                    StatusCode: HttpStatusCode.NotFound
			                                                    })
		{
			throw new EntityNotFoundException(id);
		}
		catch (CosmosException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
		{
			throw new EntityNotFoundException(id);
		}
	}

	/// <inheritdoc/>
	public string GetConcurrencyToken(TDomainEntity entity)
	{
		return !conditionalWeakTable.TryGetValue(entity, out var context)
			? throw new InvalidOperationException("Domain entity was unknown to repository")
			: context.ConcurrencyToken;
	}

	/// <inheritdoc/>
	public async Task<Guid> Add(TDomainEntity entity, CancellationToken cancellationToken = default)
	{
		var id = Guid.NewGuid();
		var dbEntity = translator.ToData(entity);
		setDbEntityId(dbEntity, id);

		try
		{
			var result = await container.CreateItemAsync(dbEntity, new PartitionKey(id.ToString("D")), cancellationToken: cancellationToken);
			conditionalWeakTable.Add(entity, new EntityContext(id, result.ETag));
			return id;
		}
		catch (CosmosException ex) when(ex.StatusCode == HttpStatusCode.PreconditionFailed)
		{
			throw new ConcurrencyException();
		}
	}

	/// <inheritdoc/>
	public async Task Remove(Guid id, CancellationToken cancellationToken = default)
	{
		var idText = id.ToString("D");

		try
		{
			_ = await container.DeleteItemAsync<TDbEntity>(idText, new PartitionKey(idText), cancellationToken: cancellationToken);
		}
		catch (CosmosException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
		{
			throw new EntityNotFoundException(id);
		}
	}

	/// <inheritdoc/>
	public async Task Replace(Guid id, TDomainEntity entity, CancellationToken cancellationToken = default)
	{
		var dbEntity = translator.ToData(entity);
		setDbEntityId(dbEntity, id);

		try
		{
			var result = await container.UpsertItemAsync(dbEntity, cancellationToken: cancellationToken);
			conditionalWeakTable.Add(entity, new EntityContext(id, result.ETag));
		}
		catch (CosmosException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
		{
			throw new EntityNotFoundException(id);
		}
	}

	/// <inheritdoc/>
	public async Task RemoveIfMatch(Guid id, string token, CancellationToken cancellationToken = default)
	{
		var idText = id.ToString("D");

		var requestOptions = new ItemRequestOptions { IfMatchEtag = token };
		try
		{
			_ = await container.DeleteItemAsync<TDbEntity>(idText, new PartitionKey(idText), requestOptions: requestOptions, cancellationToken: cancellationToken);
		}
		catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
		{
			throw new ConcurrencyException();
		}
		catch (CosmosException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
		{
			throw new EntityNotFoundException(id);
		}
	}

	/// <inheritdoc/>
	public async Task ReplaceIfMatch(Guid id, TDomainEntity entity, string token, CancellationToken cancellationToken = default)
	{
		var idText = id.ToString("D");
		var dbEntity = translator.ToData(entity);
		setDbEntityId(dbEntity, id);

		var requestOptions = new ItemRequestOptions { IfMatchEtag = token };
		try
		{
			var result = await container.ReplaceItemAsync(dbEntity, idText, new PartitionKey(idText), requestOptions: requestOptions, cancellationToken: cancellationToken);
			conditionalWeakTable.Add(entity, new EntityContext(id, result.ETag));
		}
		catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
		{
			throw new ConcurrencyException();
		}
		catch (CosmosException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
		{
			throw new EntityNotFoundException(id);
		}
	}
}
