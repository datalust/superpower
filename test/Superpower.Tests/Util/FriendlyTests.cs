using Superpower.Util;
using Xunit;

namespace Superpower.Tests.Util
{
    public class FriendlyTests
    {
        [Fact]
        public void FriendlyListsPreserveOrderButRemoveDuplicates()
        {
            var actual = Friendly.List(new[] {"one", "two", "two", "one", "three"});
            const string expected = "one, two or three";
            Assert.Equal(expected, actual);
        }
    }
}
