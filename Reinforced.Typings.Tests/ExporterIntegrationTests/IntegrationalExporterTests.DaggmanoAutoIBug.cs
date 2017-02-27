using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void DaggmanoAutoIBug()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export interface IInternalUserDaggmano
	{
		Name: string;
	}
	export interface IExternalUserDaggmano
	{
		Name: string;
	}
	export interface IAlreadyContainsI
	{
		Name: string;
	}
	export interface IDoesntContainI
	{
		Name: string;
	}
	export class DoNotNeedIAtAll
	{
		public Name: string;
	}
	export class ICannotBeRemovedHere
	{
		public Name: string;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<InternalUserDaggmano>().WithPublicProperties();
                s.ExportAsInterface<ExternalUserDaggmano>().WithPublicProperties();
                s.ExportAsInterface<IAlreadyContainsI>().WithPublicProperties();
                s.ExportAsInterface<DoesntContainI>().WithPublicProperties();
                s.ExportAsClass<DoNotNeedIAtAll>().WithPublicProperties();
                s.ExportAsClass<ICannotBeRemovedHere>().WithPublicProperties();
            }, result);
        }
    }
}