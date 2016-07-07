using System;

namespace ironsource {
    public class Response {
        public string error;
        public string data;
        public int status;

        /// <summary>
        /// Constructor for Response
        /// </summary>
        /// <param name="error">
        /// <see cref="string"/> for server reponse error message.
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> for server response data.
        /// </param>
        /// <param name="status">
        /// <see cref="int"/> for server reponse status.
        /// </param>
        public Response(string error, string data, int status) {
            this.error = error;
            this.data = data;
            this.status = status;
        }
    }
}
