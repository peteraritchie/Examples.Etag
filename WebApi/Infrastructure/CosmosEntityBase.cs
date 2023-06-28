using Newtonsoft.Json;

namespace Examples.Etag.WebApi.Infrastructure;

/// <summary>
/// Properties specific to Cosmos DB documents
/// </summary>
public abstract class CosmosEntityBase
{
	[JsonProperty(PropertyName = "id")]
	public Guid Id { get; set; }

	[JsonProperty(PropertyName = "_rid")]
	public string? ResourceId { get; set; }

	[JsonProperty(PropertyName = "_self")]
	public Uri? SelfUri { get; set; }

	[JsonProperty(PropertyName = "_etag")]
	public string? ETag{ get; set; }

	[JsonProperty(PropertyName = "_ts")]
	public int? TimestampText{ get; set; }
}
