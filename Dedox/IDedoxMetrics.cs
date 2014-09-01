namespace Dedox
{
    public interface IDedoxMetrics
    {
        int AllCodeElements { get; }

        int CodeElements { get; }

        int CodeElementsWithDocumentation { get; }

        int CodeElementsWithGeneratedDocumentation { get; }

        void IncrementCodeElements();

        void IncrementCodeElementsWithDocumentation();

        void IncrementCodeElementsWithGeneratedDocumentation();

        void IncrementAllCodeElements();
    }
}