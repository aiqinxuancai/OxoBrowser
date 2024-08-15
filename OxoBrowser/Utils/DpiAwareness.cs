using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;

namespace OxoBrowser.Utils
{
    /// <summary>
    /// DPI API
    /// </summary>
    public class DpiAwareness
    {
        public static PROCESS_DPI_AWARENESS processAwareness = PROCESS_DPI_AWARENESS.Process_System_DPI_Aware;


        [DllImport("Gdi32")]
        private static extern IntPtr CreateDC(String driver, String device, String output, IntPtr initData);

        [DllImport("Gdi32")]
        private static extern void DeleteDC(IntPtr dc);

        [DllImport("Gdi32")]
        private static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 index);

        private const Int32 LOGPIXELSX = 88;
        private const Int32 LOGPIXELSY = 90;

        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern void GetProcessDpiAwareness(IntPtr hprocess, out PROCESS_DPI_AWARENESS awareness);

        public enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }


        public static int GetDpi()
        {
            IntPtr dc = IntPtr.Zero;
            try
            {
                dc = CreateDC("DISPLAY", null, null, IntPtr.Zero);
                Int32 dpi = GetDeviceCaps(dc, LOGPIXELSX);

                if (dpi == 0)
                {
                    dpi = 96;
                }
                else
                {
                }
                return dpi;
            }
            finally
            {
                if (dc != IntPtr.Zero)
                {
                    DeleteDC(dc);
                }
            }
        }




    }
}
