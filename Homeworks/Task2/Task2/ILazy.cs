namespace Task2
{
    /// <summary>
    /// Represents the lazy calulation.
    /// </summary>
    /// <typeparam name="T">The type of the returned object.</typeparam>
    public interface ILazy<T>
    {
        /// <summary>
        /// Calculates and returns a result, repeated calls return the same object.
        /// </summary>
        /// <returns>The result of the calculation.</returns>
        T Get();
    }
}
