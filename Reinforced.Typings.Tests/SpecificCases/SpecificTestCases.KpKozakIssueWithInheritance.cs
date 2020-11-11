using System.Collections.Generic;
using System.IO;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        //[TsInterface]
        public class ComponentProps<T>
        {
            public T Model { get; set; }
        }

        //[TsInterface]
        public class AnotherComponentProps : ComponentProps<TestComponentViewModel>
        {
            
        }

        //[TsInterface]
        public class TestComponentProps : ComponentProps<TestComponentViewModel>
        {
            public int Number { get; set; }
        }

        //[TsInterface]
        public class TestComponentViewModel
        {
            
        }
        [Fact]
        public void KpKozakIssueWithInheritance()
        {
            Dictionary<string,string> results = new Dictionary<string, string>()
            {
                {
"targetDir" + Path.DirectorySeparatorChar + "TestComponentViewModel.ts",
@"export interface TestComponentViewModel
{
}"
                },
                {
"targetDir" + Path.DirectorySeparatorChar + "ComponentProps_1.ts",
@"export interface ComponentProps<T>
{
	model: T;
}"
                },
                {
"targetDir" + Path.DirectorySeparatorChar + "AnotherComponentProps.ts",
@"import { TestComponentViewModel } from './TestComponentViewModel';
import { ComponentProps } from './ComponentProps_1';

export interface AnotherComponentProps extends ComponentProps<TestComponentViewModel>
{
}"
                },
                {
"targetDir" + Path.DirectorySeparatorChar + "TestComponentProps.ts",
@"import { TestComponentViewModel } from './TestComponentViewModel';
import { ComponentProps } from './ComponentProps_1';

export interface TestComponentProps extends ComponentProps<TestComponentViewModel>
{
	number: number;
}"
                }

            };

            var exportedTypes = new[]
            {
                typeof(TestComponentViewModel), typeof(ComponentProps<>), typeof(AnotherComponentProps),
                typeof(TestComponentProps)
            };

            var componentViewModelType = typeof(TestComponentViewModel);

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().CamelCaseForProperties().UseModules().RootNamespace("TestProject"));

                foreach (var exportedType in exportedTypes)
                {
                    s.ExportAsInterfaces(new[] { exportedType }, b =>
                    {
                        b.AutoI(false);
                        b.WithAllMethods(m => m.Ignore())
                            .WithAllProperties();

                        var fileName = exportedType.IsGenericType ?
                            exportedType.Name.Substring(0, exportedType.Name.IndexOf('`')) :
                            componentViewModelType.Name;

                        b.ExportTo(exportedType.Name.Replace("`","_") + ".ts");
                    });
                }
            }, results);
        }
    }
}