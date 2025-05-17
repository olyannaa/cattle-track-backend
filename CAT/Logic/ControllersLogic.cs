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

        public static (int skip, int take) ComputePagination(bool isMobile, int page)
        {
            var take = isMobile ? 5 : 10;
            var skip = (page - 1) * take;
            return (skip, take);
        }
    }

}