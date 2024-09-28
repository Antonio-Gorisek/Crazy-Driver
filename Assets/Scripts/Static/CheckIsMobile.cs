public static class CheckIsMobile
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool IsMobile();
#endif

    public static bool Check()
    {
        var isMobile = false;

#if UNITY_ANDROID || UNITY_IOS
        isMobile = true;
#elif !UNITY_EDITOR && UNITY_WEBGL
        isMobile = IsMobile();
#endif

        return isMobile;
    }
}
