using System.Collections.ObjectModel;

namespace Vayosoft.Core.Utilities
{

    /// <summary>
    /// Contains a method used to provide an empty, read-only collection.
    /// </summary>
    public static class ReadOnlyCollection
    {
        /// <summary>
        /// Returns an empty, read-only collection that has the specified type argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type to assign to the type parameter of the returned generic read-only collection.
        /// </typeparam>
        /// <returns>
        /// An empty, read-only collection whose type argument is T.
        /// </returns>
        public static ReadOnlyCollection<T> Empty<T>()
        {
            return CachedValueProvider<T>.Value;
        }

        /// <summary/>
        private static class CachedValueProvider<T>
        {
            /// <summary/>
            //public static readonly IReadOnlyCollection<T> Value = Array.Empty<T>();
            public static readonly ReadOnlyCollection<T> Value = new List<T>(0).AsReadOnly();
        }
    }
}
