using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mPower.Framework.Utils {
	public class StreamUtils {

		/// <summary>
		/// Reads a response from a stream and converts it to a string.
		/// </summary>
		/// <remarks>
		/// Does not close the stream when finished.
		/// Default encoding is UTF-8.
		/// Throws an ArgumentException if totalLength is <= 0
		/// </remarks>
		/// <param name="stream">The stream to read from</param>
		/// <param name="totalLength">The total length to read</param>
		/// <returns>An encoded string from the stream</returns>
		public static string ReadResponseToEnd(Stream stream, long totalLength) {
			return ReadResponseToEnd(stream, totalLength, Encoding.UTF8);
		}

		/// <summary>
		/// Reads a response from a stream and converts it to a string.
		/// </summary>
		/// <remarks>
		/// Does not close the stream when finished.
		/// Throws an ArgumentException if totalLength is <= 0
		/// </remarks>
		/// <param name="stream">The stream to read from</param>
		/// <param name="totalLength">The total length to read</param>
		/// <param name="encoding">The encoding to encode the bytes as</param>
		/// <returns>An encoded string from the stream</returns>
		public static string ReadResponseToEnd(Stream stream, long totalLength, Encoding encoding) {
			if(totalLength <= 0) {
				throw new ArgumentException("totalLength has to be greater than 0");
			}

			const int bufferSize = 512;
			StringBuilder builder = new StringBuilder();

			byte[] buffer = null;
			if(totalLength < bufferSize) {
				buffer = new byte[totalLength];
			} else {
				buffer = new byte[bufferSize];
			}

			long currentCount = 0;
			int bytesRead = 0;
			while(currentCount < totalLength) {
				bytesRead = stream.Read(buffer, 0, buffer.Length);
				currentCount += bytesRead;
				builder.Append(encoding.GetString(buffer.Take(bytesRead).ToArray()));
			}

			return builder.ToString();
		}
	}
}
