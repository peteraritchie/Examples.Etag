namespace Examples.Etag.WebApi;
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal class CosmosOptions
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
{
	public const string Cosmos = "Cosmos";
	public string? EndpointUri { get; set; }
	public string? PrimaryKey { get; set; }
	public string? ApplicationName { get; set; }
	public string? DatabaseName { get; set;}
	public string? ContainerName { get; set; }
	public string? PrimaryKeyPath { get; set; } = "/id";
	public int? ThroughputRequestUnits { get; set; } = 400;
}
