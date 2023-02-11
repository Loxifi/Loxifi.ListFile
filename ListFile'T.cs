namespace Loxifi
{
    /// <summary>
    /// A class that stores its backing data as a file enabling persistence.
    /// Uses the provided serialization settings to convert between string
    /// and stronly typed object/// </summary>
    /// <typeparam name="T">
    /// A type that the objects will be serialized to and from during persistance
    /// </typeparam>
    public class ListFile<T> : ListFile, IList<T>
    {
        T IList<T>.this[int index]
        {
            get => _serializationSettings.Deserialize(base[index]);
            set => base[index] = _serializationSettings.Serialize(value);
        }

        private readonly SerializationSettings<T> _serializationSettings;

        /// <summary>
        /// Constructs a new instance of this class with the provided options
        /// </summary>
        /// <param name="path">The location on disk to persist the data to</param>
        /// <param name="serializationSettings">Serialization options to use during persistence</param>
        /// <param name="autoFlush">
        /// If true, changes will be flushed to disk with every operation
        /// Safer, but slower. Defaults to true
        /// </param>
        public ListFile(string path, SerializationSettings<T> serializationSettings, bool autoFlush = true) : base(path, autoFlush)
        {
            _serializationSettings = serializationSettings;
        }

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item) => Add(_serializationSettings.Serialize(item));

        /// <summary>
        /// Checks if an item is found in the collection.
        /// Serializes the object first, so it functions by value
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>True if the serialized item was found in the collection</returns>
        public bool Contains(T item) => Contains(_serializationSettings.Serialize(item));

        /// <summary>
        /// Copies the items to the provieed array
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The source index to start at</param>
        public void CopyTo(T[] array, int arrayIndex) => Array.Copy((this as IList<T>).ToArray(), 0, array, arrayIndex, Count);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            IEnumerator<string> baseValues = base.GetEnumerator();

            while (baseValues.MoveNext())
            {
                yield return _serializationSettings.Deserialize(baseValues.Current);
            }
        }

        /// <summary>
        /// Finds the index of the given value.
        /// Items are serialized first, so this functions by value
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>The index of the item if found, or -1</returns>
        public int IndexOf(T item) => IndexOf(_serializationSettings.Serialize(item));

        /// <summary>
        /// Inserts an item into the backing list at the specified index
        /// </summary>
        /// <param name="index">The index to insert the item at</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, T item) => Insert(index, _serializationSettings.Serialize(item));

        /// <summary>
        /// Removes an item from the backing list.
        /// The item is serialized first, so this functions by value
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was found and removed</returns>
        public bool Remove(T item) => Remove(_serializationSettings.Serialize(item));
    }
}