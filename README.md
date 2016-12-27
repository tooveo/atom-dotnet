# ironSource.atom SDK for .NET

[![License][license-image]][license-url]
[![Docs][docs-image]][docs-url]
[![Build status][travis-image]][travis-url]
[![Coverage Status][coverage-image]][coverage-url]

atom-dotnet is the official [ironSource.atom](http://www.ironsrc.com/data-flow-management) SDK for the .NET Framework.

- [Signup](https://atom.ironsrc.com/#/signup)
- [Documentation](https://ironsource.github.io/atom-dotnet/)
- [Installation](#installation)
- [Usage](#usage)
- [Change Log](#change-log)
- [Example](#example)

## Installation

Add dependency for Atom SDK DLL from [dist folder](dist/).

## Usage
 
### High Level API - "Tracker"
The Tracker is used for sending events to Atom based on several conditions:
- Every 30 seconds (default)
- Number of accumulated events has reached 200 (default)
- Size of accumulated events has reached 512KB (default)
Uses exponential back off mechanism with jitter.

The tracker is a based on a thread pool which is controlled by BatchEventPool and a backlog (QueueEventStorage)    
By default the BatchEventPool is configured to use 1 thread (worker), you can change it when constructing the tracker.
To see the full code check the [example section](#example)

```csharp

// Tracker defaults:

private const int TASK_WORKERS_COUNT_ = 1;
private const int TASK_POOL_SIZE_ = 10;

// The flush interval in milliseconds
private long flushInterval_ = 30000;

// Bulk length (number of events)
private int bulkLength_ = 200;

// The size of the bulk in bytes.
private int bulkBytesSize_ = 512 * 1024;

IronSourceAtomTracker tracker = new IronSourceAtomTracker();
// print debug info in console
tracker.EnableDebug(true);

tracker.SetBulkBytesSize(2);
tracker.SetFlushInterval(2000);
tracker.SetEndpoint("http://track.atom-data.io/");

// set event pool size and worker threads count
tracker.SetTaskPoolSize(1000);
tracker.SetTaskWorkersCount(24);

string data = "{\"strings\": \"data track\"}";

tracker.track("<YOUR_STREAM_NAME>", data, "<YOUR_AUTH_KEY>");

// stop all tracker workers
tracker.Stop();
```

Interface for store data `IEventStorage`.
Implementation must to be synchronized for multithreading use.
```csharp
using System;

namespace ironsource {
    public interface IEventStorage {
        // Save event data to your storage (RAM, SQLite, File etc.)
        void addEvent(Event eventObject);

        // Get Event from storage for specific stream
        Event getEvent(string stream);
    }
}
```
Using custom storage implementation:
```csharp
IronSourceAtomTracker tracker = new IronSourceAtomTracker();

IEventStorage customEventStorage = new QueueEventStorage();
tracker.SetEventStorage(customEventStorage);
```

### Low level API usage
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
## Change Log

### v1.1.0
- Changed setBulkSize to setBulkLength
- Fixed a bug in tracker that caused several conditions to flush at the same time
- Added example with static usage of the SDK
- Changed tracker defaults
- Changed the name of QueueEventManager to QueueEventStorage
- Changed the name of EventTaskPool to BatchEventPool
- Renamed eventWorker to trackerHandler
- Renamed  GetRequestData function to createRequestData
- Renamed EventManager interface to EventStorage

### v1.0.0
- Tracker
- Low level methods
- Storage interface

### License
[MIT](LICENSE)

[license-image]: https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square
[license-url]: LICENSE
[docs-image]: https://img.shields.io/badge/docs-latest-blue.svg
[docs-url]: https://ironsource.github.io/atom-dotnet/
[travis-image]: https://travis-ci.org/ironSource/atom-dotnet.svg?branch=master
[travis-url]: https://travis-ci.org/ironSource/atom-dotnet
[coverage-image]: https://coveralls.io/repos/github/ironSource/atom-dotnet/badge.svg?branch=master
[coverage-url]: https://coveralls.io/github/ironSource/atom-dotnet?branch=master
