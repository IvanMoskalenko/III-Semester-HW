namespace Lazy
{
    public interface ILazy<out T>
    {
        T Get();
    }
}