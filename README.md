# TernarySearchTrie
C# implementation of a Ternary Search Trie using a balanced Red-Black Tree for sub-level distribution.

## Construction
A new `TernarySearchTrie` is constructed like the following:

```C#
TernarySearchTrie<char, int> Trie = new TernarySearchTrie<char, int>();
```

The first type argument defines the pieces of the 'key' in the Dictionary-like `TernarySearchTrie`. The second argument specifies what will be stored as the values in the Trie.

In this manner, one can store sequences of uncommon or custom classes in the Trie's structure. Eg.:
```C#
var ByteSequenceTrie = new TernarySearchTrie<byte, Message>();
var IntervalTrie = new TernarySearchTrie<Steps, Action>();
```

## Collisions
Collisions with same keys are handled with linked lists. 
Removing an item from the Trie by `key` will remove the whole list.
Removing an item from the Trie by `KeyValuePair<key,value>` will remove only the specified value or the whole list if it has only one value.

## Enumeration
For enumeration, the Trie uses yeilding while recursively traversing.

# Pattern Lookup
`PatternDictionary` inherits from `TernaryTrie` and is useful for storing patterns which may contain elements defined as:
 * matching any sequence of elements (eg. `*`)
 * matching one element (eg. `?`)
 
Inserted patterns can then be collected by an instance and their stored values be returned in a list.

This functionality is specifically opposite of the functionality of Regex which collects multiple instances which match a single pattern.
 
## `PatternDictionary` Construction
The `PatternDictionary` constructor allows for the definition of the _match-any-or-none_ and the _match-any-one_ elements.

```C#
var pd = new PatternDictionary<char, double>('?', '*');
var CatPatterns = new PatternDictionary<CatPart, string>(Cat.GenericPart, Cat.GenericPartSequence);
```
