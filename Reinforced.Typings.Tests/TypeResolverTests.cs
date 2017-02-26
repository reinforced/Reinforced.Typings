using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.TypeNames;
using Xunit;

namespace Reinforced.Typings.Tests
{
    public class TypeResolverTests
    {
        private readonly TypeResolver _tr;
        private readonly TypeNameEqualityComparer _comparer;

        private readonly ExportContext _context;

        private static readonly RtSimpleTypeName AnyType = new RtSimpleTypeName("any");
        private static readonly RtSimpleTypeName NumberType = new RtSimpleTypeName("number");
        private static readonly RtSimpleTypeName StringType = new RtSimpleTypeName("string");
        private static readonly RtArrayType AnyArrayType = new RtArrayType(AnyType);

        public TypeResolverTests()
        {
            _context = new ExportContext()
            {
                SourceAssemblies = new[] { Assembly.GetExecutingAssembly() }
            };
            var exporter = new TsExporter(_context);
            exporter.Initialize();
            _tr = exporter.TypeResolver;
            _comparer = new TypeNameEqualityComparer();
        }

        [Fact]
        public void NumericsToNumber()
        {
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(int)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(uint)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(long)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(ulong)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(short)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(ushort)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(byte)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(double)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(float)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(decimal)), _comparer);
        }

        [Fact]
        public void StringsAndCharsToString()
        {
            Assert.Equal(StringType, _tr.ResolveTypeName(typeof(string)), _comparer);
            Assert.Equal(StringType, _tr.ResolveTypeName(typeof(char)), _comparer);
        }

        [Fact]
        public void NullableNumbersToNumber()
        {
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(int?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(uint?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(long?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(ulong?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(short?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(ushort?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(byte?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(double?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(float?)), _comparer);
            Assert.Equal(NumberType, _tr.ResolveTypeName(typeof(decimal?)), _comparer);
        }

        [Fact]
        public void VoidToVoid()
        {
            Assert.Equal(new RtSimpleTypeName("void"), _tr.ResolveTypeName(typeof(void)), _comparer);
        }

        [Fact]
        public void ObjectToAny()
        {
            Assert.Equal(AnyType, _tr.ResolveTypeName(typeof(object)), _comparer);
        }

        [Fact]
        public void NongenericCollectionsToAnyArray()
        {
            Assert.Equal(AnyArrayType, _tr.ResolveTypeName(typeof(IEnumerable)), _comparer);
            Assert.Equal(AnyArrayType, _tr.ResolveTypeName(typeof(IQueryable)), _comparer);
            Assert.Equal(AnyArrayType, _tr.ResolveTypeName(typeof(IList)), _comparer);
            Assert.Equal(AnyArrayType, _tr.ResolveTypeName(typeof(Stack)), _comparer);
            Assert.Equal(AnyArrayType, _tr.ResolveTypeName(typeof(Queue)), _comparer);
            Assert.Equal(AnyArrayType, _tr.ResolveTypeName(typeof(ArrayList)), _comparer);
            Assert.Equal(AnyArrayType, _tr.ResolveTypeName(typeof(CollectionBase)), _comparer);
        }

        [Fact]
        public void DictionaryToObject()
        {
            Assert.Equal(new RtDictionaryType(AnyType, AnyType), _tr.ResolveTypeName(typeof(IDictionary)), _comparer);
            Assert.True(_context.Warnings.Any(c => c.Code == 7));
            _context.Warnings.Clear();

            Assert.Equal(new RtDictionaryType(AnyType, AnyType), _tr.ResolveTypeName(typeof(IDictionary<object, object>)), _comparer);
            Assert.True(_context.Warnings.Any(c => c.Code == 7));
            _context.Warnings.Clear();

            Assert.Equal(new RtDictionaryType(StringType, AnyType), _tr.ResolveTypeName(typeof(IDictionary<string, object>)), _comparer);
            Assert.Equal(new RtDictionaryType(NumberType, AnyType), _tr.ResolveTypeName(typeof(IDictionary<int, object>)), _comparer);

            Assert.Equal(new RtDictionaryType(AnyType, AnyType), _tr.ResolveTypeName(typeof(Dictionary<object, object>)), _comparer);
            Assert.True(_context.Warnings.Any(c => c.Code == 7));
            _context.Warnings.Clear();

            Assert.Equal(new RtDictionaryType(StringType, AnyType), _tr.ResolveTypeName(typeof(Dictionary<string, object>)), _comparer);
            Assert.Equal(new RtDictionaryType(NumberType, AnyType), _tr.ResolveTypeName(typeof(Dictionary<int, object>)), _comparer);
        }

        private void GenericCollectionsOfType<T>(RtTypeName targetType)
        {
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(IEnumerable<T>)), _comparer);
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(IQueryable<T>)), _comparer);
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(IList<T>)), _comparer);
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(Stack<T>)), _comparer);
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(Queue<T>)), _comparer);
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(Collection<T>)), _comparer);
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(ICollection<T>)), _comparer);
            Assert.Equal(targetType, _tr.ResolveTypeName(typeof(T[])), _comparer);
        }

        [Fact]
        public void CollectionsToCorrespondingArray()
        {
            var numArray = new RtArrayType(NumberType);
            GenericCollectionsOfType<int>(numArray);
            GenericCollectionsOfType<uint>(numArray);
            GenericCollectionsOfType<long>(numArray);
            GenericCollectionsOfType<ulong>(numArray);
            GenericCollectionsOfType<short>(numArray);
            GenericCollectionsOfType<ushort>(numArray);
            GenericCollectionsOfType<byte>(numArray);
            GenericCollectionsOfType<double>(numArray);
            GenericCollectionsOfType<float>(numArray);
            GenericCollectionsOfType<decimal>(numArray);
            GenericCollectionsOfType<string>(new RtArrayType(StringType));
            GenericCollectionsOfType<bool>(new RtArrayType(new RtSimpleTypeName("boolean")));
        }

    }
}
