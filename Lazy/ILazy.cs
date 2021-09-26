namespace Lazy
{
    /// <summary>
    /// Lazy evaluation interface.
    /// </summary>
    /// <typeparam name="T">Type of returning object.</typeparam>
    public interface ILazy<out T>
    {
        T Get();
    }
}