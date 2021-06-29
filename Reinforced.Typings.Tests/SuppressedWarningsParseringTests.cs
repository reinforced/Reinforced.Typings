using Reinforced.Typings.Cli;
using Xunit;

namespace Reinforced.Typings.Tests
{
    public class SuppressedWarningsParseringTests
    {
        [Fact]
        public void SuppressedWarningsParseringWorks_Case1()
        {
            var result = Bootstrapper.ParseSuppressedWarnings("1;2;3;4");
            Assert.Collection(result,
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x),
                x => Assert.Equal(4, x));
        }
        
        [Fact]
        public void SuppressedWarningsParseringWorks_Case2()
        {
            var result = Bootstrapper.ParseSuppressedWarnings("001;00002;0030;04");
            Assert.Collection(result,
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(30, x),
                x => Assert.Equal(4, x));
        }
        
        [Fact]
        public void SuppressedWarningsParseringWorks_Case3()
        {
            var result = Bootstrapper.ParseSuppressedWarnings("RTW001;RTW00002;RTW0030;RTW4");
            Assert.Collection(result,
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(30, x),
                x => Assert.Equal(4, x));
        }
        
        [Fact]
        public void SuppressedWarningsParseringWorks_Case4()
        {
            var result = Bootstrapper.ParseSuppressedWarnings("RTW0001_DocumentationNotSupplied;RTW0002_DocumentationNotFound;RTW0003_TypeUnknown;RTW4");
            Assert.Collection(result,
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x),
                x => Assert.Equal(4, x));
        }
    }
}