# ironSource.atom SDK for .NET

[![License][license-image]][license-url]
[![Docs][docs-image]][docs-url]

atom-dotnet is the official [ironSource.atom](http://www.ironsrc.com/data-flow-management) SDK for the .NET Framework.

- [Signup](https://atom.ironsrc.com/#/signup)
- [Documentation](https://ironsource.github.io/atom-dotnet/)
- [Sending an event](#Using-the-IronSource-API-to-send-events)

#### Using the IronSource API to send events 
##### Low level API usage
Example of sending an event in C#:
```csharp
	IronSourceAtom api = new IronSourceAtom();
	// print debug info in console
	api.EnableDebug(true);

	// example put event "GET"
	string streamGet = "<YOUR_STREAM_NAME>;
	string dataGet = "{\"strings\": \"data GET\"}";

	api.SetAuth("<YOUR_AUTH_KEY>");
	Response responseGet = api.PutEvent(streamGet, dataGet, HttpMethod.GET);

	Console.WriteLine("GET data: " + responseGet.data);
	Console.WriteLine("GET error: " + responseGet.error);
	Console.WriteLine("GET status: " + responseGet.status);

	// example put event "POST"
	string streamPost = "<YOUR_STREAM_NAME>";
	string dataPost = "{\"strings\": \"data POST\"}";

	api.SetAuth("<YOUR_AUTH_KEY>");
	Response responsePost = api.PutEvent(streamPost, dataPost);

	Console.WriteLine("POST data: " + responsePost.data);
	Console.WriteLine("POST error: " + responsePost.error);
	Console.WriteLine("POST status: " + responsePost.status);

	// example put events - bulk
	string streamBulk = "<YOUR_STREAM_NAME>";
	List<String> dataBulk = new List<string>(); 
	dataBulk.Add("{\"strings\": \"test BULK 1\"}");
	dataBulk.Add("{\"strings\": \"test BULK 2\"}");
	dataBulk.Add("{\"strings\": \"test BULK 3\"}");

	api.SetAuth("<YOUR_AUTH_KEY>");
	Response responseBulk = api.PutEvents(streamBulk, dataBulk);

	Console.WriteLine("Bulk data: " + responseBulk.data);
	Console.WriteLine("Bulk error: " + responseBulk.error);
	Console.WriteLine("Bulk status: " + responseBulk.status);
```

### License
MIT

[license-image]: https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square
[license-url]: LICENSE
[docs-image]: https://img.shields.io/badge/docs-latest-blue.svg
[docs-url]: https://ironsource.github.io/atom-dotnet/