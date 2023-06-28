namespace Examples.Etag.WebApi.Common.Abstractions;

public interface ITranslator<TDomain, TData>
{
	TDomain ToDomain(TData data);
	TData ToData(TDomain domain);
}
