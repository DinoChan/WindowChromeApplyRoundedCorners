﻿using System.Runtime.InteropServices;

namespace WindowChromeApplyRoundedCorners
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms724385(v=vs.85).aspx
    /// Retrieves the specified system metric or system configuration setting.
    /// Note that all dimensions retrieved by GetSystemMetrics are in pixels.
    /// </summary>
    public enum SM
    {
        /// <summary>
        /// The amount of border padding for captioned windows, in pixels.
        /// Returns the amount of extra border padding around captioned windows
        /// Windows XP/2000:  This value is not supported.
        /// </summary>
        CXPADDEDBORDER = 92,
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SM nIndex);
    }
}