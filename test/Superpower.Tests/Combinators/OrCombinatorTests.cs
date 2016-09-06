using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Superpower.Tests.Support;
using Xunit;
using Superpower.Parsers;

namespace Superpower.Tests.Combinators
{
    public class OrCombinatorTests
    {
        [Fact]
        public void OrFailsWithNone()
        {
            AssertParser.Fails(Character.EqualTo('a').Or(Character.EqualTo('b')), "");
        }

        [Fact]
        public void OrFailsWithUnmatched()
        {
            AssertParser.Fails(Character.EqualTo('a').Or(Character.EqualTo('b')), "c");
        }

        [Fact]
        public void OrSucceedsWithFirstMatch()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').Or(Character.EqualTo('b')), "a", 'a');
        }

        [Fact]
        public void OrSucceedsWithSecondMatch()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').Or(Character.EqualTo('b')), "b", 'b');
        }

        [Fact]
        public void OrFailsWithPartialFirstMatch()
        {
            AssertParser.Fails(Character.EqualTo('a').Then(_ => Character.EqualTo('b')).Or(Character.EqualTo('a')), "a");
        }

        [Fact]
        public void TokenOrFailsWithNone()
        {
            AssertParser.Fails(Token.EqualTo('a').Or(Token.EqualTo('b')), "");
        }

        [Fact]
        public void TokenOrFailsWithUnmatched()
        {
            AssertParser.Fails(Token.EqualTo('a').Or(Token.EqualTo('b')), "c");
        }

        [Fact]
        public void TokenOrSucceedsWithFirstMatch()
        {
            AssertParser.SucceedsWith(Token.EqualTo('a').Or(Token.EqualTo('b')), "a", 'a');
        }

        [Fact]
        public void TokenOrSucceedsWithSecondMatch()
        {
            AssertParser.SucceedsWith(Token.EqualTo('a').Or(Token.EqualTo('b')), "b", 'b');
        }

        [Fact]
        public void TokenOrFailsWithPartialFirstMatch()
        {
            AssertParser.Fails(Token.EqualTo('a').Then(_ => Token.EqualTo('b')).Or(Token.EqualTo('a')), "a");
        }
    }
}
