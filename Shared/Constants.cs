namespace Shared
{
    public static class Constants
    {
        public static string BaseDirectory => 
            Environment.GetEnvironmentVariable("DIRECTORY") ?? AppDomain.CurrentDomain.BaseDirectory;
    }
}
