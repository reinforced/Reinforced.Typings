namespace Reinforced.WebTypings
{
    public interface ITsCodeGenerator<in TElement>
    {
        void Generate(TElement element, TypeResolver resolver, WriterWrapper sw);
    }
}
