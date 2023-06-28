using System.ComponentModel.DataAnnotations;

namespace Examples.Etag.WebApi.Common.Web;

public class WebCollectionElement<T>
{
	public WebCollectionElement(T data, string href, string etag)
	{
		Data = data;
		Href = href;
		Etag = etag;
	}

	[Required]
	public string Href { get; set; }

	[Required]
	public T Data { get; set; }

	[Required]
	public string Etag { get; set; }
}
