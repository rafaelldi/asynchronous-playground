using System.Runtime.InteropServices;

namespace synchronization_context_app;

public static class Util
{
    [DllImport("kernel32.dll")]
    public static extern int GetCurrentThreadId();
}