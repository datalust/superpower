using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class OrCombinatorTests
    {
        [Fact]
        public void OrFailsWithNone()
        {
            AssertParser.Fails(Parse.Char('a').Or(Parse.Char('b')), "");
        }

        [Fact]
        public void OrFailsWithUnmatched()
        {
            AssertParser.Fails(Parse.Char('a').Or(Parse.Char('b')), "c");
        }

        [Fact]
        public void OrSucceedsWithFirstMatch()
        {
            AssertParser.SucceedsWith(Parse.Char('a').Or(Parse.Char('b')), "a", 'a');
        }

        [Fact]
        public void OrSucceedsWithSecondMatch()
        {
            AssertParser.SucceedsWith(Parse.Char('a').Or(Parse.Char('b')), "b", 'b');
        }

        [Fact]
        public void OrFailsWithPartialFirstMatch()
        {
            AssertParser.Fails(Parse.Char('a').Then(_ => Parse.Char('b')).Or(Parse.Char('a')), "a");
        }

        [Fact]
        public void TokenOrFailsWithNone()
        {
            AssertParser.Fails(Parse.Token('a').Or(Parse.Token('b')), "");
        }

        [Fact]
        public void TokenOrFailsWithUnmatched()
        {
            AssertParser.Fails(Parse.Token('a').Or(Parse.Token('b')), "c");
        }

        [Fact]
        public void TokenOrSucceedsWithFirstMatch()
        {
            AssertParser.SucceedsWith(Parse.Token('a').Or(Parse.Token('b')), "a", 'a');
        }

        [Fact]
        public void TokenOrSucceedsWithSecondMatch()
        {
            AssertParser.SucceedsWith(Parse.Token('a').Or(Parse.Token('b')), "b", 'b');
        }

        [Fact]
        public void TokenOrFailsWithPartialFirstMatch()
        {
            AssertParser.Fails(Parse.Token('a').Then(_ => Parse.Token('b')).Or(Parse.Token('a')), "a");
        }
    }
}
