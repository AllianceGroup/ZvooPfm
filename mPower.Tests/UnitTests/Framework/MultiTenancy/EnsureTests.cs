using System;
using mPower.Environment.MultiTenancy;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Framework.MultiTenancy
{
    public class EnsureTests
    {
        [Test]
        public void Ensure_That_DoesNotThrowException_WhenExpressionTrue()
        {
            Assert.DoesNotThrow(() => Ensure.That(true));
        }

        [Test]
        public void Ensure_That_ThrowsException_WhenExpressionFalse()
        {
            Assert.Throws<Exception>(() => Ensure.That(false));
        }

        [Test]
        public void Ensure_That_WithFalseConditionAndMessage_ThrowsExceptionWithMessage()
        {
            string message = "%";
            try
            { 
                Ensure.That(false, message);
                Assert.True(false, "The Ensure.That exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_That_WithGenericExceptionTypeAndFalseCondition_ThrowsSpecificException()
        {
            Assert.Throws<InvalidTimeZoneException>(() => Ensure.That<InvalidTimeZoneException>(false));
        }

        [Test]
        public void Ensure_That_WithGenericExceptionTypeAndTrueCondition_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.That<InvalidTimeZoneException>(true));
        }

        [Test]
        public void Ensure_That_WithFalseConditionAndMessageAndGenericException_ThrowsExceptionWithMessage()
        {
            string message = "%";
            try
            {
                Ensure.That<InvalidTimeZoneException>(false, message);
                Assert.True(false, "The Ensure.That exception was not thrown");
            }
            catch (InvalidTimeZoneException ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
            catch (Exception)
            {
                Assert.True(false, "The Ensure.That exception was not correct type");
            }
        }

        [Test]
        public void Ensure_Not_FalseCondition_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.Not<Exception>(false));
        }

        [Test]
        public void Ensure_Not_TrueCondition_ThrowsSpecifiedException()
        {
            Assert.Throws<InvalidTimeZoneException>(() => Ensure.Not<InvalidTimeZoneException>(true));
        }

        [Test]
        public void Ensure_Not_WithTrueCondition_AndMessage_AndGenericException_ThrowsExceptionWithMessage()
        {
            string message = "%";
            try
            {
                Ensure.Not<InvalidTimeZoneException>(true, message);
                Assert.True(false, "The Ensure.Not exception was not thrown");
            }
            catch (InvalidTimeZoneException ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
            catch (Exception)
            {
                Assert.True(false, "The Ensure.Not exception was not correct type");
            }
        }

        [Test]
        public void Ensure_Not_NonGeneric_WithFalseCondition_AndNoMessase_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.Not(false));
        }

        [Test]
        public void Ensure_Not_NonGeneric_WithFalseCondition_AndMessase_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.Not(false, "message"));
        }

        [Test]
        public void Ensure_Not_NonGeneric_WithTrueCondition_AndNoMessage_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.Not(true));
        }

        [Test]
        public void Ensure_Not_NonGeneric_WithTrueValueAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {
                Ensure.Not(true, message);
                Assert.True(false, "The Ensure.Not exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_NotNull_WithNonNullObject_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.NotNull(new object()));
        }

        [Test]
        public void Ensure_NotNull_WithNullObject_ThrowNullReferenceExceptionException()
        {
            Assert.Throws<NullReferenceException>(() => Ensure.NotNull(null));
        }

        [Test]
        public void Ensure_NotNull_WithNullValueAndMessage_ThrowsNullReferenceExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {   
                Ensure.NotNull(null, message);
                Assert.True(false, "The Ensure.NotNull exception was not thrown");
            }
            catch (NullReferenceException ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
            catch (Exception)
            {
                Assert.True(false, "The Ensure.NotNull exception was not correct type");
            }
        }

        [Test]
        public void Ensure_Equal_WithEqualValues_DoesNotThrowException()
        { 
            string value = "%";
            Assert.DoesNotThrow(() => Ensure.Equal(value, value));
        }

        [Test]
        public void Ensure_Equal_WithEqualValues_AndMessage_DoesNotThrowException()
        {
            string value = "%";
            Assert.DoesNotThrow(() => Ensure.Equal(value, value, "message"));
        }

        [Test]
        public void Ensure_Equal_WithNonEqualValues_ThrowsException()
        {
            string value1 = "%", value2 = "$";
            Assert.Throws<Exception>(() => Ensure.Equal(value1, value2));
        }

        [Test]
        public void Ensure_Equal_WithNonEqualValuesAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {
                Ensure.Equal("a", "b", message);
                Assert.True(false, "The Ensure.Equal exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_Equal_GivenNullLeftValue_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.Equal<string>(null, "%"));
        }

        [Test]
        public void Ensure_Equal_GivenNullRightValue_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.Equal<string>("^", null));
        }

        [Test]
        public void Ensure_NotEqual_GivenEqualValues_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.NotEqual("$", "$"));
        }

        [Test]
        public void Ensure_NotEqual_GivenNonEqualValues_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.NotEqual("$", "%"));
        }

        [Test]
        public void Ensure_NotEqual_GivenNullLeftValues_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.NotEqual<string>(null, "#"));
        }

        [Test]
        public void Ensure_NotEqual_GivenNullRightValues_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.NotEqual<string>("#", null));
        }

        [Test]
        public void Ensure_NotEqual_WithEqualValuesAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {
                Ensure.NotEqual("a", "a", message);
                Assert.True(false, "The Ensure.NotEqual exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_Contains_GivenNullCollection_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.Contains<string>(null, x => true));
        }

        [Test]
        public void Ensure_Contains_GivenBlankCollection_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.Contains<string>(new string[0], x => true));
        }

        [Test]
        public void Ensure_Contains_GivenCollectionWithoutSatisfyingValue_ThrowsException()
        {
            Assert.Throws<Exception>(() => Ensure.Contains<string>(new string[] { "hello", "world" }, x => false));
        }

        [Test]
        public void Ensure_Contains_GivenCollectionWithSatisfyingValue_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.Contains<string>(new string[] { "hello", "world" }, x => true));
        }

        [Test]
        public void Ensure_Contains_WithEmptyCollectionAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {
                Ensure.Contains<string>(null, x => true, message);
                Assert.True(false, "The Ensure.Contains exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_NotNullOrEmpty_DoesNotThrowException_GivenNonEmptyString()
        {
            Assert.DoesNotThrow(() => Ensure.NotNullOrEmpty("a"));
        }

        [Test]
        public void Ensure_NotNullOrEmpty_ThrowsException_GivenEmptyString()
        {
            Assert.Throws<Exception>(() => Ensure.NotNullOrEmpty(""));
        }

        [Test]
        public void Ensure_NotNullOrEmpty_ThrowsException_GivenNullString()
        {
            Assert.Throws<Exception>(() => Ensure.NotNullOrEmpty(""));
        }

        [Test]
        public void Ensure_NotNullOfEmpty_GivenNullStringAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {
                Ensure.NotNullOrEmpty(null, message);
                Assert.True(false, "The Ensure.NotNullOrEmpty exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_Argument_Is_GivenFalseCondition_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Ensure.Argument.Is(false));
        }

        [Test]
        public void Ensure_Argument_Is_GivenTrueCondition_DoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => Ensure.Argument.Is(true));
        }

        [Test]
        public void Ensure_Argument_Is_GivenFalseConditionAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {
                Ensure.Argument.Is(false, message);
                Assert.True(false, "The Ensure.Argument.Is exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_Argument_IsNot_GivenTrueCondition_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Ensure.Argument.IsNot(true));
        }

        [Test]
        public void Ensure_Argument_Is_GivenFalseCondition_DoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => Ensure.Argument.IsNot(false));
        }

        [Test]
        public void Ensure_Argument_IsNot_GivenTrueConditionAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string message = "%";
            try
            {
                Ensure.Argument.IsNot(true, message);
                Assert.True(false, "The Ensure.Argument.IsNot exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }

        [Test]
        public void Ensure_Argument_IsNot_GivenNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.Argument.NotNull(null));
        }

        [Test]
        public void Ensure_Argument_Is_GivenNonNullCondition_DoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => Ensure.Argument.NotNull(false));
        }

        [Test]
        public void Ensure_Argument_IsNot_GivenNullValueAndMessage_ThrowsExceptionWithCorrectMessage()
        {
            string param = "%";
            try
            {
                Ensure.Argument.NotNull(null, param);
                Assert.True(false, "The Ensure.Argument.NotNull exception was not thrown");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual(param, ex.ParamName);
            }
        }

        [Test]
        public void Ensure_Argument_NotNullOrEmpty_GivenNonEmptyString_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => Ensure.Argument.NotNullOrEmpty("s"));
        }

        [Test]
        public void Ensure_Argument_NotNullOrEmpty_ThrowsArgumentException_WhenGivenNullSting()
        {
            Assert.Throws<ArgumentException>(() => Ensure.Argument.NotNullOrEmpty(null));
        }

        [Test]
        public void Ensure_Argument_NotNullOrEmpty_ThrowsArgumentException_WhenGivenEmptySting()
        {
            Assert.Throws<ArgumentException>(() => Ensure.Argument.NotNullOrEmpty(""));
        }

        [Test]
        public void Ensure_Argument_NotNullOrEmpty_ThrowsArgumentException_WhenGivenNullSting_AndParameterName()
        {
            Assert.Throws<ArgumentException>(() => Ensure.Argument.NotNullOrEmpty(null, "param"));
        }

        [Test]
        public void Ensure_Argument_NotNullOrEmpty_ThrowsArgumentException_WhenGivenEmptySting_AndParameterName()
        {
            Assert.Throws<ArgumentException>(() => Ensure.Argument.NotNullOrEmpty("", "param"));
        }

        [Test]
        public void Ensure_Argument_InFuture_DoesntThrowException_WhenGivenDateTimeInFuture()
        {
            Assert.DoesNotThrow(() => Ensure.Argument.InFuture(DateTime.Now.AddMinutes(1)));
        }

        [Test]
        public void Ensure_Argument_InFuture_ThrowsException_WhenGivenDateTimeInPast()
        {
            Assert.Throws<ArgumentException>(() => Ensure.Argument.InFuture(DateTime.Now.AddMinutes(-1)));
        }
        
    }
}