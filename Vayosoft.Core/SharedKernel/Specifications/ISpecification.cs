namespace Vayosoft.Core.SharedKernel.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T o);
    }
}