using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Framework.Utils {

	[TestFixture]
	public class StreamUtilTests {

		[Test]
		public void ReadResponseToEnd_Method_Reads_Entire_Stream_Based_On_Positive_Content_Length() {
			string testString = "I'm sorry, Dave. I'm afraid I can't do that.";
			byte[] testStringAsBytes = Encoding.UTF8.GetBytes(testString);

			using(MemoryStream stream = new MemoryStream(testStringAsBytes)) {

				string response = StreamUtils.ReadResponseToEnd(stream, testStringAsBytes.Length);

				Assert.AreEqual(testString, response);
			}
		}

		[Test]
		public void ReadResponseToEnd_Method_Reads_Entire_Stream_Based_On_Non_Positive_Content_Length() {
			byte[] testStringAsBytes = new byte[0];

			using(MemoryStream stream = new MemoryStream(testStringAsBytes)) {

				try {
					string response = StreamUtils.ReadResponseToEnd(stream, testStringAsBytes.Length);

					Assert.Fail();
				} catch (ArgumentException e) {
					Assert.AreEqual("totalLength has to be greater than 0", e.Message);
				}
			}
		}
	}
}
