﻿using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ironsource {
	public class Request {
		protected string url_;
		protected string data_;
		protected Dictionary<string, string> headers_;

		protected bool isDebug_;

		/// <summary>
		/// Constructor for Request
		/// </summary>
		/// <param name="url">
		/// <see cref="string"/> for server address.
		/// </param>
		/// <param name="data">
		/// <see cref="string"/> for sending data.
		/// </param> 
		/// <param name="headers">
		/// <see cref="Dictionary<string, string>"/> for sending headers.
		/// </param> 
		/// <param name="callback">
		/// <see cref="Action<Response>"/> for get response data.
		/// </param>        
		public Request(string url, string data, Dictionary<string, string> headers, bool isDebug = false) {
			url_ = url;
			data_ = data;
			headers_ = headers;

			isDebug_ = isDebug;
		}

		/// <summary>
		/// GET request to server
		/// </summary>
		public Response Get() {
			string url = url_ + "?data=" + Utils.Base64Encode(data_);
			printLog("Request URL: " + url);

			return ReadResponse(sendRequest("GET", url));
		}

		/// <summary>
		/// POST request to server
		/// </summary>
		public Response Post() {
			printLog("Request URL: " + url_);
			return ReadResponse(sendRequest("POST", url_, data_));
		}

		private HttpWebResponse sendRequest(string type, string url, string data = null) {
			HttpWebResponse response;
			try {
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
				request.Method = type;
				request.ContentType = "application/json";

				foreach (var entry in headers_) {
					request.Headers[entry.Key] = entry.Value;
				}

				if (type == "POST") {
					byte[] dataArray = Encoding.ASCII.GetBytes(data);

					Stream dataStream = request.GetRequestStream();
					dataStream.Write(dataArray, 0, dataArray.Length);
					dataStream.Close();
				}

				response = (HttpWebResponse)request.GetResponse();
			} catch (WebException exception) {
				// if error evaluate
				response = (HttpWebResponse)exception.Response;
			}

			return response;
		}

		/// <summary>
		/// Read response from HttpWebResponse object
		/// </summary>
		/// <param name="response">
		/// <see cref="HttpWebResponse"/> object with response information.
		/// </param>    
		private Response ReadResponse(HttpWebResponse response) {
			string responseData = null;
			using (var reader = new StreamReader(response.GetResponseStream())) {
				responseData = reader.ReadToEnd();
			}

			string data = null;
			string error = null;
			int status = (int)response.StatusCode;

			if (response.StatusCode != HttpStatusCode.OK) {
				error = responseData;
			} else {
				data = responseData;
			}

			return new Response(error, data, 200);
		}

		protected void printLog(string data) {
			Debug.WriteLineIf(isDebug_, data);
		}
	}
}
