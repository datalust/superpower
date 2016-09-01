# Superpower

A highly-experimental parser combinator library. The code in this repository is not ready for any kind of consumption - I'm currently just sketching some ideas and exploring the feasibility of meeting the following high-level goals:

 [ ] Easy migration path from Sprache
   - Migrate/re-use high-level grammars
   - Migrate/re-use low-level parsers
 [ ] Better error messages: instead of `unexpected '1', expected 'a'` get `unexpected number '123', expected keyword 'and'`
 [ ] At least as fast as Sprache, ideally faster
 [ ] Rethink some awkward APIs: `A.XOr(B)` -> `A.Or(B)`, `A.Or(B)` -> `A.Try().Or(B)`
