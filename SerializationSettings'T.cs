namespace Loxifi
{
    /// <summary>
    /// Dictates how objects are serialized and deserilized to disk.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializationSettings<T>
    {
        /// <summary>
        /// A function to call when deserializing objects from disk.
        /// Defaults to <see cref="Convert.ChangeType(object, Type)"/>
        /// </summary>
        public Func<string, T> Deserialize { get; set; } = DefaultDeserialize;

        /// <summary>
        /// A function to call when serializing objects to disk.
        /// Defaults to <see cref="object.ToString"/>
        /// </summary>
        public Func<T, string> Serialize { get; set; } = DefaultSerialize;

        private static T DefaultDeserialize(string s) => (T)Convert.ChangeType(s, typeof(T));

        private static string DefaultSerialize(T t) => $"{t}";
    }
}