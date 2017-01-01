using System;
using System.Collections;
using System.Collections.Generic;

namespace ironsource {
    /// <summary>
    ///  Holds a single atom event data (stream name, data itself and auth key for stream)
    /// </summary>
    public class Event {
        public string data_;
        public string stream_;
        public string authKey_;

        /// <summary>
        /// Event constructor
        /// </summary>
        /// <param name="stream">
        /// <see cref="string"/> name of stream.
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> data for server.
        /// </param>
        /// <param name="authKey">
        /// <see cref="string"/> stream auth key
        /// </param>
        public Event(string stream, string data, string authKey) {
            stream_ = stream;
            data_ = data;
            authKey_ = authKey;
        }
    }
}

