namespace Helpers
{
    public static class HelperTime
    {
        public static string GetTotalTime(uint seconds)
        {
            var timeSpan = System.TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
        
        public static string ToTotalTime(this uint seconds)
        {
            var timeSpan = System.TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        } 
    }
}