using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Extenstion.Tests
{
    /// <summary>
    /// Summary description for EnumExtensionTests
    /// </summary>
    [TestClass]
    public class EnumExtensionTests
    {

        [TestMethod]
        public void EnumExtension_ConvertEnumToList_GetList_ReturnsCorrectNumber()
        {
            var list = EnumExtensions.ConvertEnumToList<OperationTaskType>();
            Assert.AreEqual(19, list.Count);
        }

        [TestMethod]
        public void EnumExtension_ConvertEnumToList_GetList_HasCorrectValues()
        {
            var list = EnumExtensions.ConvertEnumToList<OperationTaskType>();
            Assert.IsTrue(list.Contains("AllocatePin"));
            Assert.IsTrue(list.Contains("AllocateCase"));
        }

        [TestMethod]
        public void EnumExtension_GetEnumDescription_GetValue_HasCorrectValue()
        {
            var d = EnumExtensions.GetEnumDescription(OperationTaskType.ChangePrimaryProject);
            Assert.AreEqual("ChangePrimaryProject", d);
        }

        [TestMethod]
        public void EnumExtension_ToEnum_EnumStringValueSupplied_HasCorrectValue()
        {
            var e = "ChangePrimaryProject".ToEnum<OperationTaskType>();
            Assert.AreEqual(OperationTaskType.ChangePrimaryProject, e);
        }

        [TestMethod]
        public void EnumExtension_ToEnumText_IntIsEnum_HasCorrectValue()
        {
            var e = 0.ToEnumText<OperationTaskType>();
            Assert.AreEqual("AllocatePin", e);
        }
    }
}
