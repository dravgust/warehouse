namespace Vayosoft.Core.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T o);
    }
}