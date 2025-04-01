using System.Runtime.InteropServices;

namespace ShaderGraphToy.Utilities.Common
{
    internal static  class RenderingDeviceManager
    {
        private static nint _nvapiIdx = nint.Zero;

        public static bool IsNvapiActive { get; set; } = false;


        public static void EnableNvapi()
        {
            try
            {
                if (Environment.Is64BitProcess)
                    _nvapiIdx = NativeLibrary.Load("nvapi64.dll");
                else
                    _nvapiIdx = NativeLibrary.Load("nvapi32.dll");

                IsNvapiActive = true;
            }
            catch (Exception)
            {
                IsNvapiActive = false;
            }
        }

        public static void DisableNvapi()
        {
            if (_nvapiIdx != nint.Zero)
                NativeLibrary.Free(_nvapiIdx);
        }
    }
}
