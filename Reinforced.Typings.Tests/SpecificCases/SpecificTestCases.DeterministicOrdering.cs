using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        #region Deterministic ordering test types

        // Members declared in deliberately non-alphabetical order so that
        // "declaration order" is distinguishable from "alphabetical order".
        public class DetMembers
        {
            public string Zebra { get; set; }
            public string Apple { get; set; }
            public string Mango { get; set; }
        }

        // Types named so that declaration order (Zeta, Alpha, Mu) differs from
        // alphabetical order, used to pin the emitted type order.
        public class DetZeta { public int Value { get; set; } }
        public class DetAlpha { public int Value { get; set; } }
        public class DetMu { public int Value { get; set; } }

        // Interfaces implemented in non-alphabetical order to exercise
        // deterministic ordering of the emitted extends list.
        public interface IDetBaseZeta { }
        public interface IDetBaseAlpha { }
        public interface IDetBaseMu { }
        public class DetImplementor : IDetBaseZeta, IDetBaseMu, IDetBaseAlpha
        {
            public int Value { get; set; }
        }

        #endregion

        /// <summary>
        /// Members must be emitted in stable reflection order (metadata token, i.e. declaration
        /// order) when neither ReorderMembers nor explicit Order is used - not alphabetical and
        /// not whatever order the runtime's reflection happens to return. Guards OrderMembers.
        /// </summary>
        [Fact]
        public void DeterministicMemberOrder()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
    export interface IDetMembers
    {
        Zebra: string;
        Apple: string;
        Mango: string;
    }
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<DetMembers>().WithPublicProperties();
            }, result);
        }

        /// <summary>
        /// Fluently configured types must be emitted in a stable order (the order they were
        /// registered), independent of dictionary/reflection iteration order. Guards the
        /// ProjectBlueprint insertion-ordered blueprint list and namespace/type emission order.
        /// </summary>
        [Fact]
        public void DeterministicTypeOrder()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
    export interface IDetZeta
    {
        Value: number;
    }
    export interface IDetAlpha
    {
        Value: number;
    }
    export interface IDetMu
    {
        Value: number;
    }
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterfaces(new[] { typeof(DetZeta), typeof(DetAlpha), typeof(DetMu) },
                    c => c.WithPublicProperties());
            }, result);
        }

        /// <summary>
        /// The list of implemented interfaces must be emitted in a deterministic order (by type
        /// key, here alphabetically) regardless of the order they appear in the C# declaration or
        /// the order reflection returns them. Guards OrderTypes applied to _GetInterfaces().
        /// </summary>
        [Fact]
        public void DeterministicInterfaceExtendsOrder()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
    export interface IDetImplementor extends Reinforced.Typings.Tests.SpecificCases.IDetBaseAlpha, Reinforced.Typings.Tests.SpecificCases.IDetBaseMu, Reinforced.Typings.Tests.SpecificCases.IDetBaseZeta
    {
        Value: number;
    }
    export interface IDetBaseZeta
    {
    }
    export interface IDetBaseAlpha
    {
    }
    export interface IDetBaseMu
    {
    }
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterfaces(
                    new[] { typeof(DetImplementor), typeof(IDetBaseZeta), typeof(IDetBaseAlpha), typeof(IDetBaseMu) },
                    c => c.WithPublicProperties());
            }, result);
        }
    }
}
