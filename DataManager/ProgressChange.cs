namespace DataManager
{
    public class ProgressChange
    {
        public int Total { get; }
        public int Current { get; }
        public string Message { get; }

        public ProgressChange(int total, int current, string message)
        {
            Total = total;
            Current = current;
            Message = message;
        }
    }
}
