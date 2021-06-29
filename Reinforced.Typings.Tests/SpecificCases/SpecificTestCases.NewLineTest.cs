using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void NewLineTest_Default() {
            var newLine = Environment.NewLine;
            string result = $"module Reinforced.Typings.Tests.SpecificCases {{{newLine}export interface ITestInterface{newLine}{{{newLine}Int: number;{newLine}String: string;{newLine}}}{newLine}}}{newLine}";
            AssertConfiguration(s => {
                s.Global(a => a.DontWriteWarningComment().TabSymbol(string.Empty).ReorderMembers());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
            }, result);
        }

        [Fact]
        public void NewLineTest_Explicit() {
            const string newLine = "\n";
            string result = $"module Reinforced.Typings.Tests.SpecificCases {{{newLine}export interface ITestInterface{newLine}{{{newLine}Int: number;{newLine}String: string;{newLine}}}{newLine}}}{newLine}";
            var actual = AssertConfiguration(s => {
                s.Global(a => a.DontWriteWarningComment().TabSymbol(string.Empty).NewLine(newLine).ReorderMembers());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
            }, result);

            Assert.Equal(result, actual);
        }
    }
}