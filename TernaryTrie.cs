using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Global.SearchTrie
{
    /// <summary>
    /// This is the structure for a balanced ternary search trie which can be used
    /// on any appropriate data types.
    /// 
    /// It uses an efficient tree structure (<paramref name="SortedDictionary"/> eg. Red-Black Tree)
    /// for indexing at a level to maintain l*log(n) lookup speeds, where 
    /// <code>l = the average length of a word</code> and 
    /// <code>n = the number of items in the structure.</code> 
    /// </summary>
    /// <typeparam name="TKey">The item to be searched for (eg. string's).</typeparam>
    /// <typeparam name="TKeyPiece">The peices that the <paramref name="TKey"/> will be split into that are comparable (eg. char's).</typeparam>
    /// <typeparam name="TValue">The object to relate with <paramref name="TKey"/>.</typeparam>
    public class TernarySearchTrie<TKey, TKeyPiece, TValue> : IDictionary<TKey, TValue> where TKey : IEnumerable<TKeyPiece> where TKeyPiece : IComparable
    {
        private Node root;
        private int size = 0;
        private bool modified = false;

        /// <summary>Represents a node of the tree.</summary>
        /// <remarks>Using fields instead of properties drops execution time by about 40%.</remarks>
        [DebuggerDisplay("Key={Key}, Value={Value}, Size={Next.Count}")]
        private class Node
        {
            /// <summary>Gets or sets the node's key.</summary>
            public TKeyPiece Key;

            /// <summary>Gets or sets the node's value.</summary>
            public TValue Value;

            /// <summary>Gets or sets the next Node down.</summary>
            public SortedDictionary<TKeyPiece, Node> Next;

            /// <summary>Marks this node as containing data or not.</summary>
            public bool IsContainer = false;

            /// <summary>Set when this node is a container.</summary>
            public TKey repValue = default(TKey);
        }

        /// <summary>
        /// Searches for a given pattern in the datastructure.
        /// '*' is a open matcher.
        /// '?' matches one char.
        /// </summary>
        /// <param name="pattern">The pattern to match to.</param>
        /// <param name="ignoreCase">Set to true to ignore case when searching.</param>
        /// <returns></returns>
        public List<TValue> Search(TKey pattern)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Searches through the datastructure for any items that 
        /// match the given pattern. If any <typeparamref name="TKeyPiece"/> 
        /// is equivalent to <code>default(</code><typeparamref name="TKeyPiece"/>
        /// <code>)</code> then it will be matched to any.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<TValue> Search(List<TKeyPiece> pattern)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds the given <typeparamref name="TKey"/>/<typeparamref name="TValue"/>
        /// pair to the Trie.</summary>
        /// <param name="key">The location to place to <paramref name="item"/>.</param>
        /// <param name="item">The object to place in the trie.</param>
        public void Add(TKey key, TValue item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            modified = true;
            root = Add(item, root, key.GetEnumerator(), key);
            size++;
        }
        /// <summary>Adds the given <typeparamref name="TKey"/>/<typeparamref name="TValue"/>
        /// pair to the Trie.</summary>
        /// <param name="key">The location to place to <paramref name="item"/>.</param>
        /// <param name="item">The object to place in the trie.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }
        /// <summary>Uses an Enumerator(<typeparamref name="TKeyPiece"/>)
        /// to advance through the <typeparamref name="TKey"/></summary>
        private Node Add(TValue item, Node node, IEnumerator<TKeyPiece> enmrtr, TKey key)
        {
            if (enmrtr.MoveNext())
            {
                TKeyPiece now = enmrtr.Current;

                if (node.Next == null)
                    node.Next = new SortedDictionary<TKeyPiece, Node>();

                // Recurse
                if (node.Next.ContainsKey(now))
                    node.Next[now] = Add(item, node.Next[now], enmrtr, key);
                else
                {
                    // Make a new node
                    Node n = new Node();
                    n.Key = now;
                    n.Next = new SortedDictionary<TKeyPiece, Node>();

                    // Go down a level
                    node.Next[now] = Add(item, n, enmrtr, key);
                }
            }
            else
            {
                node.Value = item; // Assign the value
                node.IsContainer = true;
                node.repValue = key;
            }
            return node;
        }

        /// <summary>
        /// Obtains the number of unique items in the Trie.
        /// </summary>
        public int Count
        {
            get { return size; }
        }

        /// <summary>
        /// Iterates over the Trie and collects the keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                ICollection<TKey> keys = new List<TKey>();
                foreach (KeyValuePair<TKey, TValue> item in this)
                {
                    keys.Add(item.Key);
                }
                return keys;
            }
        }

        /// <summary>
        /// Iterates over the Trie and collects the values.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                ICollection<TValue> values = new List<TValue>();
                foreach (KeyValuePair<TKey, TValue> item in this)
                {
                    values.Add(item.Value);
                }
                return values;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Obtains the <typeparamref name="TValue"/> for the given <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The <typeparamref name="TValue"/>for the given <typeparamref name="TKey"/>.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return Search(key)[0];
            }
            set
            {
                Add(key, value);
            }
        }

        /// <summary>
        /// Removes an item from the Trie by Key value.
        /// </summary>
        /// <param name="key"></param>
        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentException(nameof(key));

            bool mod;
            root = Remove(root, key.GetEnumerator(), out mod);
            size--;
            if (mod) modified = true;

            return mod;
        }
        /// <summary>Uses an Enumerator(<typeparamref name="TKeyPiece"/>)
        /// to advance through the <typeparamref name="TKey"/></summary>
        private Node Remove(Node node, IEnumerator<TKeyPiece> enmrtr, out bool mod)
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
                node.Value = default(TValue); // Assign the value
                node.IsContainer = false;
                mod = true;
            }
            return node;
        }
        /// <summary>
        /// Ignores the <typeparamref name="TValue"/> or value part of the
        /// <paramref name="item"/>, and removes based only on the 
        /// <typeparamref name="TKey"/> or key part of the <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Removes everything from the data structure.
        /// </summary>
        public void Clear()
        {
            modified = true;
            root = new Node();
            size = 0;
        }

        /// <summary>
        /// Checks if the given <typeparamref name="TKey"/> has been previously been added into the Trie.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return ContainsKey(root, key.GetEnumerator());
        }
        private bool ContainsKey(Node node, IEnumerator<TKeyPiece> enmrtr)
        {
            if (enmrtr.MoveNext())
            {
                TKeyPiece now = enmrtr.Current;

                if (node.Next == null)
                    return false;

                // Recurse
                if (node.Next.ContainsKey(now))
                    return ContainsKey(node.Next[now], enmrtr);
                else
                    return false;
            }
            else
            {
                return node.IsContainer;
            }
        }

        /// <summary>
        /// Efficiently obtains a <typeparamref name="TValue"/> when 
        /// unsure about its existence in the data structure.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <param name="value">The value to assigned to.</param>
        /// <returns>True if found.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            List<TValue> vals = Search(key);
            if (vals.Count > 0)
            {
                value = vals[0];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Checks if a given pair are in the trie.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            List<TValue> vals = Search(item.Key);
            return vals.Contains(item.Value);
        }

        /// <summary>
        /// Copies the elements of the Trie to an array of type 
        /// KeyValuePair(TKey, TValue), starting at the specified array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="ArgumentNullException"><paramref name="nameof(array)"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="nameof(arrayIndex)"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source Trie is greater than the 
        /// available space from index to the end of the destination array.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (size - arrayIndex > array.Length) throw new ArgumentException("Not enough space in the given array.");

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                array[arrayIndex++] = pair;
            }
        }

        #region --- IEnumeration Interface Functions ---

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new TernaryTrieEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TernaryTrieEnumerator(this);
        }

        public class TernaryTrieEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            // A reference to the trie
            private TernarySearchTrie<TKey, TKeyPiece, TValue> trie;

            // Access controlled current value
            private KeyValuePair<TKey, TValue> _c;
            private object _currentLock;
            private KeyValuePair<TKey, TValue> current
            {
                get
                {
                    lock (_currentLock)
                        return _c;
                }
                set
                {
                    lock (_currentLock)
                        _c = value;
                }
            }

            // Recursive stack for iteration over trie
            private Thread iteratorThread;
            private AutoResetEvent waitHandle = new AutoResetEvent(false);

            // Disposing flag
            private bool disp = false;

            /// <summary>
            /// Obtains the current item being referenced.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    lock (_currentLock)
                        return current;
                }
            }
            private object Current1
            {
                get { return Current; }
            }
            object IEnumerator.Current
            {
                get
                {
                    return Current1;
                }
            }

            /// <summary>
            /// This function does some simple things.
            /// 
            /// Mainly it traverses the Trie left to right, recursively.
            /// Thus the left-most will be visited first and the
            /// right-most nodes last.
            /// 
            /// Whenever it encounters an <typeparamref name="TValue"/>
            /// it will assign it to the current value.
            /// 
            /// A special feature of this function is that it calls 
            /// WaitOne() on a waitHandle object so that the function
            /// stops after it arrives at the next <typeparamref name="TValue"/>
            /// and thus can be used by the function MoveNext().
            /// </summary>
            /// <param name="node"></param>
            private void threadFunction(Node node)
            {
                foreach (KeyValuePair<TKeyPiece, Node> pair in node.Next)
                {
                    if (disp) return;

                    // Recurse first
                    if (pair.Value != null)
                        threadFunction(pair.Value);

                    // Check if this is an instance of a pair
                    if (pair.Value.IsContainer)
                    {
                        TKey key = pair.Value.repValue;
                        TValue value = pair.Value.Value;

                        // set the current item
                        current = new KeyValuePair<TKey, TValue>(key, value);

                        // wait to be prompted to continue
                        waitHandle.WaitOne();

                        // reset and continue
                        waitHandle.Reset();
                    }
                }
            }

            // Constructor
            public TernaryTrieEnumerator(TernarySearchTrie<TKey, TKeyPiece, TValue> trie)
            {
                this.trie = trie;
                this.trie.modified = false;
                current = default(KeyValuePair<TKey, TValue>);

                // Create a new Thread and initialize its stack. (Do not start)
                iteratorThread = new Thread(() => { threadFunction(trie.root); });
            }

            /// <summary>
            /// Prompts the next item to be referenced by Current.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                // Check for modifications
                if (trie.modified)
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                // If this is the first call to MoveNext() we need to start the iteratorThread.
                if (iteratorThread.ThreadState == System.Threading.ThreadState.Unstarted)
                    iteratorThread.Start();

                // Otherwise we will signal the thread to continue its event stack operations.
                else
                    waitHandle.Set();

                // We need to wait until the thread is finished Running.
                while (iteratorThread.ThreadState == System.Threading.ThreadState.Running)
                    ;

                // NOTE: Threads can be in multiple states so be careful using != to test for state.

                // Figure out what to return based on the thread state.
                if (iteratorThread.ThreadState == System.Threading.ThreadState.Stopped)
                    return false;
                else if (iteratorThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                    return true;
                else
                    throw new ThreadStateException(iteratorThread.ThreadState.ToString());
            }

            /// <summary>
            /// Resets the enumerator to start from the beginning of the Trie.s
            /// </summary>
            public void Reset()
            {
                trie.modified = false;
                current = default(KeyValuePair<TKey, TValue>);

                // Create a new Thread and initialize its stack. (Do not start)
                iteratorThread = new Thread(() => { threadFunction(trie.root); });
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.
                    disp = true;
                    waitHandle.Set();
                    iteratorThread = null;
                    trie = null;
                    waitHandle.Dispose();

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            ~TernaryTrieEnumerator()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(false);
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }

        #endregion
    }
}
