# Superpower [![Build status](https://ci.appveyor.com/api/projects/status/366q7k3nv4fiuwuv?svg=true)](https://ci.appveyor.com/project/NicholasBlumhardt/superpower)

[![Join the chat at https://gitter.im/datalust/superpower](https://badges.gitter.im/datalust/superpower.svg)](https://gitter.im/datalust/superpower?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

A highly-experimental parser combinator library. The code in this repository is not ready for any kind of consumption - I'm currently just sketching some ideas and exploring the feasibility of meeting the following high-level goals:

 * [ ] Easy migration path from [Sprache](https://github.com/Sprache/Sprache)
   - Migrate/re-use high-level grammars
   - Migrate/re-use low-level parsers
   - Migrate/Recompile simple parsers as-is
 * [ ] Better error messages: instead of `unexpected '1', expected 'a'` get `unexpected number '123', expected keyword 'and'`
 * [ ] At least as fast as Sprache, ideally faster
 * [ ] Rethink some awkward APIs: `A.XOr(B)` -> `A.Or(B)`, `A.Or(B)` -> `A.Try().Or(B)`
 * [ ] Handle `/* comments */` of all kinds more elegantly
 * [ ] Make it easier to avoid backtracking in grammars that require some lookahead (e.g. `"not"|"null"`
 * [ ] Separate provided parsers, which are grammar/language/locale-specific, from generic combinators
 * [ ] Only high-level API types exposed in the root namespace

_The repository's working title arose out of a talk "Parsing Text: the Programming Superpower You Need at Your Fingertips" given at DDD Brisbane 2015._
