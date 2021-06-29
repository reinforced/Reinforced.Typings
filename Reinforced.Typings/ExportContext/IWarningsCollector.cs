using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings
{
    internal interface IWarningsCollector
    {
        void AddWarning(RtWarning warning);
    }
}