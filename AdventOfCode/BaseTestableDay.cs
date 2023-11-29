namespace AdventOfCode
{
    public abstract class BaseTestableDay: BaseDay
    {
        internal bool IsTest { get; set; }

        protected override string InputFileExtension => IsTest ? "test.txt": base.InputFileExtension;
    }
}
