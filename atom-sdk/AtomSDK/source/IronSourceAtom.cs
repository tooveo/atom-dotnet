using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

namespace ironsource {
	public class IronSourceAtom {
		protected static string API_VERSION_ = "V1.0.0";

		protected string endpoint_ = "http://track.atom-data.io/";
		protected string authKey_ = "";

		protected Dictionary<string, string> headers_ = new Dictionary<string, string>();

		protected bool isDebug_ = false;

		/// <summary>
		/// API constructor
		/// </summary>
		/// <param name="gameObject">
		/// <see cref="GameObject"/> for coroutine method call.
		/// </param>
		public IronSourceAtom() {
			initHeaders();
		}

		/// <summary>
		/// Enabling print debug information
		/// </summary>
		/// <param name="isDebug">
		/// If set to <c>true</c> is debug.
		/// </param>
		public void EnableDebug(bool isDebug) {
			isDebug_ = isDebug;
		}

		/// <summary>
		/// Inits the headers.
		/// </summary>
		protected virtual void initHeaders() {            
			headers_.Add("x-ironsource-atom-sdk-type", "unity");
			headers_.Add("x-ironsource-atom-sdk-version", IronSourceAtom.API_VERSION_);
		}

		/// <summary>
		/// Prints the log.
		/// </summary>
		/// <param name="logData">Log data.</param>
		protected void printLog(string logData) {			
			Debug.WriteLineIf(isDebug_, logData);
		}

		/// <summary>
		/// API destructor - clear craeted IronSourceCoroutineHandler
		/// </summary>       
		~IronSourceAtom() {
		}

		/// <summary>
		/// Set Auth Key for stream
		/// </summary>  
		/// <param name="authKey">
		/// <see cref="string"/> for secret key of stream.
		/// </param>
		public virtual void SetAuth(string authKey) {
			authKey_ = authKey;
		}

		/// <summary>
		/// Set endpoint for send data
		/// </summary>
		/// <param name="endpoint">
		/// <see cref="string"/> for address of server
		/// </param>
		public void SetEndpoint(string endpoint) {
			endpoint_ = endpoint;
		}


		/// <summary>
		/// Send single data to Atom server.
		/// </summary>
		/// <param name="stream">
		/// Stream name for saving data in db table
		/// </param>
		/// <param name="data">
		/// user data to send
		/// </param>
		/// <param name="method">
		/// <see cref="HttpMethod"/> for POST or GET method for do request
		/// </param>
		/// <param name="callback">
		/// <see cref="string"/> for response data
		/// </param>
		public Response PutEvent(string stream, string data, HttpMethod method = HttpMethod.POST) {
			string jsonEvent = GetRequestData(stream, data);
			return SendEvent(endpoint_, method, headers_, jsonEvent);
		}

		/// <summary>
		/// Send multiple events data to Atom server.
		/// </summary>
		/// <param name="stream">
		/// <see cref="string"/> for name of stream
		/// </param>
		/// <param name="data">
		/// <see cref="string"/> for request data
		/// </param>
		/// <param name="method">
		/// <see cref="HttpMethod"/> for type of request
		/// </param>
		/// <param name="callback">
		/// <see cref="Action<Response>"/> for reponse data
		/// </param>
		public Response PutEvents(string stream, List<string> data) {            
			string json = Utils.ListToJson(data);
			return PutEvents(stream, json);
		}

		public Response PutEvents(string stream, string data) {
			HttpMethod method = HttpMethod.POST;
			printLog("Key: " + authKey_);

			string jsonEvent = GetRequestData(stream, data);

			return SendEvent(endpoint_ + "bulk", method, headers_, jsonEvent);
		}

		/// <summary>
		/// Create request json data
		/// </summary>
		/// <returns>The request data.</returns>
		/// <param name="stream">
		/// <see cref="string"/> for request stream
		/// </param>
		/// <param name="data">
		/// <see cref="string"/> for request data
		/// </param>
		protected string GetRequestData(string stream, string data) {
			string hash = Utils.EncodeHmac(data, Encoding.ASCII.GetBytes(authKey_));

			var eventObject = new Dictionary<string, string>();
			eventObject ["table"] = stream;
			eventObject["data"] = Utils.EscapeStringValue(data);
			eventObject["auth"] = hash;
			string jsonEvent = Utils.DictionaryToJson(eventObject);

			printLog("Request body: " + jsonEvent);

			return jsonEvent;
		}

		/// <summary>
		/// Check health of server
		/// </summary>     
		public void Health() {
			SendEvent(endpoint_ + "health", HttpMethod.GET, headers_, "");
		}

		/// <summary>
		/// Send data to server
		/// </summary>
		/// <param name="url">
		/// <see cref="string"/> for server address
		/// </param>
		/// <param name="method">
		/// <see cref="HttpMethod"/> for POST or GET method 
		/// </param> 
		/// <param name="headers">
		/// <see cref="Dictionary<string, string>"/>
		/// </param> 
		/// <param name="data">
		/// <see cref="string"/> for request data
		/// </param> 
		protected Response SendEvent(string url, HttpMethod method, Dictionary<string, string> headers,
								     string data) {
			Request request = new Request(url, data, headers, isDebug_);
			return (method == HttpMethod.GET) ? request.Get() : request.Post();
		}
	}
}

