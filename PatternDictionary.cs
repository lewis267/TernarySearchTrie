using Global.SearchTrie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWaySearchTrie
{
    /// <summary>
    /// A class for the storage and lookup of patterns.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TKeyPiece"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PatternDictionary<TKey, TKeyPiece, TValue>
        : TernarySearchTrie<TKey, TKeyPiece, TValue>
        where TKey : IEnumerable<TKeyPiece> where TKeyPiece : IComparable
    {

        /// <summary>
        /// The piece that matches any TKeyPiece.
        /// </summary>
        public TKeyPiece genericPiece;
        /// <summary>
        /// The piece that matches any ending of a TKey.
        /// </summary>
        public TKeyPiece genericSeriesPiece;

        public PatternDictionary(TKeyPiece generic, TKeyPiece series)
        {
            genericPiece = generic;
            genericSeriesPiece = series;
        }

        protected new List<TValue> collect(Node n, IList<TKeyPiece> pieces, int idx)
        {
            List<TValue> items = new List<TValue>();
            if (n == null) return items;

            if (idx < pieces.Count)
            {

            }
            else if (n.IsContainer) items.AddRange(n.Values);

            return items;
        }


    }
}
