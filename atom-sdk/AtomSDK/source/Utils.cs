using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace ironsource {
    public class Utils {
    	/// <summary>
    	/// Convert Dictionary to json string
    	/// </summary>
    	/// <param name="dictData">
    	/// <see cref="Dictionary<string, string>"/> data for convert.
    	/// </param>
    	public static string DictionaryToJson(Dictionary<string, string> dictData) {
    		var entries = dictData.Select(entryObject =>
			    string.Format("\"{0}\": \"{1}\"", entryObject.Key, entryObject.Value));

    		return "{" + entries.Aggregate((i, j) => i + "," + j) + "}";
    	}

    	/// <summary>
    	/// Convert List to json string
    	/// </summary>
    	/// <param name="listData">
    	/// <see cref="List<string>"/> data for convert.
    	/// </param>
    	public static string ListToJson(List<string> listData) {
		    return "[" + listData.Aggregate((i, j) => i + "," + j) + "]";
    	}

    	/// <summary>
    	/// Encode data to HMACSHA256
    	/// </summary>
    	/// <param name="input">
    	/// <see cref="string"/> data for encode.
    	/// </param>
    	/// <param name="key">
    	/// <see cref="byte[]"/> key for encode.
    	/// </param>
    	public static string EncodeHmac(string input, byte[] key) {
    		using (HMACSHA256 hmac = new HMACSHA256(key)) {
    			byte[] byteArray = Encoding.ASCII.GetBytes(input);
    			return hmac.ComputeHash(byteArray).Aggregate(String.Empty, (s, e) => s + String.Format("{0:x2}",e), 
				    s => s );
    		}
    	}

    	/// <summary>
    	/// Encode data to base64
    	/// </summary>
    	/// <param name="data">
    	/// <see cref="string"/> data to encode
    	/// </param>      
    	public static string Base64Encode(string data) {
    		return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
    	}

    	/// <summary>
    	/// Escape data in string
    	/// </summary>
    	/// <param name="value">
    	/// <see cref="string"/> data to escape
    	/// </param>  
    	public static string EscapeStringValue(string value) {
    		const char BACK_SLASH = '\\';
    		const char SLASH = '/';
    		const char DBL_QUOTE = '"';

    		var output = new StringBuilder(value.Length);
    		foreach (char c in value) {
    			switch (c) {
    			case SLASH:
    				output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
    				break;

    			case BACK_SLASH:
    				output.AppendFormat("{0}{0}", BACK_SLASH);
    				break;

    			case DBL_QUOTE:
    				output.AppendFormat("{0}{1}",BACK_SLASH,DBL_QUOTE);
    				break;

    			default:
    				output.Append(c);
    				break;
    			}
    		}

    		return output.ToString();
    	}

        /// <summary>
        /// Gets the current milliseconds.
        /// </summary>
        /// <returns>
        /// The current milliseconds.
        /// </returns>
    	public static long GetCurrentMilliseconds() {
            return (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);
    	}
    }
}

