using System;

namespace ironsource {
	public class StreamData {
		public string name_;
		public string token_;

		/// <summary>
		/// Stream Data contructor
		/// </summary>
		/// <param name="name">
		/// <see cref="string"/> Stream name
		/// </param>
		/// <param name="token">
		/// <see cref="string"/> Stream Auth key
		/// </param>
		public StreamData(string name = "", string token = "") {
			name_ = name;
			token_ = token;
		}
	}
}