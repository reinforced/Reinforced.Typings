using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
  /// <summary>
  /// Some documentation for interface.
  /// </summary>
  public interface ISomeInterfaceWithDocs
  {
    /// <summary>
    /// Some documentation for interface property.
    /// </summary>
    string InterfaceProp { get; set; }

    /// <summary>
    /// Some documentation for interface method.
    /// </summary>
    void InterfaceMethod();
  }

  /// <inheritdoc/>
  public interface ISomeInterfaceWithInheritDoc : ISomeInterfaceWithDocs
  {
    // TODO: Test with inheritdoc + cref, makes no sense without cref
  }

  /// <inheritdoc/>
  public class SomeClassWithInterfaceInheritDoc : ISomeInterfaceWithDocs
  {
    /// <inheritdoc/>
    public string InterfaceProp { get; set; }

    /// <inheritdoc/>
    public void InterfaceMethod() {}
  }

  /// <summary>
  /// Some documentation for class.
  /// </summary>
  public class SomeClassWithDocs
  {
    /// <summary>
    /// Some documentation for constructor.
    /// </summary>
    public SomeClassWithDocs() {}

    /// <summary>
    /// Some documentation for property.
    /// </summary>
    public virtual string SomeProp { get; set; }

    /// <summary>
    /// Some documentation for method.
    /// </summary>
    public virtual void SomeMethod() {}
  }

  /// <inheritdoc/>
  public class SomeClassWithInheritDoc : SomeClassWithDocs
  {
    /// <inheritdoc/>
    public SomeClassWithInheritDoc() {}

    /// <inheritdoc/>
    public override string SomeProp { get; set; }

    /// <inheritdoc/>
    public override void SomeMethod() {}
  }

  /// <inheritdoc cref="SomeClassWithDocs"/>
  public class SomeClassWithInheritDocCref
  {
    /// <inheritdoc cref="SomeClassWithDocs()"/>
    public SomeClassWithInheritDocCref() {}

    /// <inheritdoc cref="SomeClassWithDocs.SomeProp"/>
    public virtual string AnotherProp { get; set; }

    /// <inheritdoc cref="SomeClassWithDocs.SomeMethod"/>
    public virtual void AnotherMethod() {}
  }

  public partial class SpecificTestCases
  {
    [Fact]
    public void InheritDocInterface()
    {
      const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	/**
	* @inheritdoc
	*/
	export interface ISomeInterfaceWithInheritDoc
	{
	}
}
";

      AssertConfiguration(
        s =>
        {
          s.Global(a => a.DontWriteWarningComment().GenerateDocumentation());
          s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);

          s.ExportAsInterface<ISomeInterfaceWithInheritDoc>()
           .WithPublicProperties().WithPublicMethods();
        }, result, exp =>
        {
          var doc = exp.Context.Documentation.GetDocumentationMember(typeof(ISomeInterfaceWithInheritDoc));
          Assert.NotNull(doc);
        }, compareComments: true);
    }

    [Fact]
    public void InheritDocInterfaceToClass()
    {
      const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	/**
	* @inheritdoc
	*/
	export class SomeClassWithInterfaceInheritDoc
	{ 
		/**
		* @inheritdoc
		*/
		public InterfaceProp: string;
		/**
		* @inheritdoc
		*/
		public InterfaceMethod() : void { } 
	}
}
";

      AssertConfiguration(
        s =>
        {
          s.Global(a => a.DontWriteWarningComment().GenerateDocumentation());
          s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);

          s.ExportAsClass<SomeClassWithInterfaceInheritDoc>()
           .WithPublicProperties()
           .WithPublicMethods();
        }, result, exp =>
        {
          var doc = exp.Context.Documentation.GetDocumentationMember(typeof(SomeClassWithInterfaceInheritDoc));
          Assert.NotNull(doc);
        }, compareComments: true);
    }

    [Fact]
    public void InheritDocClass()
    {
      const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	/**
	* @inheritdoc
	*/
	export class SomeClassWithInheritDoc
	{
		/**
		* @inheritdoc
		*/
		constructor () { } 
		/**
		* @inheritdoc
		*/
		public SomeProp: string;
		/**
		* @inheritdoc
		*/
		public SomeMethod() : void { } 
	}
}
";

      AssertConfiguration(
        s =>
        {
          s.Global(a => a.DontWriteWarningComment().GenerateDocumentation());
          s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);

          s.ExportAsClass<SomeClassWithInheritDoc>()
           .WithConstructor()
           .WithPublicProperties()
           .WithPublicMethods();
        }, result, exp =>
        {
          var doc = exp.Context.Documentation.GetDocumentationMember(typeof(SomeClassWithInheritDoc));
          Assert.NotNull(doc);
        }, compareComments: true);
    }

    /// <summary>
    /// TODO: This test fails because cref handling isn't implemented
    /// </summary>
    [Fact]
    public void InheritDocCref()
    {
      const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	/** Some documentation for class. */
	export class SomeClassWithInheritDocCref
	{
		/** Some documentation for constructor. */
		constructor () { }

		/** Some documentation for property. */
		public AnotherProp: string; 

		/** Some documentation for method. */
		public AnotherMethod() : void { }  
	}
}
";

      AssertConfiguration(
        s =>
        {
          s.Global(a => a.DontWriteWarningComment().GenerateDocumentation());
          s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);

          s.ExportAsClass<SomeClassWithInheritDocCref>()
           .WithConstructor()
           .WithPublicProperties()
           .WithPublicMethods();
        }, result, exp =>
        {
          var doc = exp.Context.Documentation.GetDocumentationMember(typeof(SomeClassWithInheritDocCref));
          Assert.NotNull(doc);
        }, compareComments: true);
    }
  }
}