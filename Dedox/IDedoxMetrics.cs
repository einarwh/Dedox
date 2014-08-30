namespace Dedox
{
    public interface IDedoxMetrics
    {
        int CodeElements { get; }

        int CodeElementsWithDocumentation { get; }

        int CodeElementsWithGeneratedDocumentation { get; }

        void IncrementCodeElements();

        void IncrementCodeElementsWithDocumentation();

        void IncrementCodeElementsWithGeneratedDocumentation();
    }
}