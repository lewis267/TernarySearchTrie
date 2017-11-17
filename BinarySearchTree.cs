using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global.BinaryTree
{
    class BinarySearchTree<T> : ITree<T> where T : IComparable
    {
        private BinaryTreeNode root = null;
        private bool readOnly = false;
        private int count = 0;
        private object dataLock;
        private bool modified;

        public BinarySearchTree()
        {
        }
        /// <summary>
        /// Construct a new Binary Search Tree from an existing tree using shallow copy.
        /// </summary>
        /// <param name="oldTree">The origninal tree to reference.</param>
        /// <param name="readOnly">Make the tree read only.</param>
        public BinarySearchTree(BinarySearchTree<T> oldTree, bool readOnly = false)
        {
            root = oldTree.root;
            this.readOnly = readOnly;
            count = oldTree.count;
        }

        private BinaryTreeNode find(T item)
        {
            BinaryTreeNode current = root;
            while (current != null)
            {
                int comp = current.Data.CompareTo(item);

                if      (comp < 0) current = (BinaryTreeNode)current.left;
                else if (comp > 0) current = (BinaryTreeNode)current.right;
                else               break;
            }
            return current;
        }
        private bool isRight(ITreeNode<T> n)
        {
            if (n == null) return false;
            if (n.parent == null) return false;

            if (n.parent.right == n) return true;
            else return false;
        }
        private ITreeNode<T> remove(ITreeNode<T> n, T item)
        {
            if (n == null) return n;

            if (item.CompareTo(n.Data) < 0) n.left = remove(n.left, item);
            else if (item.CompareTo(n.Data) > 0) n.right = remove(n.right, item);
            else
            {
                count--;
                if (n.left == null) return n.right;
                else if (n.right == null) return n.left;

                n.Data = smallest(n.right);
                n.right = remove(n.right, n.Data);
            }

            return n;
        }
        private T smallest(ITreeNode<T> n)
        {
            T min = n.Data;
            while (n.left != null)
            {
                min = n.left.Data;
                n = n.left;
            }
            return min;
        }

        #region --- IBinaryTree Functions ---

        /// <summary>
        /// Returns the number of nodes in the tree.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Specifies whether the tree can be modified.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        public ITreeNode<T> Root
        {
            get
            {
                return root;
            }

            set
            {
                if (readOnly) return;
                modified = true;

                root = (BinaryTreeNode)value;
            }
        }

        /// <summary>
        /// Inserts an item into the tree structure.
        /// 
        /// Overwrites any existing item.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (readOnly) return;
            modified = true;

            if (root == null) { root = new BinaryTreeNode(item); return; }

            lock (dataLock) // Only let me play alone.
            {
                BinaryTreeNode current = root;
                while (current != null)
                {
                    int comp = current.Data.CompareTo(item);
                    if (comp < 0)
                    {
                        if (current.left == null)
                        {
                            current.left = new BinaryTreeNode(item);
                            break;
                        }
                        else current = (BinaryTreeNode)current.left;
                    }
                    else if (comp > 0)
                    {
                        if (current.right == null)
                        {
                            current.right = new BinaryTreeNode(item);
                            break;
                        }
                        else current = (BinaryTreeNode)current.right;
                    }
                    else current.Data = item;
                }
            }
        }

        /// <summary>
        /// Dereferences the tree from the root.
        /// </summary>
        public void Clear()
        {
            if (readOnly) return;

            modified = true;
            lock (dataLock)
            {
                root = null; // garbage collection should be able to handle the rest...
            }
        }

        /// <summary>
        /// Checks if the given item is contained in the tree.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return this[item].CompareTo(default(T)) != 0;
        }

        /// <summary>
        /// Copies the data strucure to the given array starting at a given index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex");
            if (count > array.Length - arrayIndex)
                throw new ArgumentException(
                    "The number of elements in the data structure is greater than the available space from arrayIndex to the end of the destination array."
                    );

            lock (dataLock)
            {
                copyTo(root, ref array, ref arrayIndex);
            }
        }
        private void copyTo(BinaryTreeNode n, ref T[] array, ref int idx)
        {
            if (n == null) return;
            if (n.left != null) copyTo((BinaryTreeNode)n.left, ref array, ref idx);
            array[idx++] = n.Data;
            if (n.right != null) copyTo((BinaryTreeNode)n.right, ref array, ref idx);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new InorderTreeEnumerator(this);
        }

        public bool Remove(T item)
        {
            if (readOnly) return false;
            modified = true;
            int c = count;

            lock (dataLock) // Only let me play alone
            {
                root = (BinaryTreeNode)remove(root, item);
            }

            return c < count;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Accesses items in the tree equivalent to the given value.
        /// </summary>
        /// <param name="value">The item to search for.</param>
        /// <returns>The item if it exists, or default(T) otherwise.</returns>
        public T this[T item]
        {
            get // no locks needed
            {
                BinaryTreeNode n = find(item);
                return n == null ? default(T) : n.Data;
            }
        }

        internal class BinaryTreeNode : ITreeNode<T>
        {
            public BinaryTreeNode(T item)
            {
                Data = item;
                left = null;
                right = null;
                parent = null;
            }

            /// <summary>
            /// Intrinsic value.
            /// </summary>
            public T Data { get; set; }

            /// <summary>
            /// Left child.
            /// </summary>
            public ITreeNode<T> left { get; set; }

            /// <summary>
            /// Parent node.
            /// </summary>
            public ITreeNode<T> parent { get; set; }

            /// <summary>
            /// Right child.
            /// </summary>
            public ITreeNode<T> right { get; set; }

            public int CompareTo(ITreeNode<T> other)
            {
                return Data.CompareTo(other.Data);
            }
        }

        /// <summary>
        /// An enumerator for inorder tree traversal.
        /// </summary>
        internal class InorderTreeEnumerator : IEnumerator<T>
        {
            private BinarySearchTree<T> tree;
            private BinaryTreeNode current;
            private T[] items;
            private int index = -1;

            private bool goingDown = true;
            private bool goingLeft = true;

            public InorderTreeEnumerator(BinarySearchTree<T> tree)
            {
                this.tree = tree;
                tree.modified = false;

                tree.CopyTo(items, 0);
            }

            public T Current
            {
                get
                {
                    if (items == null || index < 0 || index >= items.Length)
                        throw new InvalidOperationException();

                    return items[index];
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

            private bool disposedValue = false;
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // Dispose of managed resources.
                    }
                    items = null;
                }

                disposedValue = true;
            }

            public bool MoveNext()
            {
                if (current == null) return false;

                index++;
                return index < items.Length;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}
