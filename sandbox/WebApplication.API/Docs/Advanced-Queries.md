## Summary
This PR includes advanced queries that allow to decide comparison type from client/request sender.
Check it out some examples:
- `/books?title.contains=thief`
- `/books?category.not=4`
- `/books?price.lte=15`

And amazing thing is you can combine:
`/books?title.contains=thief&category.not=4&price.lte=15` 

Also relared with #9 

## Deprecated
⚠ **Range<T>** is deprecated and no longer be maintained and will be removed next major version.

## New Types
There are 2 new types in this PR. There are **OperatorFilter<T>** for struct types and **StringFilter** for strings as its name describes itself :).

Also you may see:
 - [OperatorFilter](OperatorFilter)
 - [StringFilter](StringFilter)
