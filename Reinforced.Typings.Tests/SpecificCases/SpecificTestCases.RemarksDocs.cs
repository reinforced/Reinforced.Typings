using System;
using System.Threading.Tasks;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        /// <summary>
        ///   dummy
        /// </summary>
        /// <remarks>
        ///   Class remark.
        /// </remarks>
        public class SomeClassWithRemarks
        {
            /// <summary>
            ///   dummy
            /// </summary>
            /// <remarks>
            ///   Ctor remark.
            /// </remarks>
            public SomeClassWithRemarks() { }

            /// <inheritdoc cref="SomeClassWithDocs"/>
            /// <remarks>
            ///   Property remark.
            /// </remarks>
            public string ClassProp { get; set; }

            /// <inheritdoc cref="SomeClassWithDocs"/>
            /// <remarks>
            ///   Method remark.
            /// </remarks>
            public void Method() { }
        }

        [Fact]
        public void RemarksParsedFromDocs()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	/** dummy */
	export class SomeClassWithRemarks
	{
		/**
		* @inheritdoc
		*/
		public ClassProp: string;
		/**
		* @inheritdoc
		*/
		public Method() : void { } 
	}
}";
            AssertConfiguration(
                s =>
                {
                    s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);
                    s.ExportAsClass<SomeClassWithRemarks>()
                        .WithPublicProperties()
                        .WithPublicMethods();
                },
                result,
                expAction =>
                {
                    // Currently the <remarks /> are just parsed into the DocumentationMember
                    // but not used in the generated typings. Hence we use the action on
                    // the ExportContext to perform the actual test.
                    // To avoid surprises like empty docs <remarks /> are currently only valid
                    // if there is a <summar /> or <inheritdoc /> tag.

                    var type = typeof(SomeClassWithRemarks);

                    var classDoc = expAction.Context.Documentation.GetDocumentationMember(type);
                    Assert.NotNull(classDoc);
                    Assert.Equal("Class remark.", classDoc.Remarks.Text);

                    var memberDoc = expAction.Context.Documentation.GetDocumentationMember(type.GetProperty(nameof(SomeClassWithRemarks.ClassProp)));
                    Assert.NotNull(memberDoc);
                    Assert.Equal("Property remark.", memberDoc.Remarks.Text);

                    var methodeDoc = expAction.Context.Documentation.GetDocumentationMember(type.GetMethod(nameof(SomeClassWithRemarks.Method)));
                    Assert.NotNull(methodeDoc);
                    Assert.Equal("Method remark.", methodeDoc.Remarks.Text);

                    var ctorRemark = expAction.Context.Documentation.GetDocumentationMember(type.GetConstructor(Array.Empty<Type>()));
                    Assert.NotNull(ctorRemark);
                    Assert.Equal("Ctor remark.", ctorRemark.Remarks.Text);
                }
            );
        }
    }
}