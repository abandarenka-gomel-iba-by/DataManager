using System.Diagnostics;

namespace DataManager
{
    public class ProgressChange
    {
        public int Percent { get; }
        public string Message { get; }

        public ProgressChange(long total, long processed, string message)
        {
            Percent = (int)(((double)processed / total) * 100);
            Message = message;
        }
    }
}
