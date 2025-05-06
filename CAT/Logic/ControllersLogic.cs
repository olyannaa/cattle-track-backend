namespace CAT.Logic
{
    public static class ControllersLogic
    {
        public static bool IsMobileDevice(string? userAgent)
        {
            return userAgent != null 
                && (userAgent.Contains("iPhone")
                || userAgent.Contains("Android") 
                || userAgent.Contains("Windows Phone"));
        }
    }

}