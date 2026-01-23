namespace Shared.Contracts;

public interface IElementProvider<T> where T : class
{
    T GetElements();
}