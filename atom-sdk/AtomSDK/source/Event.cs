using System;
using System.Collections;
using System.Collections.Generic;

namespace ironsource {
    public class Event {
        public string data_;
        public string stream_;
        public string authKey_;

        /// <summary>
        /// Batch contructor
        /// </summary>
        /// <param name="stream">
        /// <see cref="string"/> name of stream.
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> data for server.
        /// </param>
        public Event(string stream, string data, string authKey) {
            stream_ = stream;
            data_ = data;
            authKey_ = authKey;
        }
    }
}

