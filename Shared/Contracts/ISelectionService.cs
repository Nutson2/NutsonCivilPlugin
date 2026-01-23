using CSharpFunctionalExtensions;

namespace Shared.Contracts;
public interface ISelectionService<Tm> where Tm : class
{
    Maybe<Tm> RequestSelection();
}