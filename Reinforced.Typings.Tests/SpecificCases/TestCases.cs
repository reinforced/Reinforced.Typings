using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Generators;
//[assembly:TsGlobal(CamelCaseForProperties = true)]
namespace Reinforced.Typings.Tests.SpecificCases
{

    #region Basic test

    
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

    public class TestClass2
    {
        public string String { get; set; }
        public int Int { get; set; }
    }

    #endregion

    #region Enums in namespaces

    public class JonsaModel
    {
        public JonsaEnum Enum { get; set; }
    }

    public enum JonsaEnum
    {
        Foo, Bar
    }

    #endregion

    

    #region Camelcase

    public class PandaWoodCamelCaseTest
    {
        public int ID { get; set; }

        public void ETA()
        {

        }

        public void ISODate(DateTime date)
        {

        }

        public void PerformRequest()
        {

        }
    }

    #endregion

    #region Daggmano AutoI bug
    public class ExternalUserDaggmano
    {
        public string Name { get; set; }
    }

    public class InternalUserDaggmano
    {
        public string Name { get; set; }
    }

    public interface IAlreadyContainsI
    {
        string Name { get; }
    }

    public interface DoesntContainI
    {
        string Name { get; }
    }
    public class DoNotNeedIAtAll
    {
        public string Name { get; set; }
    }
    public interface ICannotBeRemovedHere
    {
        string Name { get; }
    }
    #endregion

    

    #region Type references

    public class SomeIndirectlyReferencedClass { }

    public class SomeOtherReferencedType { }

    public class SomeFluentReferencedType { }

    public class SomeFluentlyReferencedNotExported { }

    [TsAddTypeImport("* as '$'", "JQuery")]
    [TsAddTypeReference(typeof(SomeOtherReferencedType))]
    [TsAddTypeReference("../../jquery.d.ts")]
    public class SomeReferencingType
    {
        public SomeIndirectlyReferencedClass Indirect()
        {
            return null;
        }
    }

    [TsAddTypeImport("* as '$'", "JQuery")]
    [TsAddTypeReference(typeof(SomeOtherReferencedType))]
    [TsAddTypeReference("../../jquery.d.ts")]
    public class AnothreReferencingType
    {
        public SomeIndirectlyReferencedClass Indirect()
        {
            return null;
        }

        public SomeFluentReferencedType Property { get; set; }

        public SomeIndirectEnum Enum { get; set; }
    }


    public enum SomeIndirectEnum
    {
        One, Two, Three
    }
    #endregion

    #region Inline functions

    public class ClassWithManyMethods
    {
        public string String { get; set; }
        public int Int { get; set; }

        public void DoSomethinig() { }

        public void DoSomethingElse() { }

        public string DoSomethingElseWithResult() { return null; }
    }

    #endregion

    #region Generics

    public interface ISimpleGenericsInterface<T>
    {
        T Property { get; }

        T Method(T param);
    }

    public interface IParametrizedGenericsInterface<T, T2>
    {
        Dictionary<T, T2> Bag { get; }

        IEnumerable<Dictionary<T2, T>> Bag2 { get; }

        IEnumerable<List<Dictionary<T, T>>> Bag3 { get; }

        Dictionary<T, Dictionary<T2, List<IEnumerable<T>>>> Bag4 { get; }
    }

    public interface IAttributeParametrization<[TsGeneric("any")]T> : ISimpleGenericsInterface<T>
    {

    }

    public interface IChildParametrized<T> : IParametrizedGenericsInterface<T, int>
    {

    }

    public interface ITriforce<T1, T2, T3>
    {

    }

    public interface ITrimplementor1<TMaster> : ITriforce<TMaster, List<TMaster>, Dictionary<int, IEnumerable<TMaster>>>
    {

    }

    public interface ITrimplementor2<TMaster, TChild> : ITriforce<TMaster, List<TMaster>, Dictionary<int, IEnumerable<TMaster>>>
    {
        TChild Property { get; }
    }

    public interface ITrimplementor3<TMaster, TChild> : ITriforce<TMaster, List<TMaster>, Dictionary<int, IEnumerable<TChild>>>
    {
        Tuple<TMaster, TChild> Property { get; }
    }

    #endregion

    

    #region Weird inheritance

    public class BaseClass
    {
        public string Property { get; set; }

        public void DoSomething()
        {

        }
    }

    public class DerivedClass : BaseClass
    {
        public string GetName()
        {
            return null;
        }
    }

    #endregion

    

    #region Inline inferring test
    public interface IInferringTestInterface
    {
        string String { get; }
        int Int { get; }
        Guid Guid { get; }
        DateTime DateTime { get; }
        ITestInterface TestInterface { get; }

        void VoidMethod1(int arg);
        void VoidMethod2(int arg);
        void VoidMethod3(int arg);
        void VoidMethod4(int arg);

        int SomeMethod1(int arg);
        int SomeMethod2(int arg);
        int SomeMethod3(int arg);
        int SomeMethod4(int arg);
    }
    #endregion

    #region Daniel West Ref. Files test

    public enum SomeEnum
    {
        One, Two
    }

    public class SomeViewModel
    {
        public SomeEnum Enum { get; set; }
    }

    #endregion

   
}
