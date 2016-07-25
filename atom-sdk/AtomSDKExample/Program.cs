using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using ironsource;

namespace AtomSDKExample {
	class MainClass	{
		public static void Main(string[] args) {
			IronSourceAtom api = new IronSourceAtom();
			api.EnableDebug (true);

			IronSourceAtomTracker tracker = new IronSourceAtomTracker();
			tracker.EnableDebug(true);
			// set event pool size and worker threads count
			tracker.SetTaskPoolSize(1000);
            tracker.SetTaskWorkersCount(24);

			// test for bulk size
			//tracker.SetBulkBytesSize(2);
			tracker.SetFlushInterval(3000);
			tracker.SetEndpoint("http://track.atom-data.io/");

			int index = 0;

			int eventSended = 0;
			bool isRunThreads = true;
			for (int i = 0; i < 10; ++i) {
				Action eventSend = delegate() {
                    int threadIndex = i;

					while (isRunThreads) {
						string data = "{\"strings\": \"+++++ d: " + Interlocked.Increment(ref index) + 
						" t: " + threadIndex + "\"}";

                        if (threadIndex < 5) {
                            tracker.Track("ibtest", data, "");
                        } else {
                            // another stream
                            tracker.Track("ibtest2", data, "");
                        }

						if (Interlocked.Increment(ref eventSended) >= 33) {
							isRunThreads = false;
						}
					}
				};

				ThreadStart threadMethodHolder = new ThreadStart(eventSend);
				Thread thread = new Thread(threadMethodHolder);

				thread.Start();
			}

			Thread.Sleep(30000);
			tracker.Stop();

			// example put event "GET"
			string streamGet = "ibtest";
			string dataGet = "{\"strings\": \"data GET\"}";

			Response responseGet = api.PutEvent(streamGet, dataGet, HttpMethod.GET, "");

			Console.WriteLine("GET data: " + responseGet.data);
			Console.WriteLine("GET error: " + responseGet.error);
			Console.WriteLine("GET status: " + responseGet.status);

			// example put event "POST"
			string streamPost = "ibtest";
			string dataPost = "{\"strings\": \"data POST\"}";

			//api.SetAuth("I40iwPPOsG3dfWX30labriCg9HqMfL");
			Response responsePost = api.PutEvent(streamPost, dataPost);

			Console.WriteLine("POST data: " + responsePost.data);
			Console.WriteLine("POST error: " + responsePost.error);
			Console.WriteLine("POST status: " + responsePost.status);

			// example put events - bulk
			string streamBulk = "ibtest";
			List<String> dataBulk = new List<string>(); 
			dataBulk.Add("{\"strings\": \"test BULK 1\"}");
			dataBulk.Add("{\"strings\": \"test BULK 2\"}");
			dataBulk.Add("{\"strings\": \"test BULK 3\"}");

			Response responseBulk = api.PutEvents(streamBulk, dataBulk);

			Console.WriteLine("Bulk data: " + responseBulk.data);
			Console.WriteLine("Bulk error: " + responseBulk.error);
			Console.WriteLine("Bulk status: " + responseBulk.status);

			Console.WriteLine("End of the example!;");
		}
	}
}
