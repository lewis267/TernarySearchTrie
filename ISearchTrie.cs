using System.Collections.Generic;

namespace Global.SearchTrie
{
    /// <summary>
    /// A Trie is an efficient data structure for the lookup 
    /// of a large set of strings. Lookups can be performed
    /// with placeholders in a Unix fashion:
    /// * matches any sequence of charcters.
    /// ? matches any one character.
    /// 
    /// The lookup, search, and deletion time complexities 
    /// are dependent on specific implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearchTrie<T> : ICollection<string>
    {
        /// <summary>
        /// The number of unique strings contained in this datastructure.
        /// </summary>
        new ulong Count { get; }

        /// <summary>
        /// Returns whether the datastructure is modifiable.
        /// </summary>
        new bool IsReadOnly { get; }

        /// <summary>
        /// Insert an item into the data structure.
        /// </summary>
        /// <param name="item"></param>
        new void Add(string identifier);
        /// <summary>
        /// Insert an item into the data structure.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="identifier"></param>
        void Add(string identifier, T item);

        /// <summary>
        /// Removes all items from the data structure.
        /// </summary>
        new void Clear();

        /// <summary>
        /// Determines whether the string pattern matches any items.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        new bool Contains(string pattern);

        /// <summary>
        /// Copies the strings of the data structure to an Array,
        /// starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is
        /// the destination of the elements copied from the data
        /// structure. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at
        /// which copying begins.</param>
        /// <exception cref="ArgumentNullException">When array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">When <paramref name="array"/> is multidimensional.</exception>
        new void CopyTo(string[] array, int arrayIndex);

        /// <summary>
        /// Removes any item from the data structure that matches and return it.
        /// </summary>
        /// <param name="pattern">A pattern to match against items in the
        /// data structure.</param>
        /// <returns>true if items were successfully removed from the data structure.
        /// Otherwise, false. This method also returns false if item is not found in
        /// the original data structure.</returns>
        new bool Remove(string pattern);
    }
}
