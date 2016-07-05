using System;

using System.Diagnostics;

namespace ironsource {
	public class IronSourceAtomTracker {
		private float flushInterval_ = 10;
		private int bulkSize_ = 1000;
		private int bulkBytesSize_ = 64 * 1024;

		private IronSourceAtom api_;

		private bool isDebug_;

		/// <summary>
		/// API Tracker constructor
		/// </summary>
		public IronSourceAtomTracker() {
			api_ = new IronSourceAtom();
		}

		/// <summary>
		/// API destructor - clear craeted IronSourceCoroutineHandler
		/// </summary>       
		~IronSourceAtomTracker() {
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
		/// Set Auth Key for stream
		/// </summary>  
		/// <param name="authKey">
		/// <see cref="string"/> for secret key of stream.
		/// </param>
		public virtual void SetAuth(string authKey) {
			api_.SetAuth(authKey);
		}

		/// <summary>
		/// Set endpoint for send data
		/// </summary>
		/// <param name="endpoint">
		/// <see cref="string"/> for address of server
		/// </param>
		public void SetEndpoint(string endpoint) {
			api_.SetEndpoint(endpoint);
		}

		/// <summary>
		/// Set Bulk data count
		/// </summary>
		/// <param name="bulkSize">
		/// <see cref="int"/> Count of event for flush
		/// </param>
		public void SetBulkSize(int bulkSize) {
			bulkSize_ = bulkSize;
		}

		/// <summary>
		/// Set Bult data bytes size
		/// </summary>
		/// <param name="bulkBytesSize">
		/// <see cref="int"/> Size in bytes
		/// </param>
		public void SetBulkBytesSize(int bulkBytesSize) {
			bulkBytesSize_ = bulkBytesSize;
		}

		/// <summary>
		/// Set intervals for flushing data
		/// </summary>
		/// <param name="flushInterval">
		/// <see cref="float"/> Intervals in seconds
		/// </param>
		public void SetFlushInterval(float flushInterval) {
			flushInterval_ = flushInterval;
		}

		/// <summary>
		/// Track data to server
		/// </summary>
		/// <param name="stream">
		/// <see cref="string"/> Name of the stream
		/// </param>
		/// <param name="data">
		/// <see cref="string"/> Info for sending
		/// </param>
		/// <param name="token">
		/// <see cref="string"/> Secret token for stream
		/// </param>
		public void track(string stream, string data, string token = "") {
		}

		/// <summary>
		/// Flush all data to server
		/// </summary>
		public void flush() {
		}

		/// <summary>
		/// Prints the log.
		/// </summary>
		/// <param name="logData">Log data.</param>
		protected void PrintLog(string logData) {			
			Debug.WriteLineIf(isDebug_, logData);
		}
	}
}

