namespace Dedox
{
    internal class DedoxMetrics : IDedoxMetrics
    {
        private int _codeElements;
        private int _codeElementsWithDocumentation;
        private int _codeElementsWithGeneratedDocumentation;

        public int CodeElements
        {
            get
            {
                return _codeElements;
            }
        }

        public int CodeElementsWithDocumentation
        {
            get
            {
                return _codeElementsWithDocumentation;
            }
        }

        public int CodeElementsWithGeneratedDocumentation
        {
            get
            {
                return _codeElementsWithGeneratedDocumentation;
            }
        }

        public void IncrementCodeElements()
        {
            ++_codeElements;
        }

        public void IncrementCodeElementsWithDocumentation()
        {
            ++_codeElementsWithDocumentation;
        }

        public void IncrementCodeElementsWithGeneratedDocumentation()
        {
            ++_codeElementsWithGeneratedDocumentation;
        }
    }
}