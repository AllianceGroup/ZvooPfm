using Default.Areas.Administration.Services;
using NUnit.Framework;
using mPower.Documents.Documents.Membership;

namespace mPower.Tests.UnitTests.Web.Areas.Administration.Services {

	[TestFixture]
	public class UserSearchTests {

		#region UserMatches Tests
		
		[Test]
		public void UserMatches_MatchesFirstName_ReturnsTrue() {
			UserDocument user = new UserDocument { 
				FirstName = "firstname",
				LastName = "lastname",
				UserName = "username"
			};

			bool userMatches = UserSearchService.UserMatches(user, "first");

			Assert.IsTrue(userMatches);
		}

		[Test]
		public void UserMatches_MatchesLastName_ReturnsTrue() {
			UserDocument user = new UserDocument {
				FirstName = "firstname",
				LastName = "lastname",
				UserName = "username"
			};

			bool userMatches = UserSearchService.UserMatches(user, "last");

			Assert.IsTrue(userMatches);
		}

		[Test]
		public void UserMatches_MatchesUserName_ReturnsTrue() {
			UserDocument user = new UserDocument {
				FirstName = "firstname",
				LastName = "lastname",
				UserName = "username"
			};

			bool userMatches = UserSearchService.UserMatches(user, "user");

			Assert.IsTrue(userMatches);
		}

		[Test]
		public void UserMatches_MatchesAnyName_ReturnsTrue() {
			UserDocument user = new UserDocument {
				FirstName = "firstname",
				LastName = "lastname",
				UserName = "username"
			};

			bool userMatches = UserSearchService.UserMatches(user, "name");

			Assert.IsTrue(userMatches);
		}

		[Test]
		public void UserMatches_DoesNotMatchAnyName_ReturnsFalse() {
			UserDocument user = new UserDocument {
				FirstName = "firstname",
				LastName = "lastname",
				UserName = "username"
			};

			bool userMatches = UserSearchService.UserMatches(user, "this will not match");

			Assert.IsFalse(userMatches);
		}

		[Test]
		public void UserMatches_UserContainsNullField_ReturnsTrue() {
			UserDocument user = new UserDocument {
				FirstName = null,
				LastName = "lastname",
				UserName = "username"
			};

			bool userMatches = UserSearchService.UserMatches(user, "last");

			Assert.IsTrue(userMatches);
		}
		#endregion

		#region UserMatchesApplication Tests

		[Test]
		public void UserMatchesApplication_UserBelongsToApplication_ReturnsTrue() {
			string applicationId = "the application";

			UserDocument user = new UserDocument { 
				ApplicationId = "the application"
			};

			bool userMatches = UserSearchService.UserMatchesApplication(user, applicationId);

			Assert.IsTrue(userMatches);
		}

		[Test]
		public void UserMatchesApplication_UserDoesNotBelongToApplication_ReturnsFalse() {
			string applicationId = "the application";

			UserDocument user = new UserDocument {
				ApplicationId = "not the application"
			};

			bool userMatches = UserSearchService.UserMatchesApplication(user, applicationId);

			Assert.IsFalse(userMatches);
		}

		[Test]
		public void UserMatchesApplication_UserApplicationIdIsNull_ReturnsFalse() {
			string applicationId = "the application";

			UserDocument user = new UserDocument {
				ApplicationId = null
			};

			bool userMatches = UserSearchService.UserMatchesApplication(user, applicationId);

			Assert.IsFalse(userMatches);
		}

		#endregion
	}
}
