using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using mPower.OfferingsSystem;
using mPower.OfferingsSystem.Data;

namespace mPower.Tests.UnitTests.OfferingsSystem
{
    [TestFixture]
    public class AccessDataRepsitoryTest
    {
        [Test]
        public void it_loads_brands()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Brand>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,brandIdentifier,brandName,brandDescription,brandLogoName
a29ec92e-caf9-4a74-8451-234552225265,BRN_SYN,18939141,Residence Inn by Marriott,NULL,ResidenceInnARLogo.gif
163d43ad-b4a4-49ed-ba9a-0fe25ec842d9,BRN_SYN,19000985,Filmfury.com,NULL,NULL
b5707471-ff56-41d9-9462-c7f967dd0808,BRN_SYN,19000507,Advanced Window Tinting,NULL,NULL
34230119-5eb8-4d57-9378-53604b29b4f3,BRN_SYN,19020948,Kutting Edge Beauty Barber,NULL,NULL
3ba645a9-b45c-40b5-8769-6207419654af,BRN_SYN,19018785,Ecowater of Moscow,NULL,NULL
9e903eb5-0bc2-4219-8f20-04c7d0a39a55,BRN_SYN,18939331,Subway,NULL,Subway18939331.png")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var brands = repository.GetBrands();
            Assert.AreEqual(6,brands.Count());
            var first = brands.First();
            Assert.AreEqual("a29ec92e-caf9-4a74-8451-234552225265",first.RecordId);
            Assert.AreEqual("Residence Inn by Marriott", first.Name);
            Assert.AreEqual("BRN_SYN", first.RecordType);
            Assert.AreEqual("18939141", first.BrandId);
            Assert.AreEqual("NULL", first.Description);
            Assert.AreEqual("ResidenceInnARLogo.gif", first.LogoName);
         }

        [Test]
        public void it_loads_channels()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Channel>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,channelIdentifier,channelName,channelDescription,channelLogoName
502f33d6-3f68-4a62-84f8-9a6f0393eeba,CHA_SYN,3,Paper Directory,NULL,NULL
1209e112-94a6-4127-a6ca-14fd9d1990ae,CHA_SYN,1,Web Site,NULL,NULL
2275c672-eacc-4cff-896c-14e9728a7aeb,CHA_SYN,2,EMail,NULL,NULL
0462ed7e-0bdc-4e80-80ce-9772f98caf2b,CHA_SYN,4,Paper Mailer,NULL,NULL")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var channels = repository.GetChannels();
            Assert.AreEqual(4, channels.Count());
            var first = channels.First();
            Assert.AreEqual("502f33d6-3f68-4a62-84f8-9a6f0393eeba", first.RecordId);
            Assert.AreEqual("Paper Directory", first.Name);
            Assert.AreEqual("CHA_SYN", first.RecordType);
            Assert.AreEqual("3", first.ChannelId);
            Assert.AreEqual("NULL", first.Description);
            Assert.AreEqual("NULL", first.LogoName);
        }

        [Test]
        public void it_loads_merchants()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Merchant>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,brandIdentifier,locationIdentifier,locationName,phoneNumber,streetLine1,streetLine2,city,state,postalCode,country,latitude,longitude,serviceArea,locationDescription,locationUrl,locationLogoName,locationPhotoNames,keywords
6532ed05-2f9e-4561-a997-e0a09aa8959f,LOC_SYN,18993799,861098,All Around The Town Massage,801-972-3376,1918 W 4100 S,,West Valley City,UT,84119,US,40.6821655,-111.9548354,,NULL,NULL,NULL,,
7e65c7dd-a4c1-42ec-9d59-48b7c13ed63a,LOC_SYN,19020491,721491,Victorian Cottage Treasures,250-428-9395,51 D County Rd,NULL,Porthill,ID,83853,US,48.9819,-116.4785,,NULL,http://www.victoriancottagetreasures.com,NULL,,
6013ca98-bda8-4036-96ff-dafd8579760e,LOC_SYN,19002105,643134,PPM Plumbing Heating Cooling,801-226-3033,Serving Utah County,NULL,Provo,UT,84606,US,40.22542,-111.642,,NULL,http://www.ppmheating.com,NULL,,
85fd2837-fa89-4af0-9d9d-b6fe9151c2d9,LOC_SYN,18990830,901964,Play N Trade,208-733-1804,150 Bullock St,,Chubbuck,ID,83202,US,42.907645,-112.460109,,NULL,NULL,NULL,,
890b2d72-f216-4a41-935c-e21496abfe2e,LOC_SYN,18939000,915658,Papa John's Pizza,208-877-7272,1800 S Meridian Rd,NULL,Meridian,ID,83642,US,43.61141,-116.399,,Better Ingredients. Better Pizza.,http://www.papajohns.com,NULL,,
bada3ec3-3050-478e-bfd8-3e8c306b66c0,LOC_SYN,19005093,5071425,Dixie Optical,435-628-9100,1495 S Blackridge Dr Ste A270,,Saint George,UT,84770,US,37.11424,-113.596,,""The best part of coming to Dixie Optical is the chance to try something new to complement and enhance your face shape, skin, hair and eye color. Like other must-have accessories, it's essential to have a wardrobe of frames that work with a variety of looks. Dixie Optical has the options you need. We carry frames from around the world, including Prada. Eye exams are available. We treat you like you are one-of-a-kind, because you are!"",www.dixie-optical.com,NULL,,
fa034fe7-5e3c-4d55-87fb-067559057721,LOC_SYN,19012427,272595,Eagle Hills Golf Course,208-939-0402,605 N Edgewood Ln,NULL,Eagle,ID,83616,US,43.700891,-116.331961,,""Eagle Hills Golf Course offers 18 championship holes enhanced with rolling hills, sparkling lakes, mature trees and abundant flowers, all set amidst the beautiful Boise mountains. Enjoy memorable holes including Idaho Golf Magazines \one of the most beautiful par threes found anywhere\"" and our signature number five"","" which hosts the popular golf show Closest to the Pin. We offer all the amenities you would expect from a first rate golf course."",http://www.eaglehillsgolfcourse.com,NULL,
977a37f3-034e-415d-80b2-2befb1a14987,LOC_SYN,18938697,5069274,It's A Grind,770-704-8120,104 Prominence Point Pkwy,Ste 100,Canton,GA,30114,US,34.23525,-84.5028,,""It's A Grind features only the highest quality whole bean specialty coffees; traditional, espresso and iced blended coffee drinks; tea and tea based drinks, hot toasted bagels, muffins, scones and other delicious bakery items. It's A Grind begins with the finest Arabica coffees, micro-roasted in small batches to create the smoothest, best tasting coffee. We constantly measures ourself against the finest European coffee masters, assuring its customers only the best tasting coffees."",http://www.itsagrind.com/,ItsAGrindCoffeeHouseARLogo2.gif,,
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var merchant = repository.GetMerchants();
            Assert.AreEqual(8, merchant.Count());
            var first = merchant.First();
            Assert.AreEqual("6532ed05-2f9e-4561-a997-e0a09aa8959f", first.RecordId);
            Assert.AreEqual("All Around The Town Massage", first.LocationName);
            Assert.AreEqual("LOC_SYN", first.RecordType);
            Assert.AreEqual("18993799", first.BrandId);
            Assert.AreEqual("861098", first.LocationId);
            Assert.AreEqual("801-972-3376", first.Phone);
            Assert.AreEqual("1918 W 4100 S", first.StreetLine1);
            Assert.AreEqual("", first.StreetLine2);
            Assert.AreEqual("West Valley City", first.City);
            Assert.AreEqual("UT", first.State);
            Assert.AreEqual("84119", first.PostalCode);
            Assert.AreEqual("US", first.Country);
            Assert.AreEqual(40.6821655, first.Latitude);
            Assert.AreEqual(-111.9548354, first.Longitude);
            Assert.AreEqual("NULL", first.LocationUrl);
            Assert.AreEqual("NULL", first.LocationLogo);
            Assert.AreEqual("NULL", first.LocationDescription);
            Assert.AreEqual("", first.ServiceArea);
            Assert.AreEqual("", first.Keywords);
            Assert.AreEqual("", first.LocationPhotos);
        }

        [Test]
        public void it_loads_subscriptions()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Subscription>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,offerIdentifier,subscriptionIdentifiers
34534302,SUB_SYN,1745208,""845654, 232156, 513652, 786135""")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var subscriptions = repository.GetSubscriptions();
            Assert.AreEqual(1, subscriptions.Count());
            var first = subscriptions.First();
            Assert.AreEqual("34534302", first.RecordId);
            Assert.AreEqual("1745208", first.OfferId);
            Assert.AreEqual("SUB_SYN", first.RecordType);
            Assert.AreEqual("845654, 232156, 513652, 786135", first.SubscriptionIdentifiers);
        }

        [Test]
        public void it_loads_categories()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Category>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"""recordIdentifier"",""recordType"",""categoryIdentifier"",""categoryName"",""categoryDescription"",""categoryLogoName""
""30308622-6b49-48e4-a310-113684f900dd"",""CAT_SYN"",""1009"",""Casual & Fine Dining"",""New sub-category"",""NULL""
""6fa7bb9c-3401-47b0-87ca-6487b12d2ede"",""CAT_SYN"",""44"",""Hotel"",""A fixed (unmodifiable) MAD category."",""NULL""
""a63dfbe8-284c-4d85-bdfb-8713ae9d7467"",""CAT_SYN"",""50"",""Services"",""A fixed (unmodifiable) MAD category."",""NULL""
""53da58d3-3d07-4c7a-b38c-a77199332161"",""CAT_SYN"",""48"",""Golf"",""A fixed (unmodifiable) MAD category."",""NULL""
""cbb91fec-50e3-481b-9aa6-7e82546140f4"",""CAT_SYN"",""45"",""Shopping"",""A fixed (unmodifiable) MAD category."",""NULL""
""a59be8d7-9e93-43ef-ae3f-8acecf961a5d"",""CAT_SYN"",""51"",""Automotive"",""A fixed (unmodifiable) MAD category."",""NULL""
""23ee5a07-db58-40cd-8535-1f8fbc82762b"",""CAT_SYN"",""39"",""Dining & Food"",""A fixed (unmodifiable) MAD category."",""NULL""
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var categories = repository.GetCategories();
            Assert.AreEqual(7, categories.Count());
            var first = categories.First();
            Assert.AreEqual("30308622-6b49-48e4-a310-113684f900dd", first.RecordId);
            Assert.AreEqual("1009", first.CategoryId);
            Assert.AreEqual("CAT_SYN", first.RecordType);
            Assert.AreEqual("Casual & Fine Dining", first.Name);
            Assert.AreEqual("New sub-category", first.Description);
            Assert.AreEqual("NULL", first.LogoName);
        }

        [Test]
        public void it_loads_members()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Member>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,organizationCustomerIdentifier,programCustomerIdentifier,memberCustomerIdentifier,previousMemberCustomerIdentifier,memberStatus,fullName,firstName,middleName,lastName,streetLine1,streetLine2,city,state,postalCode,country,phoneNumber,emailAddress,membershipRenewalDate,productIdentifier,productTemplateField1,productTemplateField2,productTemplateField3,productTemplateField4,productTemplateField5,productRegistrationKey
41889723a301469451e92f08cec3d979,MEM_SYN,33,41,145445,NULL,OPEN,John Doe,John,The,Doe,12356 Pine Avenue,NULL,Plain,TX,84090,US,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL
c8a427acdb3402ca7ca2759051157a0a,MEM_SYN,33,41,2,NULL,OPEN,Jane Doe,Jane,NULL,Doe,12356 Pine Avenue,NULL,Plain,TX,84090,US,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var members = repository.GetMembers();
            Assert.AreEqual(2, members.Count());
            var first = members.First();
            Assert.AreEqual("41889723a301469451e92f08cec3d979", first.RecordId);
            Assert.AreEqual("MEM_SYN", first.RecordType);
            Assert.AreEqual("33", first.OrganizationCustomerId);
            Assert.AreEqual("41", first.ProgramCustomerId);
            Assert.AreEqual("145445", first.MemberCustomerId);
            Assert.AreEqual("NULL", first.PreviousMemberCustomerId);
            Assert.AreEqual("OPEN", first.Status);
            Assert.AreEqual("John Doe", first.FullName);
            Assert.AreEqual("John", first.FirstName);
            Assert.AreEqual("The", first.MiddleName);
            Assert.AreEqual("Doe", first.LastName);
            Assert.AreEqual("12356 Pine Avenue", first.StreetLine1);
            Assert.AreEqual("NULL", first.StreetLine2);
            Assert.AreEqual("Plain", first.City);
            Assert.AreEqual("TX", first.State);
            Assert.AreEqual("84090", first.PostalCode);
            Assert.AreEqual("US", first.Country);
            Assert.AreEqual("NULL", first.Phone);
            Assert.AreEqual("NULL", first.Email);
            Assert.AreEqual("NULL", first.MembershipRenewalDate);
            Assert.AreEqual("NULL", first.ProductId);
            Assert.AreEqual("NULL", first.ProductTemplateField1);
            Assert.AreEqual("NULL", first.ProductTemplateField2);
            Assert.AreEqual("NULL", first.ProductTemplateField3);
            Assert.AreEqual("NULL", first.ProductTemplateField4);
            Assert.AreEqual("NULL", first.ProductTemplateField5);
            Assert.AreEqual("NULL", first.ProductRegistrationKey);
        }

        [Test]
        public void it_loads_mids()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Mid>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,recordStatus,recordStatusMessage,brandIdenti?er,locationIdenti?er,phoneNumber,streetLine1,streetLine2,city,state,postalCode,country,latitude,longitude,startDate,endDate,midValue,midType,processor,acquirer,midAuthFormName,cardIdentifier,lastFour,ttxDatetime,ttxAmount
e38999f4-2796-47e3-a6d2-955297a90f1a,MID_SYN,PENDING,NULL,18993799,161098,,1918 W 4100 S,NULL,West Valley City,UT,84119,US,40.6821655,-111.9548354,20080704,20590504,23783434885,VISA,data entry,NULL,NULL,NULL,NULL,NULL,NULL
14568339-c992-46b6-be57-29b233938051,MID_SYN,PENDING,NULL,19020491,321491,,51 D County Rd,NULL,Porthill,ID,83853,US,48.9819,-116.4785,20090504,20110217,83434556565,VISA,NULL,NULL,NULL,NULL,NULL,NULL,NULL
f9e6ef9a-6592-4c7c-881e-134d8e13459b,MID_VER,PENDING,NULL,19002105,5103450,,Serving Utah County,NULL,Provo,UT,84606,US,40.22542,-111.642,20110428,20610428,3.20921E+13,VISA,NULL,NULL,Foo_Bar_Co_5103450.pdf,1D36DD6E09604776EA49C29E55E1F3AE,1263,20110325-17:42:00,1.02
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var mids = repository.GetMids();
            Assert.AreEqual(3, mids.Count());
            var first = mids.First();
            Assert.AreEqual("e38999f4-2796-47e3-a6d2-955297a90f1a", first.RecordId);
            Assert.AreEqual("MID_SYN", first.RecordType);
            Assert.AreEqual("PENDING", first.Status);
            Assert.AreEqual("NULL", first.StatusMessage);
            Assert.AreEqual("18993799", first.BrandId);
            Assert.AreEqual("161098", first.LocationId);
            Assert.AreEqual("", first.Phone);
            Assert.AreEqual("1918 W 4100 S", first.StreetLine1);
            Assert.AreEqual("NULL", first.StreetLine2);
            Assert.AreEqual("West Valley City", first.City);
            Assert.AreEqual("UT", first.State);
            Assert.AreEqual("84119", first.PostalCode);
            Assert.AreEqual("US", first.Country);
            Assert.AreEqual("40.6821655", first.Latitude);
            Assert.AreEqual("-111.9548354", first.Longitude);
            Assert.AreEqual("20080704", first.StartDate);
            Assert.AreEqual("20590504", first.EndDate);
            Assert.AreEqual("23783434885", first.Value);
            Assert.AreEqual("VISA", first.Type);
            Assert.AreEqual("data entry", first.Processor);
            Assert.AreEqual("NULL", first.Acquirer);
            Assert.AreEqual("NULL", first.AuthFormName);
            Assert.AreEqual("NULL", first.CardId);
            Assert.AreEqual("NULL", first.LastFour);
            Assert.AreEqual("NULL", first.TtxDatetime);
            Assert.AreEqual("NULL", first.TtxAmount);
        }

        [Test]
        public void it_loads_offers()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Offer>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,offerIdentifier,offerDataIdentifier,locationIdentifier,startDate,endDate,categoryIdentifier,offerType,expressionType,award,minimumPurchase,maximumAward,taxRate,tipRate,description,awardRating,dayExclusions,monthExclusions,dateExclusions,redemptions,redemptionPeriod,redeemIdentifiers,terms,disclaimer,offerPhotoNames,keywords
ced96638-dbf0-4d26-b2a2-141f2f45881f,OFR_SYN,700005,89246,194269,20050715,20550715,1009,REWARD,AMOUNT,3,6,0,6,0,$3 award with minimum purchase of $6.  Purchase price before tax rate of 6%.,3,,,,1,YEAR,77588,,""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,
db32df90-444b-4a25-b187-80edd50cb1af,OFR_SYN,704193,265523,532715,20051101,20551101,44,REWARD,AMOUNT,25,149,0,9.6,0,$25 award with minimum purchase of $149.  Purchase price before tax rate of 9.60%.,25,,,,0,YEAR,79271,,""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,
2b2d386b-d36a-497c-9da1-e88a7cd03390,OFR_SYN,704793,273251,643327,20060503,20560503,50,REWARD,AMOUNT,25,200,0,0,0,$25 award with minimum purchase of $200.,25,,,,0,YEAR,79752,,""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,
2703b4ce-6f0a-447e-81ef-99a186f677c9,OFR_SYN,704794,273251,643330,20060503,20560503,50,REWARD,AMOUNT,25,200,0,0,0,$25 award with minimum purchase of $200.,25,,,,0,YEAR,79752,,""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,
55fa0305-edd2-4e43-a434-376f33bc99cb,OFR_SYN,704796,273367,643580,20060520,20560520,48,REWARD,AMOUNT,1,8,0,6.5,0,$1 award with minimum purchase of $8.  Purchase price before tax rate of 6.50%.,1,,,,0,YEAR,78967,,""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,
25c87f58-b63c-4f5b-8b36-38197be400f3,OFR_SYN,724582,261148,376258,20050715,20550715,44,DISCOUNT,PERCENT,50,0,0,0,0,""Access discount rate is $37.99.  That's a savings of at least 50% off the highest published rate! Valid any time, subject to availability. For single king or double queen rooms only."",63,,,,0,YEAR,6039,""Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Void if coupon is altered, transferred, purchased or sold. Use for these purposes is illegal and constitutes fraud. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved. Questions? Contact Access merchant services at 1-888-325-3216."",""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",376258p1.png,
bf948cc2-813a-41e4-8e88-f9ba8379356b,OFR_SYN,705427,280427,583347,20061004,20561004,44,DISCOUNT,PERCENT,50,0,0,0,0,""Access discount rate is $69.99/Sep-Feb, $79.99/Mar-Sep, subject to availability. That's a savings of at least 50% off the highest published rate. Not valid on wknds in Jun & Jul."",63,,,,0,YEAR,5917,Excludes special events & heavy wknds. Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved.   Questions? Contact Access merchant services at 1-888-325-3216.,""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",583347p1.png,
1b8f2b90-0339-412c-a8b0-e8f510bdb2d9,OFR_SYN,701077,143255,248302,20050715,20550715,1009,DISCOUNT,PERCENT,50,0,0,0,0,Free entree with purchase of entree of equal or greater value.,9,,,,0,YEAR,""3,696,271,193"",""Dine-in only, up to $6 value. Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved.   Questions? Contact Access merchant services at 1-888-325-3216."",""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,""American,Bar,Casual Dining,Club,Fast Food,Kid Friendly,Lounge""
98bebda6-cd17-42e3-a476-761e629620ee,OFR_SYN,701176,147030,252082,20050715,20550715,1008,DISCOUNT,PERCENT,50,0,0,0,0,Free 6 donuts with purchase of 6 donuts at regular menu price.,4,,,,0,YEAR,""3,708,571,307"",""Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Void if coupon is altered, transferred, purchased or sold. Use for these purposes is illegal and constitutes fraud. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved. Questions? Contact Access merchant services at 1-888-325-3216."",""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,""American,Bakery,Breakfast,Coffee,Desserts,Donuts,Fast Food,Handicap Accessible,Kid Friendly,Online Ordering,Quick Serve,Take Out""
aec9314d-de9f-4cdb-9264-784013cb080f,OFR_SYN,701245,152290,257343,20050715,20550715,1009,DISCOUNT,PERCENT,10,0,0,0,0,10% off purchase.,2,,,,0,LIFETIME,""931,263,283"",""Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Void if coupon is altered, transferred, purchased or sold. Use for these purposes is illegal and constitutes fraud. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved. Questions? Contact Access merchant services at 1-888-325-3216."",""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,""Casual Dining,Fast Food,Online Ordering,Pizza,Take Out""
05643288-195b-4230-8708-14fb8da0ea7e,OFR_SYN,701256,153408,258462,20050715,20550715,1008,DISCOUNT,PERCENT,10,0,0,0,0,10% off total purchase.,1,,,,0,YEAR,""2,533,967,674"",""Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Void if coupon is altered, transferred, purchased or sold. Use for these purposes is illegal and constitutes fraud. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved. Questions? Contact Access merchant services at 1-888-325-3216."",""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,""Delivery,Fast Food,Handicap Accessible,Kid Friendly,Online Ordering,Pizza,Quick Serve,Take Out""
ddc99a67-8574-4e87-bbd4-2a55f6248f2a,OFR_SYN,701288,158418,263480,20050715,20550715,59,DISCOUNT,PERCENT,20,0,0,0,0,25% off IMAX show.,15,,,,0,YEAR,""3,671,770,960"",""Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Void if coupon is altered, transferred, purchased or sold. Use for these purposes is illegal and constitutes fraud. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved. Questions? Contact Access merchant services at 1-888-325-3216."",""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",,
28298f02-3904-499d-bd75-d762f3e68e49,OFR_SYN,701320,162993,271565,20050715,20550715,48,DISCOUNT,PERCENT,30,0,0,0,0,""Free required cart with paid greens fee, Mon-Sat any time, after 2pm Sun & hldys, & 2-for-1 bucket of balls."",14,,,,0,YEAR,82861,""Powered by Access. Valid only at the location listed. Only one offer may be redeemed per visit. Offer may not be valid with other sales/promotions. Void if coupon is altered, transferred, purchased or sold. Use for these purposes is illegal and constitutes fraud. Other restrictions may apply. Offers are subject to change without notice. No cash value. © Access Development. All rights reserved. Questions? Contact Access merchant services at 1-888-325-3216."",""Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. "",271565p1.png,
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var offers = repository.GetOffers();
            Assert.AreEqual(13, offers.Count());
            var first = offers.First();
            Assert.AreEqual("ced96638-dbf0-4d26-b2a2-141f2f45881f", first.RecordId);
            Assert.AreEqual("OFR_SYN", first.RecordType);
            Assert.AreEqual("700005", first.OfferId);
            Assert.AreEqual("89246", first.OfferDataId);
            Assert.AreEqual("194269", first.LocationId);
            Assert.AreEqual("20050715", first.StartDate);
            Assert.AreEqual("20550715", first.EndDate);
            Assert.AreEqual("1009", first.CategoryId);
            Assert.AreEqual("REWARD", first.OfferType);
            Assert.AreEqual("AMOUNT", first.ExpressionType);
            Assert.AreEqual("3", first.Award);
            Assert.AreEqual("6", first.MinimumPurchase);
            Assert.AreEqual("0", first.MaximumAward);
            Assert.AreEqual("6", first.TaxRate);
            Assert.AreEqual("0", first.TipRate);
            Assert.AreEqual("$3 award with minimum purchase of $6.  Purchase price before tax rate of 6%.", 
                first.Desciption);
            Assert.AreEqual("3", first.AwardRating);
            Assert.AreEqual("", first.DayExclusions);
            Assert.AreEqual("", first.MonthExclusions);
            Assert.AreEqual("", first.DateExclusions);
            Assert.AreEqual("1", first.Redemptions);
            Assert.AreEqual("YEAR", first.RedemptionPeriod);
            Assert.AreEqual("77588", first.RedeemIdentifiers);
            Assert.AreEqual("", first.Terms);
            Assert.AreEqual("Disclaimer: Through your access to and use of this content, you agree to be bound by these terms and conditions. We have made every effort to provide quality products and services. The products listed here have been made available by the provider listed. As such, we have no control over the quality of the products or services rendered. We shall not be liable for injury, damage, loss, delay or any other claim which may arise from the actions or omissions of any business, the employees of said business, or other individuals providing products or other related services referenced herein. We do not warrant the products provided or the services performed. There are no warranties made by us, express or implied, including, but not limited to, any warranties of merchantability or fitness for any particular purpose. To the extent that the offer described here differs from the offer authorized by the provider, we reserve the right to correct the offer. You agree to use only the corrected offer.  Our distribution channels and the products listed thereon are subject to change without notice. Merchants welcome your patronage subject to the terms indicated above. ",
                first.Disclaimer);
            Assert.AreEqual("", first.OfferPhotoNames);
            Assert.AreEqual("", first.Keywords);
        }

        [Test]
        public void it_loads_products()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Product>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,organizationCustomerIdentifer,programCustomerIdentifer,productIdentifer,productName,productDescription,productStartDate,productEndDate
a29ec92e-caf9-4a74-8451-234552225265,PRO_SYN,18939141,46395560,53654324,Member Welcome Kit,Kit 137,NULL,NULL
163d43ad-b4a4-49ed-ba9a-0fe25ec842d9,PRO_SYN,19000985,46395711,53654324,Member Welcome Kit,Kit 137,NULL,NULL
b5707471-ff56-41d9-9462-c7f967dd0808,PRO_SYN,19000507,46398005,53654324,Member Welcome Kit,Kit 137,NULL,NULL
34230119-5eb8-4d57-9378-53604b29b4f3,PRO_SYN,19020948,46399314,53667832,My SuperSavings Program Welcome Kit,Kit 435,NULL,NULL
3ba645a9-b45c-40b5-8769-6207419654af,PRO_SYN,19018785,46412365,53667832,My SuperSavings Program Welcome Kit,Kit 435,NULL,NULL
9e903eb5-0bc2-4219-8f20-04c7d0a39a55,PRO_SYN,18939331,46426576,53667832,My SuperSavings Program Welcome Kit,Kit 435,NULL,NULL
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var products = repository.GetProducts();
            Assert.AreEqual(6, products.Count());
            var first = products.First();
            Assert.AreEqual("a29ec92e-caf9-4a74-8451-234552225265", first.RecordId);
            Assert.AreEqual("PRO_SYN", first.RecordType);
            Assert.AreEqual("18939141", first.OrganizationCustomerId);
            Assert.AreEqual("46395560", first.ProgrammCustomerId);
            Assert.AreEqual("53654324", first.ProductId);
            Assert.AreEqual("Member Welcome Kit", first.Name);
            Assert.AreEqual("Kit 137", first.Description);
            Assert.AreEqual("NULL", first.StartDate);
            Assert.AreEqual("NULL", first.EndDate);
        }

        [Test]
        public void it_loads_redeems()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Redeem>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"""recordIdentifier"",""recordType"",""redeemIdentifier"",""publicationChannels"",""redeemMethod"",""redeemInstruction"",""redeemCode"",""redeemCouponName""
""34c1bc86-6c77-446f-a8d9-6f2e6319ab12"",""RED_SYN"",""77588"",""0001,0003"",""INSTORE_PAYMENT_CARD"",""Pay with your Eligible Card, sign for your purchase, and your rewards will be credited to your account within 90 days."",""NULL"",""NULL""
""9d479121-6f3a-4d80-81a5-c9f31f40baa4"",""RED_SYN"",""79271"",""0001,0003"",""INSTORE_PAYMENT_CARD"",""Pay with your Eligible Card, sign for your purchase, and your rewards will be credited to your account within 90 days."",""NULL"",""NULL""
""9a9da11f-9cfd-45a3-8abc-24c22d322e63"",""RED_SYN"",""162"",""0001"",""INSTORE_PRINT_COUPON"",""To redeem this offer instore, show this screen to the cashier prior to payment."",""NULL"",""coupon/generated_4042090.pdf""
""16b0ee38-12a1-4caf-9813-faf739cac0c0"",""RED_SYN"",""177"",""0001"",""INSTORE_PRINT_COUPON"",""To redeem this offer instore, print coupon and bring into your nearest participating location. "",""500150002000"",""coupon/generated_4042100.pdf""
""8ebfb505-a788-4627-b8b4-0744c08b9e08"",""RED_SYN"",""310"",""0001,0003"",""INSTORE_PRINT_COUPON"",""Mention code: <b>AKC20</b> at checkout."",""AKC20"",""coupon/generated_4042868.pdf""
""50150037-0483-4fd7-868a-f34b7a2ffe20"",""RED_SYN"",""312"",""0001,0003"",""INSTORE_PRINT_COUPON"",""Present printed offer or mobile device displaying the special offer at the box office."",""Access"",""coupon/generated_4040562.pdf""
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var redeems = repository.GetRedeems();
            Assert.AreEqual(6, redeems.Count());
            var first = redeems.First();
            Assert.AreEqual("34c1bc86-6c77-446f-a8d9-6f2e6319ab12", first.RecordId);
            Assert.AreEqual("RED_SYN", first.RecordType);
            Assert.AreEqual("77588", first.RedeemId);
            Assert.AreEqual("0001,0003", first.PublicationChannels);
            Assert.AreEqual("INSTORE_PAYMENT_CARD", first.Method);
            Assert.AreEqual("Pay with your Eligible Card, sign for your purchase, and your rewards will be credited to your account within 90 days.", 
                first.Instruction);
            Assert.AreEqual("NULL", first.Code);
            Assert.AreEqual("NULL", first.CouponName);
        }

        [Test]
        public void it_loads_statements()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Statement>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,organizationCustomerIdentifier,programCustomerIdentifier,cardIdentifier,settlementIdentifier,statementIdentifier,settlementStatus,productIdentifier,transactionIdentifier,transactionStatus,transactionDatetime,transactionGross,transactionNet,transactionTax,transactionTip,transactionReward
4486193,STM,33,40,5308D323BB3721E0B1ED80EEEFB571ED,13210,3015152,PAID,57,255004775,QTX,20110328-07:00:00,5.57,5.57,0,0,0.56
4501266,STM,33,40,C4447603BFF214A9DB9BFCF732F1280E,13210,3015762,PAID,63,260493576,QTX,20110422-06:00:00,22.95,22.95,0,0,2.3
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var statements = repository.GetStatements();
            Assert.AreEqual(2, statements.Count());
            var first = statements.First();
            Assert.AreEqual("4486193", first.RecordId);
            Assert.AreEqual("STM", first.RecordType);
            Assert.AreEqual("33", first.OrganizationCustomerId);
            Assert.AreEqual("40", first.ProgrammCustomerId);
            Assert.AreEqual("5308D323BB3721E0B1ED80EEEFB571ED", first.CardId);
            Assert.AreEqual("13210", first.SettlementId);
            Assert.AreEqual("3015152", first.StatementId);
            Assert.AreEqual("PAID", first.SettlementStatus);
            Assert.AreEqual("57", first.ProductId);
            Assert.AreEqual("255004775", first.TransactionId);
            Assert.AreEqual("QTX", first.TransactionStatus);
            Assert.AreEqual("20110328-07:00:00", first.TransactionDatetime);
            Assert.AreEqual("5.57", first.TransactionGross);
            Assert.AreEqual("5.57", first.TransactionNet);
            Assert.AreEqual("0", first.TransactionTax);
            Assert.AreEqual("0", first.TransactionTip);
            Assert.AreEqual("0.56", first.TransactionReward);
        }

        [Test]
        public void it_loads_statuses()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Status>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,fileName,lineNumber,originalRecordIdentifier,originalRecordType,recordStatus,recordStatusMessage
70d8ac65-cfee-43a6-9d7e-d3139ce54872,STA_FIL,EX-AD-1780-20110503-163546-MEMBER.csv.pgp,459,NULL,NULL,SUCCESS,NULL
c51aa0a0-4db0-40df-83bb-b2ad4a4e0d3f,STA_REC,EX-AD-1780-20110503-163548-MEMBER.csv.pgp,6,00010335701029646:40e07010d0,MEM_CID,INVALID_RECORD_FORMAT,member with previousMemberIdentifier does not exist: 35701029646
643eb59f-8e9b-414c-b0f3-8e6d9073ca1b,STA_FIL,EX-AD-1780-20110503-163548-MEMBER.csv.pgp,21,NULL,NULL,SUCCESS,NULL
f4545d68-7da3-4927-a549-ba5f19fd085e,STA_REC,EX-AD-1780-20110503-163551-CARD.csv.pgp,2,000000001:d0e040c0f0,CRD_RMM,INVALID_RECORD_FORMAT,member is not associated with card
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var statuses = repository.GetStatuses();
            Assert.AreEqual(4, statuses.Count());
            var first = statuses.First();
            Assert.AreEqual("70d8ac65-cfee-43a6-9d7e-d3139ce54872", first.RecordId);
            Assert.AreEqual("STA_FIL", first.RecordType);
            Assert.AreEqual("EX-AD-1780-20110503-163546-MEMBER.csv.pgp", first.FileName);
            Assert.AreEqual("459", first.LineNumber);
            Assert.AreEqual("NULL", first.OriginalRecordId);
            Assert.AreEqual("NULL", first.OriginalRecordType);
            Assert.AreEqual("SUCCESS", first.RecordStatus);
            Assert.AreEqual("NULL", first.RecordStatusMessage);
        }

        [Test]
        public void it_loads_cards()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Card>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,organizationCustomerIdentifier,programCustomerIdentifier,memberCustomerIdentifier,cardIdentifier,cardStatus,cardBrand,cardType,nameOnCard,lastFour,expirationDate
384f73ab036000e9c3a53b330798d090,CRD_SYN,33,41,7445,0181C86B145BF3FF43E32745DBBF04EDD12D226D,OPEN,DISCOVER_CARD,BUSINESS_DEBIT,NULL,1134,NULL
d4ffdf71681085cbfd6823c0e78d7990,CRD_SYN,33,41,2356,46F86BCA83F5EFB5BA6616A6DC26A5F10D2603F4,OPEN,VISA,CREDIT,NULL,1676,914
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var cards = repository.GetCards();
            Assert.AreEqual(2, cards.Count());
            var first = cards.First();
            Assert.AreEqual("384f73ab036000e9c3a53b330798d090", first.RecordId);
            Assert.AreEqual("CRD_SYN", first.RecordType);
            Assert.AreEqual("33", first.OrganizationCustomerId);
            Assert.AreEqual("41", first.ProgrammCustomerId);
            Assert.AreEqual("7445", first.MemberCustomerId);
            Assert.AreEqual("0181C86B145BF3FF43E32745DBBF04EDD12D226D", first.CardId);
            Assert.AreEqual("OPEN", first.Status);
            Assert.AreEqual("DISCOVER_CARD", first.Brand);
            Assert.AreEqual("BUSINESS_DEBIT", first.Type);
            Assert.AreEqual("NULL", first.Name);
            Assert.AreEqual("1134", first.LastFour);
            Assert.AreEqual("NULL", first.ExpirationDate);
        }

        [Test]
        public void it_loads_transaction()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Transaction>()).Returns(() =>
                new List<TextReader>{
                new StringReader(
@"recordIdentifier,recordType,recordStatus,recordStatusMessage,organizationCustomerIdentifier,programCustomerIdentifier,memberCustomerIdentifier,cardIdentifier,midValue,transactionIdentifier,transactionDatetime,transactionGross,authorizationCode,transactionNet,transactionTax,transactionTip,transactionReward,transactionStatus,transactionCode,offerIdentifier
023885bd31476a4f6c3287516c60fb8c,TXN,PENDING,NULL,33,41,6335,5308D323BB3721E0B1ED80EEEFB571ED,3.57658E+16,dda22ffb23d87beb,20110309-22:35:52,44.21,f0bae81e,NULL,NULL,NULL,NULL,NULL,NULL,NULL
47fbb555e2beecf561729e7c36089417,TXN,PENDING,NULL,33,41,23234,C4447603BFF214A9DB9BFCF732F1280E,1.2032E+16,5a41c6d9eb40f16c,20110219-22:35:52,5.28,99a89c5f,NULL,NULL,NULL,NULL,NULL,NULL,NULL
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var transactions = repository.GetTransactions();
            Assert.AreEqual(2, transactions.Count());
            var first = transactions.First();
            Assert.AreEqual("023885bd31476a4f6c3287516c60fb8c", first.RecordId);
            Assert.AreEqual("TXN", first.RecordType);
            Assert.AreEqual("PENDING", first.RecordStatus);
            Assert.AreEqual("NULL", first.RecordStatusMessage);
            Assert.AreEqual("33", first.OrganizationCustomerId);
            Assert.AreEqual("41", first.ProgrammCustomerId);
            Assert.AreEqual("6335", first.MemberCustomerId);
            Assert.AreEqual("5308D323BB3721E0B1ED80EEEFB571ED", first.CardId);
            Assert.AreEqual("3.57658E+16", first.MidValue);
            Assert.AreEqual("dda22ffb23d87beb", first.TransactionId);
            Assert.AreEqual("20110309-22:35:52", first.TransactionDatetime);
            Assert.AreEqual("44.21", first.TransactionGross);
            Assert.AreEqual("f0bae81e", first.AuthorizationCode);
            Assert.AreEqual("NULL", first.TransactionNet);
            Assert.AreEqual("NULL", first.TransactionTax);
            Assert.AreEqual("NULL", first.TransactionTip);
            Assert.AreEqual("NULL", first.TransactionReward);
            Assert.AreEqual("NULL", first.TransactionStatus);
            Assert.AreEqual("NULL", first.TransactionCode);
            Assert.AreEqual("NULL", first.OfferIdentifier);
        }

        [Test]
        public void it_loads_usages()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Usage>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,organizationCustomerIdentifier,programCustomerIdentifier,memberCustomerIdentifier,eventDatetime,brandIdentifier,locationIdentifier,offerIdentifier,offerDataIdentifier,publicationChannel,action,description
d35dc85d20f1d124ebe56204d10a45cf,USE_SYN,33,41,7234,20110429-21:35:51,23573,45413,76353,343134,1,REDEEM,NULL
2b792a56dfaea41c306bab4aa99da87d,USE_SYN,33,41,2534,20110424-21:35:51,91043,1408366,61558,567042,3,SEARCH,Offer search
0203817d66928000456e4eef2294873e,USE_SYN,33,41,9234,20110128-22:35:51,686981,289466,75605,78924051,2,IMPRESSION,NULL
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var usages = repository.GetUsages();
            Assert.AreEqual(3, usages.Count());
            var first = usages.First();
            Assert.AreEqual("d35dc85d20f1d124ebe56204d10a45cf", first.RecordId);
            Assert.AreEqual("USE_SYN", first.RecordType);
            Assert.AreEqual("33", first.OrganizationCustomerId);
            Assert.AreEqual("41", first.ProgrammCustomerId);
            Assert.AreEqual("7234", first.MemberCustomerId);
            Assert.AreEqual("20110429-21:35:51", first.EventDatetime);
            Assert.AreEqual("23573", first.BrandId);
            Assert.AreEqual("45413", first.LocationId);
            Assert.AreEqual("76353", first.OfferId);
            Assert.AreEqual("343134", first.OfferDataId);
            Assert.AreEqual("1", first.PublicationChannel);
            Assert.AreEqual("REDEEM", first.Action);
            Assert.AreEqual("NULL", first.Description);
        }

        [Test]
        public void it_loads_settlements()
        {
            var fileLoader = new Mock<IFileLoader>();
            fileLoader.Setup(x => x.LoadFor<Settlement>()).Returns(() =>
                new List<TextReader>{
                     new StringReader(
@"recordIdentifier,recordType,organizationCustomerIdentifier,programCustomerIdentifier,settlementIdentifier,settlementStartDate,settlementEndDate,totalRewards,totalFunded
34534302,SET,33,40,13210,20110401,20110401,1538.94,948.82
")});
            var repository = new AccessDataRepsitory(fileLoader.Object);
            var settlements = repository.GetSettlements();
            Assert.AreEqual(1, settlements.Count());
            var first = settlements.First();
            Assert.AreEqual("34534302", first.RecordId);
            Assert.AreEqual("SET", first.RecordType);
            Assert.AreEqual("33", first.OrganizationCustomerId);
            Assert.AreEqual("40", first.ProgrammCustomerId);
            Assert.AreEqual("13210", first.SettlementId);
            Assert.AreEqual("20110401", first.StartDate);
            Assert.AreEqual("20110401", first.EndDate);
            Assert.AreEqual("1538.94", first.TotalRewards);
            Assert.AreEqual("948.82", first.TotalFunded);
        }
    }
}