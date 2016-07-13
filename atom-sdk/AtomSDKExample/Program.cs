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
			// test for bulk size
			tracker.SetBulkBytesSize(2);
			tracker.SetFlushInterval(2000);
			tracker.SetEndpoint("http://track.atom-data.io1/");

			int index = 0;

			int eventSended = 0;
			bool isRunThreads = true;
			for (int i = 0; i < 5; ++i) {
				int threadIndex = i;

				Action eventSend = delegate() {
					while (isRunThreads) {
						string data = "{\"strings\": \"---- d: " + Interlocked.Increment(ref index) + 
						" t: " + threadIndex + "\"}";

						Debug.WriteLine("Send: " + data);

						tracker.Track("sdkdev_sdkdev.public.g8y3etest", data, "I40iwPPOsG3dfWX30labriCg9HqMfL");

						if (Interlocked.Increment(ref eventSended) >= 33) {
							isRunThreads = false;
						}

						Thread.Sleep(3000);
					}
				};

				ThreadStart threadMethodHolder = new ThreadStart(eventSend);
				Thread thread = new Thread(threadMethodHolder);

				thread.Start();
			}

			Thread.Sleep(30000);
			//tracker.Stop();

			/*
			// example put event "GET"
			string streamGet = "sdkdev_sdkdev.public.g8y3etest";
			string dataGet = "{\"strings\": \"data GET\"}";

			Response responseGet = api.PutEvent(streamGet, dataGet, HttpMethod.GET, "I40iwPPOsG3dfWX30labriCg9HqMfL");

			Console.WriteLine("GET data: " + responseGet.data);
			Console.WriteLine("GET error: " + responseGet.error);
			Console.WriteLine("GET status: " + responseGet.status);

			// example put event "POST"
			string streamPost = "sdkdev_sdkdev.public.g8y3etest";
			string dataPost = "{\"strings\": \"data POST\"}";

			//api.SetAuth("I40iwPPOsG3dfWX30labriCg9HqMfL");
			Response responsePost = api.PutEvent(streamPost, dataPost);

			Console.WriteLine("POST data: " + responsePost.data);
			Console.WriteLine("POST error: " + responsePost.error);
			Console.WriteLine("POST status: " + responsePost.status);

			// example put events - bulk
			string streamBulk = "sdkdev_sdkdev.public.g8y3etest";
			List<String> dataBulk = new List<string>(); 
			dataBulk.Add("{\"strings\": \"test BULK 1\"}");
			dataBulk.Add("{\"strings\": \"test BULK 2\"}");
			dataBulk.Add("{\"strings\": \"test BULK 3\"}");

			//api.SetAuth("I40iwPPOsG3dfWX30labriCg9HqMfL");
			Response responseBulk = api.PutEvents(streamBulk, dataBulk);

			Console.WriteLine("Bulk data: " + responseBulk.data);
			Console.WriteLine("Bulk error: " + responseBulk.error);
			Console.WriteLine("Bulk status: " + responseBulk.status);

			Console.WriteLine("End of the example!;");
			*/
		}
	}
}
