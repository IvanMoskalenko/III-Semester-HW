namespace MyNUnit
{
    public class TestInformation
    {
        public string Name { get; }
        public string Result { get; }
        public long Time { get; }
        public string IgnoreReason { get; }

        public TestInformation(string name, string result, string ignoreReason, long time)
        {
            Name = name;
            Result = result;
            IgnoreReason = ignoreReason;
            Time = time;
        }
    }
}