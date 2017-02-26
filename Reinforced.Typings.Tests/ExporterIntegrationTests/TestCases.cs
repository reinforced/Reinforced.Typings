using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public interface ITestInterface
    {
        string String { get; }
        int Int { get; }
    }

    public class TestClass
    {
        public string String { get; set; }
        public int Int { get; set; }
    }

    public class ClassWithMethods
    {
        public string String { get; set; }
        public int Int { get; set; }

        [TsDecorator("b()")]
        public void DoSomethinig()
        {
            
        }
    }

    public class PandaWoodForceNullableCase
    {
        [TsProperty(ForceNullable = true)]
        public string PandaWoodProperty { get; set; }
    }
}
