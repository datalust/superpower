using Superpower.Model;

namespace Superpower
{
    public delegate CharResult<T> CharParser<T>(StringSpan input);
}