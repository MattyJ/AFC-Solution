using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Extenstion.Tests
{
    [TestClass]
    public class ObjectExtensionTests
    {
        [TestMethod]
        public void ObjectExtensions_ConvertGenericValueToString_SuppliedValueIsNull_ReturnsNull()
        {
            var s = null as string;
            Assert.IsNull(s.ConvertGenericValueToString());
        }

        [TestMethod]
        public void ObjectExtensions_ConvertGenericValueToString_SuppliedValueIsDateTimeFormat_ReturnsFormatAsDefinedByDatabaseConstant()
        {
            var now = DateTime.Now;
            var expected = now.ToString(Database.DateTimeFormat);
            var actual = now.ConvertGenericValueToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ObjectExtensions_ConvertGenericValueToString_SuppliedValueInt_ReturnsInt()
        {
            const string expected = "2";
            var actual = 2.ConvertGenericValueToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
