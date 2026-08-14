using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.Services
{
    public class PrinterService
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class DOCINFO
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName = "";

            [MarshalAs(UnmanagedType.LPWStr)]
            public string? pOutputFile;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataType = "RAW";
        }

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool OpenPrinter(
            string pPrinterName,
            out IntPtr phPrinter,
            IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(
            IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool StartDocPrinter(
            IntPtr hPrinter,
            int level,
            [In] DOCINFO pDocInfo);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndDocPrinter(
            IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool StartPagePrinter(
            IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndPagePrinter(
            IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool WritePrinter(
            IntPtr hPrinter,
            IntPtr pBytes,
            int dwCount,
            out int dwWritten);

        public void PrintRaw(string printerName, byte[] data)
        {
            IntPtr printerHandle = IntPtr.Zero;
            IntPtr unmanagedData = IntPtr.Zero;

            try
            {
                // Mở máy in
                if (!OpenPrinter(printerName, out printerHandle, IntPtr.Zero))
                {
                    int error = Marshal.GetLastWin32Error();

                    throw new Exception(
                        $"Không thể mở máy in '{printerName}'. " +
                        $"Windows Error: {error}");
                }

                var docInfo = new DOCINFO
                {
                    pDocName = "LabelPrinter Test",
                    pDataType = "RAW"
                };

                // Bắt đầu Print Job
                if (!StartDocPrinter(printerHandle, 1, docInfo))
                {
                    int error = Marshal.GetLastWin32Error();

                    throw new Exception(
                        $"Không thể bắt đầu Print Job. " +
                        $"Windows Error: {error}");
                }

                try
                {
                    if (!StartPagePrinter(printerHandle))
                    {
                        int error = Marshal.GetLastWin32Error();

                        throw new Exception(
                            $"Không thể bắt đầu trang in. " +
                            $"Windows Error: {error}");
                    }

                    try
                    {
                        unmanagedData = Marshal.AllocCoTaskMem(data.Length);

                        Marshal.Copy(
                            data,
                            0,
                            unmanagedData,
                            data.Length);

                        if (!WritePrinter(
                            printerHandle,
                            unmanagedData,
                            data.Length,
                            out int bytesWritten))
                        {
                            int error = Marshal.GetLastWin32Error();

                            throw new Exception(
                                $"Không thể gửi dữ liệu đến máy in. " +
                                $"Windows Error: {error}");
                        }

                        if (bytesWritten != data.Length)
                        {
                            throw new Exception(
                                $"Dữ liệu gửi không đầy đủ. " +
                                $"Đã gửi {bytesWritten}/{data.Length} bytes.");
                        }
                    }
                    finally
                    {
                        EndPagePrinter(printerHandle);
                    }
                }
                finally
                {
                    EndDocPrinter(printerHandle);
                }
            }
            finally
            {
                if (unmanagedData != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(unmanagedData);
                }

                if (printerHandle != IntPtr.Zero)
                {
                    ClosePrinter(printerHandle);
                }
            }
        }
    }
}
