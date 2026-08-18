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
    public class LabelService
    {
        // ============================================================
        // DPI MẶC ĐỊNH
        // ============================================================

        private const int DefaultDpi = 203;


        // ============================================================
        // CREATE BITMAP - KHỔ A7 MẶC ĐỊNH
        // ============================================================

        public BitmapSource CreateLabelBitmap(LabelData label)
        {
            var labelSize = new LabelSize
            {
                Id = "A7",
                Name = "75 x 100 mm",
                WidthMm = 75,
                HeightMm = 100,
                Dpi = DefaultDpi
            };

            return CreateLabelBitmap(label, labelSize);
        }


        // ============================================================
        // CREATE BITMAP THEO KHỔ TEM
        // ============================================================

        public BitmapSource CreateLabelBitmap(
            LabelData label,
            LabelSize labelSize)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            if (labelSize == null)
                throw new ArgumentNullException(nameof(labelSize));


            // --------------------------------------------------------
            // DPI
            // --------------------------------------------------------

            int dpi = labelSize.Dpi > 0
                ? labelSize.Dpi
                : DefaultDpi;


            // --------------------------------------------------------
            // KÍCH THƯỚC BITMAP
            // --------------------------------------------------------

            int labelWidth =
                MmToDots(
                    labelSize.WidthMm,
                    dpi);

            int labelHeight =
                MmToDots(
                    labelSize.HeightMm,
                    dpi);


            // --------------------------------------------------------
            // VISUAL
            // --------------------------------------------------------

            var visual =
                new DrawingVisual();


            using (DrawingContext dc =
                   visual.RenderOpen())
            {
                // ----------------------------------------------------
                // NỀN TRẮNG
                // ----------------------------------------------------

                dc.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(
                        0,
                        0,
                        labelWidth,
                        labelHeight));


                // ----------------------------------------------------
                // FONT
                // ----------------------------------------------------

                var fontTitle =
                    new Typeface(
                        new FontFamily("Arial"),
                        FontStyles.Normal,
                        FontWeights.Bold,
                        FontStretches.Normal);

                var fontBold =
                    new Typeface(
                        new FontFamily("Arial"),
                        FontStyles.Normal,
                        FontWeights.Bold,
                        FontStretches.Normal);

                var fontNormal =
                    new Typeface(
                        new FontFamily("Arial"),
                        FontStyles.Normal,
                        FontWeights.Normal,
                        FontStretches.Normal);


                // ----------------------------------------------------
                // CHỌN TEMPLATE
                // ----------------------------------------------------

                switch (labelSize.Id)
                {
                    case "A7":

                        RenderA7(
                            dc,
                            label,
                            labelWidth,
                            labelHeight,
                            fontTitle,
                            fontBold,
                            fontNormal);

                        break;


                    case "50x50":

                        Render50x50(
                            dc,
                            label,
                            labelWidth,
                            labelHeight,
                            fontTitle,
                            fontBold,
                            fontNormal);

                        break;


                    case "50x30":

                        Render50x30(
                            dc,
                            label,
                            labelWidth,
                            labelHeight,
                            fontTitle,
                            fontBold,
                            fontNormal);

                        break;


                    case "75x50":

                        Render75x50(
                            dc,
                            label,
                            labelWidth,
                            labelHeight,
                            fontTitle,
                            fontBold,
                            fontNormal);

                        break;


                    case "100x150":

                        Render100x150(
                            dc,
                            label,
                            labelWidth,
                            labelHeight,
                            fontTitle,
                            fontBold,
                            fontNormal);

                        break;


                    default:

                        RenderA7(
                            dc,
                            label,
                            labelWidth,
                            labelHeight,
                            fontTitle,
                            fontBold,
                            fontNormal);

                        break;
                }
            }


            // ========================================================
            // RENDER BITMAP
            // ========================================================

            var bitmap =
                new RenderTargetBitmap(
                    labelWidth,
                    labelHeight,
                    dpi,
                    dpi,
                    PixelFormats.Pbgra32);


            bitmap.Render(visual);

            bitmap.Freeze();

            return bitmap;
        }


        // ============================================================
        // A7 - 75 x 100 MM
        // ============================================================

        private void RenderA7(
            DrawingContext dc,
            LabelData label,
            int width,
            int height,
            Typeface fontTitle,
            Typeface fontBold,
            Typeface fontNormal)
        {
            double margin = 25;

            double contentWidth =
                width - margin * 2;

            double y = 12;


            // ========================================================
            // TÊN HÀNG
            // ========================================================

            double titleHeight =
                DrawTextCentered(
                    dc,
                    label.TenHang,
                    fontTitle,
                    16,
                    Brushes.Black,
                    new Rect(
                        margin,
                        y,
                        contentWidth,
                        60));


            y += Math.Max(
                45,
                titleHeight + 8);


            // ========================================================
            // LINE
            // ========================================================

            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 8;


            // ========================================================
            // THÔNG TIN
            // ========================================================

            DrawLabel(
                dc,
                "Thành phần:",
                label.ThanhPhan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            DrawLabel(
                dc,
                "Xuất xứ:",
                label.XuatXu,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            DrawLabel(
                dc,
                "Bảo quản:",
                label.BaoQuan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            DrawLabel(
                dc,
                "Ngày sản xuất:",
                label.NgaySanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            DrawLabel(
                dc,
                "Hạn sử dụng:",
                label.HanSuDung,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            y += 5;


            // ========================================================
            // LINE
            // ========================================================

            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 8;


            // ========================================================
            // NHÀ PHÂN PHỐI
            // ========================================================

            DrawLabel(
                dc,
                "Nhà phân phối:",
                label.NhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            // ========================================================
            // ĐỊA CHỈ NPP
            // ========================================================

            DrawMultilineLabel(
                dc,
                "Địa chỉ:",
                label.DiaChiNhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            y += 3;


            // ========================================================
            // NƠI SẢN XUẤT
            // ========================================================

            DrawLabel(
                dc,
                "Nơi sản xuất:",
                label.NoiSanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            // ========================================================
            // ĐỊA CHỈ SẢN XUẤT
            // ========================================================

            DrawMultilineLabel(
                dc,
                "Địa chỉ:",
                label.DiaChiSanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            // ========================================================
            // FOOTER
            // ========================================================

            double footerY =
                height - 35;


            DrawLine(
                dc,
                margin,
                footerY,
                width - margin,
                footerY);


            // ========================================================
            // SỐ TEM
            // ========================================================

            DrawTextRight(
                dc,
                $"Số tem: {label.SoBanIn}",
                fontNormal,
                10,
                Brushes.Black,
                new Rect(
                    margin,
                    height - 27,
                    contentWidth,
                    18));
        }


        // ============================================================
        // 50 x 50 MM
        // ============================================================

        private void Render50x50(
            DrawingContext dc,
            LabelData label,
            int width,
            int height,
            Typeface fontTitle,
            Typeface fontBold,
            Typeface fontNormal)
        {
            double margin = 16;

            double y = 10;


            // ========================================================
            // TÊN HÀNG
            // ========================================================

            double titleHeight =
                DrawTextCentered(
                    dc,
                    label.TenHang,
                    fontTitle,
                    12,
                    Brushes.Black,
                    new Rect(
                        margin,
                        y,
                        width - margin * 2,
                        40));


            y += Math.Max(
                35,
                titleHeight + 5);


            // ========================================================
            // LINE
            // ========================================================

            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 6;


            // ========================================================
            // THÔNG TIN
            // ========================================================

            DrawLabel(
                dc,
                "Thành phần:",
                label.ThanhPhan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            DrawLabel(
                dc,
                "Xuất xứ:",
                label.XuatXu,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            DrawLabel(
                dc,
                "Bảo quản:",
                label.BaoQuan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            DrawLabel(
                dc,
                "NSX:",
                label.NgaySanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            DrawLabel(
                dc,
                "HSD:",
                label.HanSuDung,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            y += 3;


            // ========================================================
            // LINE
            // ========================================================

            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 6;


            // ========================================================
            // NPP
            // ========================================================

            DrawLabel(
                dc,
                "NPP:",
                label.NhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            DrawMultilineLabel(
                dc,
                "Đ/c:",
                label.DiaChiNhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                11);


            // ========================================================
            // NƠI SX
            // ========================================================

            DrawLabel(
                dc,
                "NSX:",
                label.NoiSanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                12);


            DrawMultilineLabel(
                dc,
                "Đ/c:",
                label.DiaChiSanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                11);


            // ========================================================
            // SỐ TEM
            // ========================================================

            DrawTextRight(
                dc,
                $"Tem: {label.SoBanIn}",
                fontNormal,
                9,
                Brushes.Black,
                new Rect(
                    margin,
                    height - 17,
                    width - margin * 2,
                    12));
        }


        // ============================================================
        // 50 x 30 MM
        // ============================================================

        private void Render50x30(
            DrawingContext dc,
            LabelData label,
            int width,
            int height,
            Typeface fontTitle,
            Typeface fontBold,
            Typeface fontNormal)
        {
            double margin = 14;

            double y = 8;


            // ========================================================
            // TÊN HÀNG
            // ========================================================

            double titleHeight =
                DrawTextCentered(
                    dc,
                    label.TenHang,
                    fontTitle,
                    15,
                    Brushes.Black,
                    new Rect(
                        margin,
                        y,
                        width - margin * 2,
                        35));


            y += Math.Max(
                30,
                titleHeight + 4);


            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 5;


            // ========================================================
            // THÔNG TIN
            // ========================================================

            DrawLabel(
                dc,
                "TP:",
                label.ThanhPhan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                10);


            DrawLabel(
                dc,
                "XX:",
                label.XuatXu,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                10);


            DrawLabel(
                dc,
                "BQ:",
                label.BaoQuan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                10);


            DrawLabel(
                dc,
                "NSX:",
                label.NgaySanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                10);


            DrawLabel(
                dc,
                "HSD:",
                label.HanSuDung,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                10);


            // ========================================================
            // NPP
            // ========================================================

            if (y < height - 25)
            {
                DrawLabel(
                    dc,
                    "NPP:",
                    label.NhaPhanPhoi,
                    fontBold,
                    fontNormal,
                    margin,
                    ref y,
                    width,
                    9);
            }


            // ========================================================
            // SỐ TEM
            // ========================================================

            DrawTextRight(
                dc,
                $"Tem: {label.SoBanIn}",
                fontNormal,
                8,
                Brushes.Black,
                new Rect(
                    margin,
                    height - 13,
                    width - margin * 2,
                    10));
        }


        // ============================================================
        // 75 x 50 MM
        // ============================================================

        private void Render75x50(
            DrawingContext dc,
            LabelData label,
            int width,
            int height,
            Typeface fontTitle,
            Typeface fontBold,
            Typeface fontNormal)
        {
            double margin = 20;

            double y = 12;


            // ========================================================
            // TÊN HÀNG
            // ========================================================

            double titleHeight =
                DrawTextCentered(
                    dc,
                    label.TenHang,
                    fontTitle,
                    20,
                    Brushes.Black,
                    new Rect(
                        margin,
                        y,
                        width - margin * 2,
                        50));


            y += Math.Max(
                45,
                titleHeight + 5);


            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 7;


            // ========================================================
            // THÔNG TIN
            // ========================================================

            DrawLabel(
                dc,
                "Thành phần:",
                label.ThanhPhan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                15);


            DrawLabel(
                dc,
                "Xuất xứ:",
                label.XuatXu,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                15);


            DrawLabel(
                dc,
                "Bảo quản:",
                label.BaoQuan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                15);


            DrawLabel(
                dc,
                "NSX:",
                label.NgaySanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                15);


            DrawLabel(
                dc,
                "HSD:",
                label.HanSuDung,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                15);


            y += 5;


            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 7;


            // ========================================================
            // NPP
            // ========================================================

            DrawLabel(
                dc,
                "NPP:",
                label.NhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                14);


            DrawMultilineLabel(
                dc,
                "Đ/c:",
                label.DiaChiNhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                13);


            // ========================================================
            // SỐ TEM
            // ========================================================

            DrawTextRight(
                dc,
                $"Tem: {label.SoBanIn}",
                fontNormal,
                11,
                Brushes.Black,
                new Rect(
                    margin,
                    height - 18,
                    width - margin * 2,
                    14));
        }


        // ============================================================
        // 100 x 150 MM
        // ============================================================

        private void Render100x150(
            DrawingContext dc,
            LabelData label,
            int width,
            int height,
            Typeface fontTitle,
            Typeface fontBold,
            Typeface fontNormal)
        {
            double margin = 35;

            double y = 30;


            // ========================================================
            // TÊN HÀNG
            // ========================================================

            double titleHeight =
                DrawTextCentered(
                    dc,
                    label.TenHang,
                    fontTitle,
                    36,
                    Brushes.Black,
                    new Rect(
                        margin,
                        y,
                        width - margin * 2,
                        100));


            y += Math.Max(
                90,
                titleHeight + 10);


            // ========================================================
            // LINE
            // ========================================================

            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 20;


            // ========================================================
            // THÔNG TIN
            // ========================================================

            DrawLabel(
                dc,
                "Thành phần:",
                label.ThanhPhan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                26);


            DrawLabel(
                dc,
                "Xuất xứ:",
                label.XuatXu,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                26);


            DrawLabel(
                dc,
                "Bảo quản:",
                label.BaoQuan,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                26);


            DrawLabel(
                dc,
                "Ngày sản xuất:",
                label.NgaySanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                26);


            DrawLabel(
                dc,
                "Hạn sử dụng:",
                label.HanSuDung,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                26);


            y += 15;


            // ========================================================
            // LINE
            // ========================================================

            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 20;


            // ========================================================
            // NPP
            // ========================================================

            DrawLabel(
                dc,
                "Nhà phân phối:",
                label.NhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                26);


            DrawMultilineLabel(
                dc,
                "Địa chỉ:",
                label.DiaChiNhaPhanPhoi,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                25);


            y += 10;


            // ========================================================
            // NƠI SX
            // ========================================================

            DrawLabel(
                dc,
                "Nơi sản xuất:",
                label.NoiSanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                26);


            DrawMultilineLabel(
                dc,
                "Địa chỉ:",
                label.DiaChiSanXuat,
                fontBold,
                fontNormal,
                margin,
                ref y,
                width,
                25);


            // ========================================================
            // LINE BARCODE
            // ========================================================

            DrawLine(
                dc,
                margin,
                height - 180,
                width - margin,
                height - 180);


            // ========================================================
            // SỐ TEM
            // ========================================================

            DrawTextRight(
                dc,
                $"Số tem: {label.SoBanIn}",
                fontNormal,
                20,
                Brushes.Black,
                new Rect(
                    margin,
                    height - 60,
                    width - margin * 2,
                    30));
        }


        // ============================================================
        // DRAW LABEL
        // ============================================================

        private double DrawLabel(
            DrawingContext dc,
            string title,
            string? value,
            Typeface titleFont,
            Typeface valueFont,
            double x,
            ref double y,
            int labelWidth,
            double fontSize)
        {
            value ??= "";

            // --------------------------------------------------------
            // TÁCH TITLE VÀ VALUE
            // --------------------------------------------------------

            double titleWidth =
                MeasureTextWidth(
                    title,
                    titleFont,
                    fontSize);


            double maxWidth =
                labelWidth - x - 25;


            // --------------------------------------------------------
            // Nếu value ngắn:
            //
            // Thành phần: Thịt bò
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(value))
            {
                double valueWidth =
                    MeasureTextWidth(
                        value,
                        valueFont,
                        fontSize);


                if (titleWidth + 5 + valueWidth <= maxWidth)
                {
                    DrawText(
                        dc,
                        title,
                        titleFont,
                        fontSize,
                        Brushes.Black,
                        x,
                        y);


                    DrawText(
                        dc,
                        value,
                        valueFont,
                        fontSize,
                        Brushes.Black,
                        x + titleWidth + 5,
                        y);


                    double lineHeight =
                        GetLineHeight(
                            valueFont,
                            fontSize);


                    y += lineHeight + 3;

                    return lineHeight;
                }
            }


            // --------------------------------------------------------
            // Nếu value dài:
            //
            // Thành phần:
            // Thịt bò nhập khẩu ...
            // --------------------------------------------------------

            DrawText(
                dc,
                title,
                titleFont,
                fontSize,
                Brushes.Black,
                x,
                y);


            y += GetLineHeight(
                titleFont,
                fontSize);


            double valueMaxWidth =
                maxWidth;


            double valueHeight =
                DrawWrappedText(
                    dc,
                    value,
                    valueFont,
                    fontSize,
                    Brushes.Black,
                    x,
                    y,
                    valueMaxWidth);


            y += valueHeight + 3;


            return valueHeight;
        }


        // ============================================================
        // DRAW MULTILINE LABEL
        //
        // Ví dụ:
        //
        // Địa chỉ:
        // 123 Nguyễn Văn A, Phường...
        // Quận...
        // ============================================================

        private double DrawMultilineLabel(
            DrawingContext dc,
            string title,
            string? value,
            Typeface titleFont,
            Typeface valueFont,
            double x,
            ref double y,
            int labelWidth,
            double fontSize)
        {
            value ??= "";


            double maxWidth =
                labelWidth - x - 25;


            // --------------------------------------------------------
            // TITLE
            // --------------------------------------------------------

            DrawText(
                dc,
                title,
                titleFont,
                fontSize,
                Brushes.Black,
                x,
                y);


            y += GetLineHeight(
                titleFont,
                fontSize);


            // --------------------------------------------------------
            // VALUE
            // --------------------------------------------------------

            double valueHeight =
                DrawWrappedText(
                    dc,
                    value,
                    valueFont,
                    fontSize,
                    Brushes.Black,
                    x,
                    y,
                    maxWidth);


            y += valueHeight + 3;


            return valueHeight;
        }


        // ============================================================
        // DRAW WRAPPED TEXT
        // ============================================================

        private double DrawWrappedText(
            DrawingContext dc,
            string text,
            Typeface typeface,
            double fontSize,
            Brush brush,
            double x,
            double y,
            double maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text))
                return GetLineHeight(
                    typeface,
                    fontSize);


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
                    MaxTextWidth = maxWidth,
                    Trimming = TextTrimming.None
                };


            dc.DrawText(
                formatted,
                new Point(
                    x,
                    y));


            return formatted.Height;
        }


        // ============================================================
        // DRAW TEXT
        // ============================================================

        private void DrawText(
            DrawingContext dc,
            string? text,
            Typeface typeface,
            double fontSize,
            Brush brush,
            double x,
            double y)
        {
            if (string.IsNullOrEmpty(text))
                return;


            var formatted =
                new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    brush,
                    1.0);


            dc.DrawText(
                formatted,
                new Point(
                    x,
                    y));
        }


        // ============================================================
        // TEXT CENTER
        // ============================================================

        private double DrawTextCentered(
            DrawingContext dc,
            string? text,
            Typeface typeface,
            double fontSize,
            Brush brush,
            Rect rect)
        {
            text ??= "";


            if (string.IsNullOrWhiteSpace(text))
                return 0;


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


            double x =
                rect.X;


            double y =
                rect.Y +
                Math.Max(
                    0,
                    (rect.Height -
                     formatted.Height) / 2);


            dc.DrawText(
                formatted,
                new Point(
                    x,
                    y));


            return formatted.Height;
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
        // MEASURE TEXT WIDTH
        // ============================================================

        private double MeasureTextWidth(
            string text,
            Typeface typeface,
            double fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return 0;


            var formatted =
                new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black,
                    1.0);


            return formatted.Width;
        }


        // ============================================================
        // LINE HEIGHT
        // ============================================================

        private double GetLineHeight(
            Typeface typeface,
            double fontSize)
        {
            var formatted =
                new FormattedText(
                    "Ag",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black,
                    1.0);


            return formatted.Height;
        }


        // ============================================================
        // DRAW LINE
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


        // ============================================================
        // MM → DOTS
        // ============================================================

        private int MmToDots(
            double mm,
            int dpi)
        {
            return (int)Math.Round(
                mm / 25.4 * dpi);
        }


        // ============================================================
        // CREATE TSPL BITMAP COMMAND
        // ============================================================

        public byte[] CreateTsplBitmapCommand(
            LabelData label,
            LabelSize labelSize,
            int quantity = 1)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            if (labelSize == null)
                throw new ArgumentNullException(nameof(labelSize));


            // --------------------------------------------------------
            // TẠO BITMAP
            // --------------------------------------------------------

            BitmapSource bitmap =
                CreateLabelBitmap(
                    label,
                    labelSize);


            byte[] bitmapData =
                ConvertToMonochrome(
                    bitmap);


            int width =
                bitmap.PixelWidth;


            int height =
                bitmap.PixelHeight;


            int bytesPerRow =
                (width + 7) / 8;


            // --------------------------------------------------------
            // TSPL
            // --------------------------------------------------------

            using var stream =
                new MemoryStream();


            string header =
                $"SIZE {labelSize.WidthMm} mm,{labelSize.HeightMm} mm\r\n" +
                $"GAP 2 mm,0\r\n" +
                $"DIRECTION 1\r\n" +
                $"CLS\r\n" +
                $"BITMAP 0,0,{bytesPerRow},{height},0,";


            byte[] headerBytes =
                Encoding.ASCII.GetBytes(
                    header);


            stream.Write(
                headerBytes,
                0,
                headerBytes.Length);


            // --------------------------------------------------------
            // BITMAP DATA
            // --------------------------------------------------------

            stream.Write(
                bitmapData,
                0,
                bitmapData.Length);


            // --------------------------------------------------------
            // PRINT
            // --------------------------------------------------------

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
        // OVERLOAD CODE CŨ
        // ============================================================

        public byte[] CreateTsplBitmapCommand(
            LabelData label,
            int quantity = 1)
        {
            var defaultSize =
                new LabelSize
                {
                    Id = "A7",
                    Name = "75 x 100 mm",
                    WidthMm = 75,
                    HeightMm = 100,
                    Dpi = DefaultDpi
                };


            return CreateTsplBitmapCommand(
                label,
                defaultSize,
                quantity);
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


                    // ------------------------------------------------
                    // TRẮNG = 1
                    // ĐEN = 0
                    // ------------------------------------------------

                    if (gray >= 128)
                    {
                        int byteIndex =
                            y * bytesPerRow +
                            (x / 8);


                        int bitIndex =
                            7 - (x % 8);


                        result[byteIndex] |=
                            (byte)(
                                1 << bitIndex);
                    }
                }
            }


            return result;
        }
    }
}