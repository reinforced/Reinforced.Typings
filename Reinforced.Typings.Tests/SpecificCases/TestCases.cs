using System;
using Reinforced.Typings.Attributes;

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

    #endregion

    #region Decorators

    public class ClassWithMethods
    {
        public string String { get; set; }
        public int Int { get; set; }

        [TsDecorator("b()")]
        public void DoSomethinig()
        {

        }
    }

    #endregion

    #region ForceNullable property

    public class PandaWoodForceNullableCase
    {
        [TsProperty(ForceNullable = true)]
        public string PandaWoodProperty { get; set; }
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

    #region Substitutions

    public class CrozinSubstitutionTest
    {
        public Guid GuidProperty { get; set; }

        public DateTime TimeProperty { get; set; }
    }

    public class CrozinLocalSubstitutionTest
    {
        public Guid OneMoreGuidProperty { get; set; }

        public DateTime OneMoreTimeProperty { get; set; }
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

    #region DDante case

    public abstract class PolluxEntity<Key>
    {
        public Key EntityId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }


        public PolluxEntity()
        {
            this.CreatedOn = DateTime.Now;
        }
    }


    public class ContactData : PolluxEntity<long>
    {
        public string Phone { get; set; }
        public string AlternatePhone { get; set; }
        public bool PhoneConfirmed { get; set; }

        public string Email { get; set; }
        public string AlternateEmail { get; set; }
        public bool EmailConfirmed { get; set; }

        public virtual string OwnerId { get; set; }

        public ContactData()
        {
        }
    }

    #endregion

    #region Type references

    public class SomeIndirectlyReferencedClass { }

    public class SomeOtherReferencedType { }

    public class SomeFluentReferencedType { }

    public class SomeFluentlyReferencedNotExported{ }

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

    #endregion
}
