using System;
using System.Collections;
using System.Collections.Generic;

namespace ironsource {
	public class Batch {
		public List<string> events_;
		public int lastId_;

		/// <summary>
		/// Batch contructor
		/// </summary>
		/// <param name="events">
		/// <see cref="List<string>"/> list of events.
		/// </param>
		/// <param name="lastId">
		/// <see cref="int"/> last Id getted from DB.
		/// </param>
		public Batch(List<string> events, int lastId) {
			events_ = events;
			lastId_ = lastId;
		}
	}
}

