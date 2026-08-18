using LabelPrinter.Models;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LabelPrinter.Services
{
    public class LabelServiceA7 : ILabelService
    {
        private const int LabelWidth = 600;
        private const int LabelHeight = 800;

        public BitmapSource CreateLabelBitmap(LabelData label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            var visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(
                        0,
                        0,
                        LabelWidth,
                        LabelHeight));

                var fontTitle = new Typeface(
                    new FontFamily("Arial"),
                    FontStyles.Normal,
                    FontWeights.Bold,
                    FontStretches.Normal);

                var fontBold = new Typeface(
                    new FontFamily("Arial"),
                    FontStyles.Normal,
                    FontWeights.Bold,
                    FontStretches.Normal);

                var fontNormal = new Typeface(
                    new FontFamily("Arial"),
                    FontStyles.Normal,
                    FontWeights.Normal,
                    FontStretches.Normal);

                double margin = 30;
                double y = 25;

                // ==============================
                // TÊN HÀNG
                // ==============================

                DrawTextCentered(
                    dc,
                    label.TenHang,
                    fontTitle,
                    38,
                    Brushes.Black,
                    new Rect(
                        margin,
                        y,
                        LabelWidth - margin * 2,
                        70));

                y += 85;

                // ==============================
                // LINE
                // ==============================

                DrawLine(
                    dc,
                    margin,
                    y,
                    LabelWidth - margin,
                    y);

                y += 20;

                // ==============================
                // THÀNH PHẦN
                // ==============================

                DrawLabel(
                    dc,
                    "Thành phần:",
                    label.ThanhPhan,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 10;

                // ==============================
                // XUẤT XỨ
                // ==============================

                DrawLabel(
                    dc,
                    "Xuất xứ:",
                    label.XuatXu,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 10;

                // ==============================
                // BẢO QUẢN
                // ==============================

                DrawLabel(
                    dc,
                    "Bảo quản:",
                    label.BaoQuan,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 10;

                // ==============================
                // NSX
                // ==============================

                DrawLabel(
                    dc,
                    "Ngày sản xuất:",
                    label.NgaySanXuat,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 10;

                // ==============================
                // HSD
                // ==============================

                DrawLabel(
                    dc,
                    "Hạn sử dụng:",
                    label.HanSuDung,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 15;

                // ==============================
                // LINE
                // ==============================

                DrawLine(
                    dc,
                    margin,
                    y,
                    LabelWidth - margin,
                    y);

                y += 20;

                // ==============================
                // NHÀ PHÂN PHỐI
                // ==============================

                DrawLabel(
                    dc,
                    "Nhà phân phối:",
                    label.NhaPhanPhoi,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 5;

                // ==============================
                // ĐỊA CHỈ NPP
                // ==============================

                DrawMultilineLabel(
                    dc,
                    "Địa chỉ:",
                    label.DiaChiNhaPhanPhoi,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 10;

                // ==============================
                // NƠI SẢN XUẤT
                // ==============================

                DrawLabel(
                    dc,
                    "Nơi sản xuất:",
                    label.NoiSanXuat,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                y += 5;

                // ==============================
                // ĐỊA CHỈ SX
                // ==============================

                DrawMultilineLabel(
                    dc,
                    "Địa chỉ:",
                    label.DiaChiSanXuat,
                    fontBold,
                    fontNormal,
                    25,
                    ref y);

                // ==============================
                // LINE BARCODE
                // ==============================

                DrawLine(
                    dc,
                    margin,
                    650,
                    LabelWidth - margin,
                    650);

                // ==============================
                // SỐ TEM
                // ==============================

                DrawTextRight(
                    dc,
                    $"Số tem: {label.SoBanIn}",
                    fontNormal,
                    18,
                    Brushes.Black,
                    new Rect(
                        margin,
                        750,
                        LabelWidth - margin * 2,
                        30));
            }

            var bitmap = new RenderTargetBitmap(
                LabelWidth,
                LabelHeight,
                96,
                96,
                PixelFormats.Pbgra32);

            bitmap.Render(visual);

            bitmap.Freeze();

            return bitmap;
        }

        // =========================================================
        // TSPL
        // =========================================================

        public byte[] CreateTsplBitmapCommand(
            LabelData label,
            int quantity = 1)
        {
            BitmapSource bitmap =
                CreateLabelBitmap(label);

            byte[] bitmapData =
                ConvertToMonochrome(bitmap);

            int bytesPerRow =
                (LabelWidth + 7) / 8;

            using var stream =
                new MemoryStream();

            string header =
                "SIZE 75 mm,100 mm\r\n" +
                "GAP 2 mm,0\r\n" +
                "DIRECTION 1\r\n" +
                "CLS\r\n" +
                $"BITMAP 0,0,{bytesPerRow},{LabelHeight},0,";

            byte[] headerBytes =
                Encoding.ASCII.GetBytes(header);

            stream.Write(
                headerBytes,
                0,
                headerBytes.Length);

            stream.Write(
                bitmapData,
                0,
                bitmapData.Length);

            byte[] footerBytes =
                Encoding.ASCII.GetBytes(
                    $"\r\nPRINT 1,{quantity}\r\n");

            stream.Write(
                footerBytes,
                0,
                footerBytes.Length);

            return stream.ToArray();
        }

        // =========================================================
        // MONOCHROME
        // =========================================================

        private byte[] ConvertToMonochrome(
            BitmapSource source)
        {
            BitmapSource bitmap =
                new FormatConvertedBitmap(
                    source,
                    PixelFormats.Bgr24,
                    null,
                    0);

            int width =
                bitmap.PixelWidth;

            int height =
                bitmap.PixelHeight;

            int bytesPerRow =
                (width + 7) / 8;

            byte[] pixels =
                new byte[width * height * 3];

            bitmap.CopyPixels(
                pixels,
                width * 3,
                0);

            byte[] result =
                new byte[bytesPerRow * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex =
                        (y * width + x) * 3;

                    byte blue =
                        pixels[pixelIndex];

                    byte green =
                        pixels[pixelIndex + 1];

                    byte red =
                        pixels[pixelIndex + 2];

                    int gray =
                        (int)(
                            0.299 * red +
                            0.587 * green +
                            0.114 * blue);

                    if (gray >= 128)
                    {
                        int byteIndex =
                            y * bytesPerRow +
                            (x / 8);

                        int bitIndex =
                            7 - (x % 8);

                        result[byteIndex] |=
                            (byte)(1 << bitIndex);
                    }
                }
            }

            return result;
        }

        // =========================================================
        // DRAW LABEL
        // =========================================================

        private void DrawLabel(
            DrawingContext dc,
            string title,
            string? value,
            Typeface titleFont,
            Typeface valueFont,
            double x,
            ref double y)
        {
            value ??= "";

            string text =
                $"{title} {value}";

            double maxWidth =
                LabelWidth - x - 30;

            var formatted =
                new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    valueFont,
                    22,
                    Brushes.Black,
                    1.0)
                {
                    MaxTextWidth = maxWidth,
                    Trimming = TextTrimming.None
                };

            dc.DrawText(
                formatted,
                new Point(x, y));

            y += formatted.Height + 8;
        }

        // =========================================================
        // MULTILINE
        // =========================================================

        private void DrawMultilineLabel(
            DrawingContext dc,
            string title,
            string? value,
            Typeface titleFont,
            Typeface valueFont,
            double x,
            ref double y)
        {
            value ??= "";

            string text =
                $"{title} {value}";

            double maxWidth =
                LabelWidth - x - 30;

            var formatted =
                new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    valueFont,
                    22,
                    Brushes.Black,
                    1.0)
                {
                    MaxTextWidth = maxWidth,
                    Trimming = TextTrimming.None
                };

            dc.DrawText(
                formatted,
                new Point(x, y));

            y += formatted.Height + 8;
        }

        // =========================================================
        // CENTER
        // =========================================================

        private void DrawTextCentered(
            DrawingContext dc,
            string? text,
            Typeface typeface,
            double fontSize,
            Brush brush,
            Rect rect)
        {
            text ??= "";

            var formatted =
                new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    brush,
                    1.0)
                {
                    MaxTextWidth = rect.Width,
                    TextAlignment = TextAlignment.Center
                };

            double y =
                rect.Y +
                (rect.Height - formatted.Height) / 2;

            dc.DrawText(
                formatted,
                new Point(
                    rect.X,
                    y));
        }

        // =========================================================
        // RIGHT
        // =========================================================

        private void DrawTextRight(
            DrawingContext dc,
            string? text,
            Typeface typeface,
            double fontSize,
            Brush brush,
            Rect rect)
        {
            text ??= "";

            var formatted =
                new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    brush,
                    1.0)
                {
                    MaxTextWidth = rect.Width,
                    TextAlignment = TextAlignment.Right
                };

            dc.DrawText(
                formatted,
                new Point(
                    rect.X,
                    rect.Y));
        }

        // =========================================================
        // LINE
        // =========================================================

        private void DrawLine(
            DrawingContext dc,
            double x1,
            double y1,
            double x2,
            double y2)
        {
            var pen =
                new Pen(
                    Brushes.Black,
                    2);

            dc.DrawLine(
                pen,
                new Point(x1, y1),
                new Point(x2, y2));
        }
    }
}