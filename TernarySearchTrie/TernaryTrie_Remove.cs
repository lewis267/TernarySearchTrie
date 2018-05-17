using System;
using System.Collections.Generic;

namespace Global.SearchTrie
{
    public partial class TernarySearchTrie<TKeyPiece, TValue> : IDictionary<IEnumerable<TKeyPiece>, IList<TValue>>
        where TKeyPiece : IComparable
    {
        #region --- public ---

        /// <summary>
        /// Remove a KeyValuePair if it exists in the dictionary.
        /// </summary>
        /// <param name="item">The pair to remove from the collection.</param>
        /// <returns>True if the removal was successful and and item was
        /// removed from the Trie.</returns>
        public bool Remove(KeyValuePair<IEnumerable<TKeyPiece>, IList<TValue>> item)
        {
            if (item.Key == null)
                throw new ArgumentException(nameof(item));

            root = Remove(root, item.Key.GetEnumerator(), item.Value, out bool mod);
            if (mod)
                modified = true;

            return mod;
        }

        /// <summary>
        /// Removes an item from the Trie by Key value.
        /// </summary>
        /// <param name="key">The Key to remove from the Trie.</param>
        public bool Remove(IEnumerable<TKeyPiece> key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            root = Remove(root, key.GetEnumerator(), out bool mod);
            if (mod)
                modified = true;

            return mod;
        }
        
        #endregion

        #region --- private ---

        /// <summary>Uses an Enumerator(<typeparamref name="TKeyPiece"/>)
        /// to advance through the <typeparamref name="TKey"/></summary>
        protected Node Remove(Node node, IEnumerator<TKeyPiece> enmrtr, out bool mod)
        {
            if (enmrtr.MoveNext())
            {
                TKeyPiece now = enmrtr.Current;

                if (node.Next == null)
                {
                    mod = false;
                    return node;
                }

                // Recurse
                if (node.Next.ContainsKey(now))
                    node.Next[now] = Remove(node.Next[now], enmrtr, out mod);
                else
                    mod = false;
            }
            else
            {
                mod = false;
                if (node.IsContainer)
                {
                    node.Values.Clear();
                    node.IsContainer = false;
                    size--;
                    mod = true;
                }
            }
            return node;
        }

        /// <summary>
        /// Removes a Node with the given key and the given set of values.
        /// Otherwise it does nothing and mod is set to false.
        /// </summary>
        /// <param name="node">The node to work with on this level of
        /// recursion.</param>
        /// <param name="enmrtr">The instance of an enumerator to move through
        /// the tree.</param>
        /// <param name="value">The set of values in the pair to remove.</param>
        /// <param name="mod">Flag to show whether the tree was modified.</param>
        /// <returns>The Node we worked with.</returns>
        protected Node Remove(Node node, IEnumerator<TKeyPiece> enmrtr, IList<TValue> value, out bool mod)
        {
            if (enmrtr.MoveNext())
            {
                TKeyPiece now = enmrtr.Current;

                if (node.Next == null)
                {
                    mod = false;
                    return node;
                }

                // Recurse
                if (node.Next.ContainsKey(now))
                    node.Next[now] = Remove(node.Next[now], enmrtr, out mod);
                else
                    mod = false;
            }
            else
            {
                for (int i = 0; i < value.Count; i++)
                    if (!value[i].Equals(node.Values[i]))
                    {
                        mod = false;
                        return node;
                    }

                mod = false;
                if (node.IsContainer)
                {
                    node.Values.Clear();
                    node.IsContainer = false;
                    size--;
                    mod = true;
                }
                
            }
            return node;
        }

        #endregion
    }
}
