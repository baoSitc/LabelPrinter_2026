using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.Services
{
    public class NetworkPrinterService
    {
        public async Task PrintAsync(
            string printerIp,
            byte[] data,
            int port = 9100)
        {
            if (string.IsNullOrWhiteSpace(printerIp))
            {
                throw new ArgumentException(
                    "IP máy in không được để trống.");
            }

            if (data == null || data.Length == 0)
            {
                throw new ArgumentException(
                    "Không có dữ liệu để in.");
            }

            try
            {
                using var client = new TcpClient();

                await client.ConnectAsync(
                    printerIp,
                    port);

                using NetworkStream stream =
                    client.GetStream();

                await stream.WriteAsync(
                    data,
                    0,
                    data.Length);

                await stream.FlushAsync();
            }
            catch (SocketException ex)
            {
                throw new Exception(
                    $"Không thể kết nối máy in {printerIp}:9100. " +
                    $"Chi tiết: {ex.Message}",
                    ex);
            }
        }
    }
}
