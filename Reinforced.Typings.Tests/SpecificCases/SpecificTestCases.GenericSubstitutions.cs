using System.Linq;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    interface INotPromise<T>
    {

    }
    class A
    {
        public async Task<string> Do()
        {
            return string.Empty;
        }
    }

    class B
    {
        public INotPromise<string> NotPromise { get; set; }

        public void MethodWithPromise(INotPromise<string> p)
        {

        }
    }
    class C<T>
    {
        public INotPromise<T> NotPromise { get; set; }

        public void MethodWithPromise(INotPromise<T> p)
        {

        }
    }
    class D<T>
    {
        public INotPromise<T> NotPromise { get; set; }

        public void MethodWithPromise(INotPromise<T> p)
        {

        }
    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void GenericSubstitutions()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IA
	{
		Do() : angular.Promise<string>;
	}
	export interface IB
	{
		NotPromise: angular.Promise<string>;
		MethodWithPromise(p: angular.Promise<string>) : void;
	}
	export interface IC<T>
	{
		NotPromise: angular.Promise<T>;
		MethodWithPromise(p: angular.Promise<T>) : void;
	}
	export interface ID
	{
		NotPromise: angular.Promise<number>;
		MethodWithPromise(p: angular.Promise<number>) : void;
	}
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.SubstituteGeneric(typeof(INotPromise<>), (t, tr) => new RtSimpleTypeName("angular.Promise", t.GetGenericArguments().Select(tr.ResolveTypeName).ToArray()));

                s.ExportAsInterface<A>()
                    .SubstituteGeneric(typeof(Task<>), (t, tr) => new RtSimpleTypeName("angular.Promise", t.GetGenericArguments().Select(tr.ResolveTypeName).ToArray()))
                    .WithPublicProperties()
                    .WithPublicMethods();
                s.ExportAsInterface<B>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    ;
                s.ExportAsInterfaces(new[] { typeof(C<>) }, x => x.WithPublicProperties().WithPublicMethods());
                s.ExportAsInterface<D<int>>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    ;
            }, result);
        }
    }
}