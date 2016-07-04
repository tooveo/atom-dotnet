using System;
using System.Collections;
using System.Collections.Generic;

using ironsource;

namespace AtomSDKExample {
	class MainClass	{
		public static void Main(string[] args) {
			IronSourceAtom api = new IronSourceAtom();

			// example put event "GET"
			string streamGet = "";
			string dataGet = "";

			Response responseGet = api.PutEvent(streamGet, dataGet, HttpMethod.GET);


			// example put event "POST"
			string streamPost = "";
			string dataPost = "";

			Response responsePost = api.PutEvent(streamPost, dataPost);

			// example put events - bulk

			string streamBulk = "";
			List<String> dataBulk = new List<string>(); 
			dataBulk.Add("{\"event\": \"test post 1\"}");
			dataBulk.Add("{\"event\": \"test post 2\"}");
			dataBulk.Add("{\"event\": \"test post 3\"}");

			Response responseBulk = api.PutEvents(streamBulk, dataBulk);

			Console.WriteLine("End of the example!;");
		}
	}
}
