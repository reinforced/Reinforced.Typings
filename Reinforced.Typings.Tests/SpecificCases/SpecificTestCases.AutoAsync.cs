using System.Threading.Tasks;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    class TestAsync
    {
        public async Task DoVoid()
        {

        }

        public async Task<string> DoArgument()
        {
            return "aaa";
        }
    }

    class TestAsync2
    {
        public async Task DoVoid()
        {

        }

        public async Task<string> DoArgument()
        {
            return "aaa";
        }
    }

    interface ITestAsync
    {
        Task DoVoid();

        Task<string> DoArgument();
    }

    public partial class SpecificTestCases
    {
        [Fact]
        public void AutoAsyncWorks()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export class TestAsync
	{
		public async DoVoid() : Promise<void> { } 
		public async DoArgument() : Promise<string>
		{
			return null;
		}
	}
	export interface ITestAsync2
	{
		DoVoid() : Promise<void>;
		DoArgument() : Promise<string>;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().AutoAsync());
                s.ExportAsClass<TestAsync>().WithPublicMethods();
                s.ExportAsInterface<TestAsync2>().WithPublicMethods();
            }, result);
        }

        [Fact]
        public void AutoAsyncInterfaceWorks()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface ITestAsync
	{
		DoVoid() : Promise<void>;
		DoArgument() : Promise<string>;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().AutoAsync());
                s.ExportAsInterface<ITestAsync>().WithPublicMethods();
            }, result);
        }

        [Fact]
        public void NoAutoAsyncInterfaceWorks()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface ITestAsync
	{
		DoVoid() : void;
		DoArgument() : string;
	}
}";
            AssertConfiguration(s =>
            {
                s.ExportAsInterface<ITestAsync>().WithPublicMethods();
            }, result);
        }
    }
}