using System.Collections;

namespace Loxifi
{
    /// <summary>
    /// A class that stores its backing data as a file enabling persistence.
    /// Exposes underlying data as a collection of strings
    /// </summary>
    public class ListFile : IList<string>, IDisposable
    {
        /// <summary>
        /// Returns the string found at the specified index
        /// </summary>
        /// <param name="index">The index to access</param>
        /// <returns>The string found at the specified index</returns>
        public string this[int index]
        {
            get => ((IList<string>)_backing)[index];
            set => ((IList<string>)_backing)[index] = value;
        }

        private readonly bool _autoFlush = true;

        private readonly List<string> _backing = new();

        private readonly string _path;

        private bool _disposedValue;

        /// <summary>
        /// Count of the items in the underlying collection
        /// </summary>
        public int Count => ((ICollection<string>)_backing).Count;

        /// <summary>
        /// True, if there are unpersisted changes
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// True if the backing collection is read only
        /// Should never be true
        /// </summary>
        public bool IsReadOnly => ((ICollection<string>)_backing).IsReadOnly;

        /// <summary>
        /// Constructs a new instance of this class
        /// </summary>
        /// <param name="path">The file path to store the backing data at</param>
        /// <param name="autoFlush">
        /// If true, changes will be flushed to disk with every operation
        /// Safer, but slower. Defaults to true
        /// </param>
        public ListFile(string path, bool autoFlush = true)
        {
            _path = path;
            _autoFlush = autoFlush;

            if (System.IO.File.Exists(path))
            {
                _backing = System.IO.File.ReadAllLines(path).ToList();
            }
        }

        /// <summary>
        /// Adds an item to the underlying collection
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(string item)
        {
            ((ICollection<string>)_backing).Add(item);

            if (_autoFlush)
            {
                System.IO.File.WriteAllLines(_path, _backing);
            }
            else
            {
                IsDirty = true;
            }
        }

        /// <summary>
        /// Clears the underlying collection
        /// </summary>
        public void Clear()
        {
            ((ICollection<string>)_backing).Clear();

            if (_autoFlush)
            {
                System.IO.File.Delete(_path);
            }
            else
            {
                IsDirty = true;
            }
        }

        /// <summary>
        /// Returns true if the item was found in the underlying collection
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>True if the item was found in the underlying collection</returns>
        public bool Contains(string item) => ((ICollection<string>)_backing).Contains(item);

        /// <summary>
        /// Copies the collection to the provided array
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The source index to start at</param>
        public void CopyTo(string[] array, int arrayIndex) => ((ICollection<string>)_backing).CopyTo(array, arrayIndex);

        /// <summary>
        /// Flushes and disposes of this object
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Writes the backing collection to disk
        /// </summary>
        public void Flush()
        {
            System.IO.File.WriteAllLines(_path, _backing);
            IsDirty = false;
        }

        /// <summary>
        /// Gets the enumerator for the underlying collection
        /// </summary>
        /// <returns>The enumerator for the underlying collection</returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_backing).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_backing).GetEnumerator();

        /// <summary>
        /// Finds the index of the provided item
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The index of the item if found, otherwise -1</returns>
        public int IndexOf(string item) => ((IList<string>)_backing).IndexOf(item);

        /// <summary>
        /// Inserts the item at the provided index
        /// </summary>
        /// <param name="index">The index to insert the item at</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, string item)
        {
            ((IList<string>)_backing).Insert(index, item);

            if (_autoFlush)
            {
                System.IO.File.WriteAllLines(_path, _backing);
            }
            else
            {
                IsDirty = true;
            }
        }

        /// <summary>
        /// Removes the item from the underlying collection
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was found and removed</returns>
        public bool Remove(string item)
        {
            bool v = ((ICollection<string>)_backing).Remove(item);

            if (_autoFlush)
            {
                System.IO.File.WriteAllLines(_path, _backing);
            }
            else
            {
                IsDirty = true;
            }

            return v;
        }

        /// <summary>
        /// Removes the item at the specified index
        /// </summary>
        /// <param name="index">The index to remove the item at</param>
        public void RemoveAt(int index)
        {
            ((IList<string>)_backing).RemoveAt(index);

            if (_autoFlush)
            {
                System.IO.File.WriteAllLines(_path, _backing);
            }
            else
            {
                IsDirty = true;
            }
        }

        /// <summary>
        /// Sets the item at the provided index
        /// </summary>
        /// <param name="index">The index to set the item at</param>
        /// <param name="value">The item to set at the provided index</param>
        public void SetElement(int index, string value)
        {
            if (_backing.Count <= index)
            {
                while (_backing.Count < index)
                {
                    Add(string.Empty);

                    if (_autoFlush)
                    {
                        System.IO.File.AppendAllText(_path, System.Environment.NewLine);
                    }
                }

                Add(value);

                if (_autoFlush)
                {
                    System.IO.File.AppendAllText(_path, value);
                }
                else
                {
                    IsDirty = true;
                }
            }
            else
            {
                string eVal = _backing[index];

                if (eVal == value)
                {
                    return;
                }

                _backing[index] = value;

                if (_autoFlush)
                {
                    System.IO.File.WriteAllLines(_path, _backing);
                }
                else
                {
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Flushes the backing collection to disk and disposes
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (!_autoFlush && IsDirty)
                    {
                        Flush();
                    }
                }

                _disposedValue = true;
            }
        }
    }
}