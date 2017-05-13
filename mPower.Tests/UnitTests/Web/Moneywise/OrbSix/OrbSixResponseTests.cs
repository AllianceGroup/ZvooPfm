using MoneyWise.Services;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Moneywise.OrbSix {
	[TestFixture]
	public class OrbSixResponseTests {

		[Test]
		public void GetToken_Pull_Token_From_XML_Text_Token_Found() {
			string token = "i'm just an auth token";
			string xml = @"<?xml version='1.0' encoding='utf-8'?><resultSet><authenticationToken>{0}</authenticationToken></resultSet>";

			string tokenResult = OrbSixServices.GetToken(string.Format(xml, token));

			Assert.AreEqual(token, tokenResult);
		}

		[Test]
		public void GetToken_Pull_Token_From_XML_Text_Token_Not_Found() {
			string xml = @"<?xml version='1.0' encoding='utf-8'?><resultSet><authenticationToken></authenticationToken></resultSet>";

			string tokenResult = OrbSixServices.GetToken(xml);

			Assert.AreEqual(null, tokenResult);
		}

		[Test]
		public void ValidChecksum_Compute_Checksum_Valid_Checksum() {
			string xml = @"<?xml version='1.0' encoding='utf-8'?><resultSet md5sum='51ee66bdadfae357bbb5b60aa08de888'><result><value><![CDATA[Doesn't matter what this is]]></value></result></resultSet>";

			bool result = OrbSixServices.ValidChecksum(xml);

			Assert.IsTrue(result);
		}

		[Test]
		public void ValidChecksum_Compute_Checksum_Invalid_Checksum() {
			string xml = @"<?xml version='1.0' encoding='utf-8'?><resultSet md5sum='what is this, this is not a checksum'><result><value><![CDATA[Doesn't matter what this is]]></value></result></resultSet>";

			bool result = OrbSixServices.ValidChecksum(xml);

			Assert.IsFalse(result);
		}
	}
}
