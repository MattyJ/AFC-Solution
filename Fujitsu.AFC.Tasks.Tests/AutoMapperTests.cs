using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Fujitsu.AFC.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Handlers.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AutoMapperTests
    {
        [TestMethod]
        public void MapperConfigTest()
        {
            Bootstrapper.Initialise();
            Mapper.AssertConfigurationIsValid();
        }
    }
}
