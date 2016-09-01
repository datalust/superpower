using Superpower.Model;

namespace Superpower
{
    public delegate Result<T> CharParser<T>(StringSpan input);
}