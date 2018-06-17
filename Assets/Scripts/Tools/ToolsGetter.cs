namespace Tools
{
    public static class ToolsGetter
    {
        public static Assembly Assembly
        {
            get { return Assembly.Instance; }
        }

        public static ImageFilters ImageFilters
        {
            get { return ImageFilters.Instance; }
        }
        
#if UNITY_ANDROID
        public static Android Android
        {
            get { return Android.Instance; }
        }
#endif

#if UNITY_EDITOR
        public static IO IO
        {
            get { return IO.Instance; }
        }
#endif
    }
}
