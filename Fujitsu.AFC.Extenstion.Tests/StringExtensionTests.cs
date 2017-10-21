using Fujitsu.AFC.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace Fujitsu.AFC.Extenstion.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StringExtensionTests
    {
        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsInt()
        {
            const string source = "1";
            var converted = source.ConvertTo<int>();
            Assert.AreEqual(1, converted);
        }

        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsString()
        {
            const string source = "X";
            var converted = source.ConvertTo<string>();
            Assert.AreEqual("X", converted);
        }

        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsDateTime()
        {
            const string source = "2013/10/02";
            var converted = source.ConvertTo<DateTime>();
            Assert.AreEqual(new DateTime(2013, 10, 2), converted);
        }

        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsDateTimeForAmericanTypeDate()
        {
            const string source = "2013/10/12";
            var converted = source.ConvertTo<DateTime>();
            Assert.AreEqual(new DateTime(2013, 10, 12), converted);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void StringExtension_ConvertTo_ReturnsExceptionForInt()
        {
            const string source = "X";
            var converted = source.ConvertTo<int>();
        }

        [TestMethod]
        public void StringExtension_ConvertStringToGenericValue_StringIsNullTypeIsInt_ReturnsDefaultIntWhichIsZero()
        {
            var s = null as string;
            Assert.AreEqual(0, s.ConvertStringToGenericValue<int>());
        }

        [TestMethod]
        public void StringExtension_ConvertStringToGenericValue_StringIsNullTypeIsDateTime_ReturnsDateCorrectlyParsed()
        {
            const string s = "06/12/2014 13:12:12";
            var actual = s.ConvertStringToGenericValue<DateTime>();
            Assert.AreEqual(2014, actual.Year);
            Assert.AreEqual(12, actual.Month);
            Assert.AreEqual(6, actual.Day);
            Assert.AreEqual(13, actual.Hour);
            Assert.AreEqual(12, actual.Minute);
            Assert.AreEqual(12, actual.Second);
        }

        [TestMethod]
        public void StringExtension_ConvertStringToGenericValue_StringIsNullTypeIsDecimal_ReturnsDecimal()
        {
            const string s = "12.55";
            var actual = s.ConvertStringToGenericValue<Decimal>();
            Assert.AreEqual(12.55m, actual);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtension_Constructor_CropWholeWords_NoValue_ThrowsException()
        {
            string test = null;
            test.CropWholeWords(10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StringExtension_Constructor_CropWholeWords_NegativeLength_ThrowsException()
        {
            const string test = "Matthew Jordan";
            test.CropWholeWords(-1);
        }

        [TestMethod]
        public void StringExtension_CropWholeWords_CropsCorrectly()
        {
            const string s = "Matt Jordan Says Hello";
            var crop = s.CropWholeWords(10);
            Assert.AreEqual("Matt", crop);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtension_Constructor_CropWholeWordsIntoChunks_NoValue_ThrowsException()
        {
            string test = null;
            test.CropWholeWordsIntoChunks(1, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StringExtension_Constructor_CropWholeWordsIntoChunks_NegativeLength_ThrowsException()
        {
            const string test = "Matthew Jordan";
            test.CropWholeWordsIntoChunks(10, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StringExtension_Constructor_CropWholeWordsIntoChunks_ChunkSizeLessThanOne_ThrowsException()
        {
            const string test = "Matthew Jordan";
            test.CropWholeWordsIntoChunks(0, 10);
        }

        [TestMethod]
        public void StringExtension_CropWholeWordsIntoChunks_ChunksAndCropsTwice()
        {
            const string s = "Matt Jordan";
            var dictionary = s.CropWholeWordsIntoChunks(2, 10);
            Assert.AreEqual(dictionary.Count, 2);
            Assert.AreEqual("Matt", dictionary[1]);
            Assert.AreEqual("Jordan", dictionary[2]);
        }

        [TestMethod]
        public void StringExtension_CropWholeWordsIntoChunks_ChunksAndCropsThreeTimesWithEmptyEntry()
        {
            const string s = "Matt Jordan";
            var dictionary = s.CropWholeWordsIntoChunks(3, 10);
            Assert.AreEqual(dictionary.Count, 3);
            Assert.AreEqual("Matt", dictionary[1]);
            Assert.AreEqual("Jordan", dictionary[2]);
            Assert.AreEqual(String.Empty, dictionary[3]);
        }

        [TestMethod]
        public void
            StringExtension_CropWholeWordsIntoChunks_ChunksAndCropsCorrectlyWhenFirstWordInChunkIsGreaterThanLength()
        {
            const string s = "Matt Jordan";
            var dictionary = s.CropWholeWordsIntoChunks(3, 4);
            Assert.AreEqual(dictionary.Count, 3);
            Assert.AreEqual("Matt", dictionary[1]);
            Assert.AreEqual(String.Empty, dictionary[2]);
            Assert.AreEqual(String.Empty, dictionary[3]);
        }


        [TestMethod]
        public void StringExtension_SafeEquals_NullComparisonNoExceptionIsThrownResultIsFalse()
        {
            const string test = "Test";
            string target = null;

            var result = target.SafeEquals(test);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StringExtension_SafeEquals_IdenticalStringsResultIsTrue()
        {
            const string test = "Test";
            const string target = "Test";

            var result = target.SafeEquals(test);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringExtension_SafeEquals_IdenticalStringsCaseIgnoredResultIsTrue()
        {
            const string test = "Test";
            const string target = "TEST";

            var result = target.SafeEquals(test);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void StringExtension_SafeContains_NullNoExceptionIsThrownResultIsFalse()
        {
            const string test = "Test";
            string target = null;

            var result = target.SafeContains(test);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StringExtension_SafeContains_StringLocatedResultIsTrue()
        {
            const string test = "quick";
            const string target = "The quick brown fox.";

            var result = target.SafeContains(test);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringExtension_SafeContains_StringLocatedCaseIgnoredResultIsTrue()
        {
            const string test = "QUICK";
            const string target = "The quick brown fox.";

            var result = target.SafeContains(test);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringExtension_SafeTrim_NullNoExceptionIsThrownResultIsEmpty()
        {
            string target = null;

            var result = target.SafeTrim();

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void StringExtension_SafeTrim_ReturnsCorrectResult()
        {
            const string target = "The quick brown fox.      ";
            const string expectedResult = "The quick brown fox.";

            var result = target.SafeTrim();

            Assert.AreEqual(expectedResult, result);
        }


        [TestMethod]
        public void StringExtension_IsNullOrEmpty_EmptyStringReturnsTrueResult()
        {
            var target = string.Empty;

            var result = target.IsNullOrEmpty();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringExtension_IsNullOrEmpty_NullReturnsTrueResult()
        {
            string target = null;

            var result = target.IsNullOrEmpty();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringExtension_IsNullOrEmpty_PopulatedStringReturnsFalseResult()
        {
            const string target = "Test";

            var result = target.IsNullOrEmpty();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StringExtension_PasswordToSecureString_ReturnsCorrectResult()
        {
            const string password = "P@55word";
            var secureString = new SecureString();
            foreach (var c in password.ToCharArray())
            {
                secureString.AppendChar(c);
            }

            var result = password.PasswordToSecureString();

            Assert.IsTrue(IsEqualTo(secureString, result));
        }

        [TestMethod]
        public void StringExtension_IsValidXml_NullReturnsTrue()
        {
            string test = null;
            Assert.IsTrue(test.IsValidXml());
        }

        [TestMethod]
        public void StringExtension_IsValidXml_EmptyStringReturnsTrue()
        {
            var test = string.Empty;
            Assert.IsTrue(test.IsValidXml());
        }

        [TestMethod]
        public void StringExtension_IsValidXml_ValidXmlReturnsTrue()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            Assert.IsTrue(test.IsValidXml());
        }

        [TestMethod]
        public void StringExtension_IsValidXml_InvalidXmlReturnsFalse()
        {
            const string test =
                "<matt><items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items></mjj>";
            Assert.IsFalse(test.IsValidXml());
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_ReturnsCorrectNumberOfDictionaryValues()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item><item><key>Area</key><value></value></item><item><key>Theme</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionary();
            Assert.AreEqual(4, dictionary.Count);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DoesNotIgnoreBlankValuesReturnsCorrectNumberOfDictionaryValues()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value></value></item><item><key>Service User Pin Two</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionary();
            Assert.AreEqual(3, dictionary.Count);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DictionaryKeysExists()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();


            Assert.IsTrue(dictionary.ContainsKey("Service Type"));
            Assert.IsTrue(dictionary.ContainsKey("Service User Pin"));
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DictionaryValuesExists()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();


            Assert.IsTrue(dictionary.ContainsValue("Early Years"));
            Assert.IsTrue(dictionary.ContainsValue("987876678"));
        }


        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DictionaryContainsCorrectValueForKey()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();

            Assert.AreEqual("Early Years", dictionary["Service Type"]);
            Assert.AreEqual("987876678", dictionary["Service User Pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DictionaryContainsCorrectValueForKeyIgnoringCase()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();

            Assert.AreEqual("Early Years", dictionary["service type"]);
            Assert.AreEqual("987876678", dictionary["service user pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DictionaryContainsCorrectValueForKeyTrimmingLeadingWhitespaceInKeyAndTrailingWhitespaceInValue()
        {
            const string test =
                "<items><item><key> Service Type</key><value>Early Years </value></item><item><key> Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();

            Assert.AreEqual("Early Years", dictionary["Service Type"]);
            Assert.AreEqual("987876678", dictionary["Service User Pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DictionaryContainsCorrectValueForKeyTrimmingTrailingWhitespaceInKeyAndValueIgnoringCase()
        {
            const string test =
                "<items><item><key>Service Type </key><value>Early Years </value></item><item><key>Service User Pin </key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();

            Assert.AreEqual("Early Years", dictionary["service type"]);
            Assert.AreEqual("987876678", dictionary["service user pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionary_DictionaryContainsCorrectValueForKeyTrimmingWhitespaceInKeyAndWhitespaceInValue
            ()
        {
            const string test =
                "<items><item><key> Service Type </key><value> Early Years </value></item><item><key> Service User Pin </key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();

            Assert.AreEqual("Early Years", dictionary["Service Type"]);
            Assert.AreEqual("987876678", dictionary["Service User Pin"]);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void StringExtension_ToKeyValueDictionary_DictionaryThrowsKeyNotFoundExceptionForKeyThatDoesNotExist()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionary();

            Assert.AreEqual("Early Years", dictionary["Matt Jordan"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_ReturnsCorrectNumberOfDictionaryValues()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item><item><key>Area</key><value></value></item><item><key>Theme</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();
            Assert.AreEqual(2, dictionary.Count);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_IgnoresEmptyValuesReturnsCorrectNumberOfDictionaryValues()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value></value></item><item><key>Service User Pin Two</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();
            Assert.AreEqual(1, dictionary.Count);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoresEmptyValues_DictionaryKeysExists()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item><item><key>Area</key><value></value></item><item><key>Theme</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();


            Assert.IsTrue(dictionary.ContainsKey("Service Type"));
            Assert.IsTrue(dictionary.ContainsKey("Service User Pin"));
            Assert.IsFalse(dictionary.ContainsKey("Area"));
            Assert.IsFalse(dictionary.ContainsKey("Theme"));
        }


        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_DictionaryContainsCorrectValueForKey()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();

            Assert.AreEqual("Early Years", dictionary["Service Type"]);
            Assert.AreEqual("987876678", dictionary["Service User Pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_DictionaryContainsCorrectValueForKeyIgnoringCase()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();

            Assert.AreEqual("Early Years", dictionary["service type"]);
            Assert.AreEqual("987876678", dictionary["service user pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_DictionaryContainsCorrectValueForKeyTrimmingLeadingWhitespaceInKeyAndTrailingWhitespaceInValue()
        {
            const string test =
                "<items><item><key> Service Type</key><value>Early Years </value></item><item><key> Service User Pin</key><value>987876678</value></item><item><key>Area</key><value></value></item><item><key>Theme</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();

            Assert.AreEqual("Early Years", dictionary["Service Type"]);
            Assert.AreEqual("987876678", dictionary["Service User Pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_DictionaryContainsCorrectValueForKeyTrimmingTrailingWhitespaceInKeyAndValueIgnoringCase()
        {
            const string test =
                "<items><item><key>Service Type </key><value>Early Years </value></item><item><key>Service User Pin </key><value>987876678</value></item><item><key>Area</key><value></value></item><item><key>Theme</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();

            Assert.AreEqual("Early Years", dictionary["service type"]);
            Assert.AreEqual("987876678", dictionary["service user pin"]);
        }

        [TestMethod]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_DictionaryContainsCorrectValueForKeyTrimmingWhitespaceInKeyAndWhitespaceInValue()
        {
            const string test =
                "<items><item><key> Service Type </key><value> Early Years </value></item><item><key> Service User Pin </key><value>987876678</value></item><item><key>Area</key><value></value></item><item><key>Theme</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();

            Assert.AreEqual("Early Years", dictionary["Service Type"]);
            Assert.AreEqual("987876678", dictionary["Service User Pin"]);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void StringExtension_ToKeyValueDictionaryIgnoreEmptyValues_DictionaryThrowsKeyNotFoundExceptionForKeyThatDoesNotExist()
        {
            const string test =
                "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item><item><key>Area</key><value></value></item><item><key>Theme</key><value /></item></items>";
            var dictionary = test.ToKeyValueDictionaryIgnoreEmptyValues();

            Assert.AreEqual("Early Years", dictionary["Matt Jordan"]);
        }

        [TestMethod]
        public void StringExtension_IsValidXml_DuplicateKeyReturnsFalse()
        {
            const string test =
                @"<items><item><key>PIN</key><value>200654</value></item><item><key>ELSI Code</key><value>33333</value></item><item><key>Project Name</key><value>Project Name for - 33333</value></item><item><key>Cluster</key><value>Cluster 8</value></item><item><key>Patch</key><value>Patch 7</value></item><item><key>Area</key><value>North</value></item><item><key>Service Delivery Vehicle</key><value>Service Delivery Vehicle 1</value></item><item><key>Theme</key><value>Theme 2</value></item><item><key>Commissioning Body</key><value>Commissioning Body 4</value></item><item><key>Project Country</key><value>United Kingdom</value></item><item><key>Service User Forename</key><value>Summer</value></item><item><key>Service User Surname</key><value>Young</value></item><item><key>Service User DOB</key><value>24/5/2006</value></item><item><key>Service User Gender</key><value>Male</value></item><item><key>Service User Ethnicity</key><value>Albanians</value></item><item><key>Service User Address Line 1</key><value>24</value></item><item><key>Service User Address Line 2</key><value>Richmond Road</value></item><item><key>Service User Address Town</key><value>Gravesend</value></item><item><key>Service User Address County</key><value>Cambridgeshire</value></item><item><key>Service User Address Country</key><value>United Kingdom</value></item><item><key>Service User Postcode</key><value>DG5 4SR</value></item><item><key>Service User Telephone Number</key><value>9953 8594524</value></item><item><key>Service User Mobile</key><value>07914639541</value></item><item><key>Service User Email</key><value>Summer.Young@AtlasArchitecturalDesigns.edu</value></item><item><key>Service User Disability Status</key><value>Regsitered Disabled</value></item><item><key>Service User Disability Type</key><value>Activity</value></item><item><key>Service User Religion</key><value>Other</value></item><item><key>Service First Language</key><value>Mandarin</value></item><item><key>Service Second Language</key><value>English</value></item><item><key>Referral Date</key><value>8/5/2013</value></item><item><key>Referrer Name</key><value>Jude Lumb</value></item><item><key>Referral Reason</key><value>misconduct</value></item><item><key>Referral Category of Need</key><value>Referral Category of Need 4</value></item><item><key>Referral Completion Action</key><value>Referral Completion Action 5</value></item><item><key>Referral Source</key><value>Referral Source 9</value></item><item><key>Needs1</key><value>Needs 9,  Needs 8,  Needs 9,  Needs 9</value></item><item><key>Needs2</key><value>Needs 4,  Needs 7,  Needs 4,  Needs 8</value></item><item><key>Interventions1</key><value>Interventions 7,  Interventions 8,  Interventions 5,  Interventions 2</value></item><item><key>Interventions2</key><value>Interventions 1,  Interventions 6,  Interventions 5,  Interventions 8</value></item><item><key>Outcomes1</key><value>Outcomes 9,  Outcomes 4,  Outcomes 9,  Outcomes 2</value></item><item><key>Outcomes2</key><value>Outcomes 7,  Outcomes 3,  Outcomes 6,  Outcomes 8</value></item><item><key>Assessment Type</key><value>Assessment Type 6</value></item><item><key>Case Closure Planned and Agreed</key><value>Yes</value></item><item><key>Service User DOB</key><value>7/11/2015</value></item></items>";
            Assert.IsFalse(test.IsValidXml());
        }

        [TestMethod]
        public void StringExtension_IsValidXml_RemoveDuplicateKeyReturnsTrue()
        {
            const string test =
                @"<items><item><key>PIN</key><value>200654</value></item><item><key>ELSI Code</key><value>33333</value></item><item><key>Project Name</key><value>Project Name for - 33333</value></item><item><key>Cluster</key><value>Cluster 8</value></item><item><key>Patch</key><value>Patch 7</value></item><item><key>Area</key><value>North</value></item><item><key>Service Delivery Vehicle</key><value>Service Delivery Vehicle 1</value></item><item><key>Theme</key><value>Theme 2</value></item><item><key>Commissioning Body</key><value>Commissioning Body 4</value></item><item><key>Project Country</key><value>United Kingdom</value></item><item><key>Service User Forename</key><value>Summer</value></item><item><key>Service User Surname</key><value>Young</value></item><item><key>Service User DOB</key><value>24/5/2006</value></item><item><key>Service User Gender</key><value>Male</value></item><item><key>Service User Ethnicity</key><value>Albanians</value></item><item><key>Service User Address Line 1</key><value>24</value></item><item><key>Service User Address Line 2</key><value>Richmond Road</value></item><item><key>Service User Address Town</key><value>Gravesend</value></item><item><key>Service User Address County</key><value>Cambridgeshire</value></item><item><key>Service User Address Country</key><value>United Kingdom</value></item><item><key>Service User Postcode</key><value>DG5 4SR</value></item><item><key>Service User Telephone Number</key><value>9953 8594524</value></item><item><key>Service User Mobile</key><value>07914639541</value></item><item><key>Service User Email</key><value>Summer.Young@AtlasArchitecturalDesigns.edu</value></item><item><key>Service User Disability Status</key><value>Regsitered Disabled</value></item><item><key>Service User Disability Type</key><value>Activity</value></item><item><key>Service User Religion</key><value>Other</value></item><item><key>Service First Language</key><value>Mandarin</value></item><item><key>Service Second Language</key><value>English</value></item><item><key>Referral Date</key><value>8/5/2013</value></item><item><key>Referrer Name</key><value>Jude Lumb</value></item><item><key>Referral Reason</key><value>misconduct</value></item><item><key>Referral Category of Need</key><value>Referral Category of Need 4</value></item><item><key>Referral Completion Action</key><value>Referral Completion Action 5</value></item><item><key>Referral Source</key><value>Referral Source 9</value></item><item><key>Needs1</key><value>Needs 9,  Needs 8,  Needs 9,  Needs 9</value></item><item><key>Needs2</key><value>Needs 4,  Needs 7,  Needs 4,  Needs 8</value></item><item><key>Interventions1</key><value>Interventions 7,  Interventions 8,  Interventions 5,  Interventions 2</value></item><item><key>Interventions2</key><value>Interventions 1,  Interventions 6,  Interventions 5,  Interventions 8</value></item><item><key>Outcomes1</key><value>Outcomes 9,  Outcomes 4,  Outcomes 9,  Outcomes 2</value></item><item><key>Outcomes2</key><value>Outcomes 7,  Outcomes 3,  Outcomes 6,  Outcomes 8</value></item><item><key>Assessment Type</key><value>Assessment Type 6</value></item><item><key>Case Closure Planned and Agreed</key><value>Yes</value></item></items>";
            Assert.IsTrue(test.IsValidXml());
        }

        [TestMethod]
        public void StringExtension_OpenProjectTitleUpdate_ReturnsCorrectResult()
        {

            const string test = @"Test Project [Open] Current open episode";
            var title = test.UpdateTitle("Bayswater Project");

            Assert.AreEqual("Bayswater Project [Open] Current open episode", title);
        }

        [TestMethod]
        public void StringExtension_ClosedProjectTitleUpdate_ReturnsCorrectResult()
        {

            const string test = @"Test Project [Closed] Current open episode";
            var title = test.UpdateTitle("Bayswater Project");

            Assert.AreEqual("Bayswater Project [Closed] Current open episode", title);
        }

        [TestMethod]
        public void StringExtension_OpenProjectCaseInsensitiveTitleUpdate_ReturnsCorrectResult()
        {

            const string test = @"Test Project [OPEN] Current open episode";
            var title = test.UpdateTitle("Bayswater Project");

            Assert.AreEqual("Bayswater Project [OPEN] Current open episode", title);
        }

        [TestMethod]
        public void StringExtension_ClosedProjectCaseInsensitiveTitleUpdate_ReturnsCorrectResult()
        {

            const string test = @"Test Project [CLOSED] Current open episode";
            var title = test.UpdateTitle("Bayswater Project");

            Assert.AreEqual("Bayswater Project [CLOSED] Current open episode", title);
        }

        [TestMethod]
        public void StringExtension_NoOpenOrClosedProject_ReturnsCorrectResult()
        {

            const string test = @"Test Project Current open episode";
            var title = test.UpdateTitle("Bayswater Project");

            Assert.AreEqual("Test Project Current open episode", title);
        }

        [TestMethod]
        public void StringExtension_NoOpenOrClosedProjectCaseInsensitiveTitleUpdate_ReturnsCorrectResult()
        {

            const string test = @"Test Project Current open episode";
            var title = test.UpdateTitle("Bayswater Project");

            Assert.AreEqual("Test Project Current open episode", title);
        }


        private static bool IsEqualTo(SecureString ss1, SecureString ss2)
        {
            var bstr1 = IntPtr.Zero;
            var bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = Marshal.SecureStringToBSTR(ss1);
                bstr2 = Marshal.SecureStringToBSTR(ss2);
                var length1 = Marshal.ReadInt32(bstr1, -4);
                var length2 = Marshal.ReadInt32(bstr2, -4);
                if (length1 == length2)
                {
                    for (var x = 0; x < length1; ++x)
                    {
                        byte b1 = Marshal.ReadByte(bstr1, x);
                        byte b2 = Marshal.ReadByte(bstr2, x);
                        if (b1 != b2) return false;
                    }
                }
                else return false;
                return true;
            }
            finally
            {
                if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
                if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            }
        }
    }
}
