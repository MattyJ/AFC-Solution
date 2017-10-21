using Fujitsu.AFC.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.AFC.Extenstion.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DictionaryExtensionTests
    {
        [TestMethod]
        public void DictionaryExtension_ToXmlString_ConvertsXMLStringSuccessful()
        {
            const string test = "<items><item><key>PIN</key><value>1234567</value></item><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";

            var dictionary = new Dictionary<string, string>
            {
                {"PIN", "1234567" },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var xmlString = dictionary.ToXmlString();
            Assert.AreEqual(test, xmlString);
        }

        [TestMethod]
        public void DictionaryExtension_MergeDictionary_MergesXMLSuccessfulUpdatesThePinKeyValuePair()
        {
            const string expectedPin = "7654321";

            var masterDictionary = new Dictionary<string, string>
            {
                {"PIN", "1234567" },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var deltaDictionary = new Dictionary<string, string>
            {
                { "PIN", expectedPin }
            };

            masterDictionary.MergeDictionary(deltaDictionary);
            Assert.AreEqual(3, masterDictionary.Count);
            Assert.AreEqual(expectedPin, masterDictionary["PIN"]);
        }

        [TestMethod]
        public void DictionaryExtension_MergeDictionary_MergesXMLNoUpdate()
        {
            const string expectedPin = "7654321";

            var masterDictionary = new Dictionary<string, string>
            {
                {"PIN", expectedPin },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var deltaDictionary = new Dictionary<string, string>
            {
                { "PIN", expectedPin }
            };

            masterDictionary.MergeDictionary(deltaDictionary);
            Assert.AreEqual(3, masterDictionary.Count);
            Assert.AreEqual(expectedPin, masterDictionary["PIN"]);
        }

        [TestMethod]
        public void DictionaryExtension_MergeDictionary_MergesXMLSuccessfulAddingOneKeyValuePair()
        {
            var masterDictionary = new Dictionary<string, string>
            {
                {"PIN", "1234567" },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var deltaDictionary = new Dictionary<string, string>
            {
                { "Service Delivery Vehicle", "Service Delivery Vehicle 1" }
            };

            masterDictionary.MergeDictionary(deltaDictionary);
            Assert.AreEqual(4, masterDictionary.Count);
            Assert.IsTrue(masterDictionary.ContainsKey("Service Delivery Vehicle"));
        }

        [TestMethod]
        public void DictionaryExtension_MergeDictionary_MergesXMLSuccessfulRemovingAKeyValuePair()
        {
            var masterDictionary = new Dictionary<string, string>
            {
                {"PIN", "1234567" },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var deltaDictionary = new Dictionary<string, string>
            {
                { "Service User Pin", "" }
            };

            masterDictionary.MergeDictionary(deltaDictionary);
            Assert.AreEqual(2, masterDictionary.Count);
            Assert.IsFalse(masterDictionary.ContainsKey("Service User Pin"));
        }


        [TestMethod]
        public void DictionaryExtension_MergeDictionary_MergesXMLSuccessfulAddingThreeKeyValuePairsOneRemovingTwo()
        {
            const string serviceDeliveryVehicle = "Service Delivery Vehicle 1";
            var masterDictionary = new Dictionary<string, string>
            {
                {"PIN", "1234567" },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var deltaDictionary = new Dictionary<string, string>
            {
                { "Service Delivery Vehicle", serviceDeliveryVehicle },
                { "Patch", "Patch 1" },
                { "Area", "North" },
                {"Service Type", ""},
                {"Service User Pin", "" }


            };

            masterDictionary.MergeDictionary(deltaDictionary);
            Assert.AreEqual(4, masterDictionary.Count);
            Assert.IsTrue(masterDictionary.ContainsKey("Service Delivery Vehicle"));
            Assert.AreEqual(serviceDeliveryVehicle, masterDictionary["Service Delivery Vehicle"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DictionaryExtension_MergeDictionary_MergesXMLDuplicateThrowsException()
        {
            const string serviceDeliveryVehicle = "Service Delivery Vehicle 1";
            var masterDictionary = new Dictionary<string, string>
            {
                {"PIN", "1234567" },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var deltaDictionary = new Dictionary<string, string>
            {
                { "Service Delivery Vehicle", serviceDeliveryVehicle },
                { "Patch", "Patch 1" },
                { "Area", "North" },
                {"Service Type", ""},
                {"Service User Pin", "" },
                { "Service Delivery Vehicle", serviceDeliveryVehicle },


            };

            masterDictionary.MergeDictionary(deltaDictionary);
        }

        [TestMethod]
        public void DictionaryExtension_MergeDictionary_MergesXMLDoesNotAddNewKeyWithEmptyValue()
        {
            const string serviceDeliveryVehicle = "Service Delivery Vehicle 1";
            var masterDictionary = new Dictionary<string, string>
            {
                {"PIN", "1234567" },
                {"Service Type", "Early Years"},
                {"Service User Pin", "987876678" }
            };

            var deltaDictionary = new Dictionary<string, string>
            {
                { "Service Delivery Vehicle", serviceDeliveryVehicle },
                { "Patch", "Patch 1" },
                { "Area", "North" },
                {"Service Type", ""},
                {"Service User Pin Two", "" }
            };

            masterDictionary.MergeDictionary(deltaDictionary);
            Assert.AreEqual(5, masterDictionary.Count);
        }

    }
}
