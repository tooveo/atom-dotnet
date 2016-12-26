using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using ironsource;

namespace AtomSDKExample {
	class MainClass	{
        static String stream = "sdkdev_sdkdev.public.g8y3etest";
        static String authKey = "I40iwPPOsG3dfWX30labriCg9HqMfL";

       // static IronSourceAtom atom_;
        static IronSourceAtomTracker atomTracker_ = new IronSourceAtomTracker();


		public static void Main(string[] args) {
            testMultiThread();
		}

        public static void testMultiThread() {
            atomTracker_.EnableDebug(true);
            atomTracker_.SetAuth(authKey);
            atomTracker_.SetBulkSize(2);
            atomTracker_.SetBulkBytesSize(2048);
            atomTracker_.SetFlushInterval(2000);

            LinkedList<Thread> threads = new LinkedList<Thread>();
            Random randomGen = new Random();

            for (int i = 0; i < 10; ++i) {
                int threadID = 40 + i;
                Action eventSend = delegate () {
                    Thread.Sleep(randomGen.Next(1000, 6000));

                    Console.WriteLine("From thread: " + threadID);
                    for (int j = 0; j < 6; ++j) {
                        string data = "{\"strings\": \"c# thread : " + threadID +
                            " req: " + j + "\", \"id\": " + threadID + "}";

                        atomTracker_.Track(stream, data);
                    }
                };

                ThreadStart threadMethodHolder = new ThreadStart(eventSend);
                Thread thread = new Thread(threadMethodHolder);

                thread.Start();
                threads.AddLast(thread);
            }

            Thread.Sleep(10000);
            foreach (Thread thread in threads) {
                thread.Join();
            }

            atomTracker_.Flush();
        }

        public static void test1() {
            IronSourceAtom api = new IronSourceAtom();
            api.EnableDebug(true);

            IronSourceAtomTracker tracker = new IronSourceAtomTracker();
            tracker.EnableDebug(true);

            // test for bulk size
            //tracker.SetBulkBytesSize(2);
            tracker.SetFlushInterval(3000);
            tracker.SetEndpoint("http://track.atom-data.io/");

            int index = 0;

            int eventSended = 0;
            bool isRunThreads = true;
            for (int i = 0; i < 10; ++i) {
                Action eventSend = delegate () {
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
