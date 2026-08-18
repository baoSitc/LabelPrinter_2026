using LabelPrinter.Models;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LabelPrinter.Services
{
    public class LabelService50x50: ILabelService
    {
        // ============================================================
        // KHỔ TEM 50 x 50 mm
        // ============================================================

        private const int LabelWidth = 400;
        private const int LabelHeight = 400;

        // 203 DPI:
        // 50 / 25.4 * 203 ≈ 400 dots


        // ============================================================
        // TẠO BITMAP TEM
        // ============================================================

        public BitmapSource CreateLabelBitmap(LabelData label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            var visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                // ====================================================
                // NỀN TRẮNG
                // ====================================================

                dc.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(
                        0,
                        0,
                        LabelWidth,
                        LabelHeight));


                // ====================================================
                // FONT
                // ====================================================

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


                // ====================================================
                // VỊ TRÍ
                // ====================================================

                double margin = 18;

                double y = 12;


                // ====================================================
                // TÊN HÀNG
                // ====================================================

                DrawTextCentered(
                    dc,
                    label.TenHang,
                    fontTitle,
                    18,
                    Brushes.Black,
                    new Rect(
                        margin,
                        y,
                        LabelWidth - margin * 2,
                        45));

                y += 55;


                // ====================================================
                // LINE
                // ====================================================

                DrawLine(
                    dc,
                    margin,
                    y,
                    LabelWidth - margin,
                    y);

                y += 10;


                // ====================================================
                // THÀNH PHẦN
                // ====================================================

                DrawLabel(
                    dc,
                    "Thành phần:",
                    label.ThanhPhan,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 3;


                // ====================================================
                // XUẤT XỨ
                // ====================================================

                DrawLabel(
                    dc,
                    "Xuất xứ:",
                    label.XuatXu,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 3;


                // ====================================================
                // BẢO QUẢN
                // ====================================================

                DrawLabel(
                    dc,
                    "Bảo quản:",
                    label.BaoQuan,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 3;


                // ====================================================
                // NGÀY SẢN XUẤT
                // ====================================================

                DrawLabel(
                    dc,
                    "NSX:",
                    label.NgaySanXuat,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 3;


                // ====================================================
                // HẠN SỬ DỤNG
                // ====================================================

                DrawLabel(
                    dc,
                    "HSD:",
                    label.HanSuDung,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 8;


                // ====================================================
                // LINE
                // ====================================================

                DrawLine(
                    dc,
                    margin,
                    y,
                    LabelWidth - margin,
                    y);

                y += 10;


                // ====================================================
                // NHÀ PHÂN PHỐI
                // ====================================================

                DrawLabel(
                    dc,
                    "NPP:",
                    label.NhaPhanPhoi,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 3;


                // ====================================================
                // ĐỊA CHỈ NPP
                // ====================================================

                DrawMultilineLabel(
                    dc,
                    "Đ/c:",
                    label.DiaChiNhaPhanPhoi,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 5;


                // ====================================================
                // NƠI SẢN XUẤT
                // ====================================================

                DrawLabel(
                    dc,
                    "NSX:",
                    label.NoiSanXuat,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);

                y += 3;


                // ====================================================
                // ĐỊA CHỈ SẢN XUẤT
                // ====================================================

                DrawMultilineLabel(
                    dc,
                    "Đ/c:",
                    label.DiaChiSanXuat,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y);


                // ====================================================
                // LINE CUỐI
                // ====================================================

                DrawLine(
                    dc,
                    margin,
                    365,
                    LabelWidth - margin,
                    365);


                // ====================================================
                // SỐ TEM
                // ====================================================

                string soBanIn =
                    $"Tem: {label.SoBanIn}";

                DrawTextRight(
                    dc,
                    soBanIn,
                    fontNormal,
                    13,
                    Brushes.Black,
                    new Rect(
                        margin,
                        370,
                        LabelWidth - margin * 2,
                        20));
            }


            // ========================================================
            // RENDER BITMAP
            // ========================================================

            var bitmap = new RenderTargetBitmap(
                LabelWidth,
                LabelHeight,
                203,
                203,
                PixelFormats.Pbgra32);

            bitmap.Render(visual);

            bitmap.Freeze();

            return bitmap;
        }


        // ============================================================
        // TẠO TSPL BITMAP
        // ============================================================

        public byte[] CreateTsplBitmapCommand(
            LabelData label,
            int quantity = 1)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            if (quantity <= 0)
                quantity = 1;


            // ========================================================
            // TẠO BITMAP
            // ========================================================

            BitmapSource bitmap =
                CreateLabelBitmap(label);


            // ========================================================
            // CHUYỂN SANG MONOCHROME
            // ========================================================

            byte[] bitmapData =
                ConvertToMonochrome(bitmap);


            int bytesPerRow =
                (LabelWidth + 7) / 8;


            // ========================================================
            // STREAM
            // ========================================================

            using var stream =
                new MemoryStream();


            // ========================================================
            // TSPL HEADER
            // ========================================================

            string header =
                $"SIZE 50 mm,50 mm\r\n" +
                $"GAP 2 mm,0\r\n" +
                $"DIRECTION 1\r\n" +
                $"CLS\r\n" +
                $"BITMAP 0,0,{bytesPerRow},{LabelHeight},0,";


            byte[] headerBytes =
                Encoding.ASCII.GetBytes(header);


            stream.Write(
                headerBytes,
                0,
                headerBytes.Length);


            // ========================================================
            // BITMAP DATA
            // ========================================================

            stream.Write(
                bitmapData,
                0,
                bitmapData.Length);


            // ========================================================
            // PRINT
            // ========================================================

            byte[] footerBytes =
                Encoding.ASCII.GetBytes(
                    $"\r\nPRINT 1,{quantity}\r\n");


            stream.Write(
                footerBytes,
                0,
                footerBytes.Length);


            return stream.ToArray();
        }


        // ============================================================
        // BITMAP → MONOCHROME
        // ============================================================

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
                new byte[
                    width *
                    height *
                    3];


            bitmap.CopyPixels(
                pixels,
                width * 3,
                0);


            byte[] result =
                new byte[
                    bytesPerRow *
                    height];


            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
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


                    // =================================================
                    // GIỮ NGUYÊN LOGIC CỦA CODE A7
                    // =================================================

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


        // ============================================================
        // DRAW LABEL
        // ============================================================

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
                LabelWidth - x - 25;


            var formatted =
                new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    valueFont,
                    16,
                    Brushes.Black,
                    1.0)
                {
                    MaxTextWidth = maxWidth,
                    Trimming = TextTrimming.None
                };


            dc.DrawText(
                formatted,
                new Point(x, y));


            // ========================================================
            // QUAN TRỌNG
            // LẤY CHIỀU CAO THẬT CỦA TEXT
            // ========================================================

            y += formatted.Height + 5;
        }


        // ============================================================
        // DRAW MULTILINE
        // ============================================================

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
                LabelWidth - x - 25;


            var formatted =
                new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    valueFont,
                    15,
                    Brushes.Black,
                    1.0)
                {
                    MaxTextWidth = maxWidth,
                    Trimming = TextTrimming.None
                };


            dc.DrawText(
                formatted,
                new Point(x, y));


            // ========================================================
            // TỰ TĂNG THEO CHIỀU CAO THỰC TẾ
            // ========================================================

            y += formatted.Height + 5;
        }


        // ============================================================
        // TEXT CENTER
        // ============================================================

        private void DrawTextCentered(
            DrawingContext dc,
            string? text,
            Typeface typeface,
            double fontSize,
            Brush brush,
            Rect rect)
        {
            text ??= "";

            if (string.IsNullOrWhiteSpace(text))
                return;


            var formatted =
                new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    brush,
                    1.0)
                {
                    MaxTextWidth = rect.Width,
                    MaxTextHeight = rect.Height,
                    TextAlignment = TextAlignment.Center,
                    Trimming = TextTrimming.None
                };


            double y =
                rect.Y +
                Math.Max(
                    0,
                    (rect.Height -
                     formatted.Height) / 2);


            dc.DrawText(
                formatted,
                new Point(
                    rect.X,
                    y));
        }


        // ============================================================
        // TEXT RIGHT
        // ============================================================

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
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    brush,
                    1.0)
                {
                    MaxTextWidth = rect.Width,
                    TextAlignment = TextAlignment.Right,
                    Trimming = TextTrimming.None
                };


            dc.DrawText(
                formatted,
                new Point(
                    rect.X,
                    rect.Y));
        }


        // ============================================================
        // LINE
        // ============================================================

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
                new Point(
                    x1,
                    y1),
                new Point(
                    x2,
                    y2));
        }
    }
}