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
    public abstract class LabelServiceBase : ILabelService
    {
        protected const int Dpi = 203;

        protected abstract double WidthMm { get; }

        protected abstract double HeightMm { get; }


        // ============================================================
        // CREATE BITMAP
        // ============================================================

        public BitmapSource CreateLabelBitmap(LabelData label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            int width = MmToDots(WidthMm);
            int height = MmToDots(HeightMm);

            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                // Nền trắng
                dc.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(
                        0,
                        0,
                        width,
                        height));

                RenderLabel(
                    dc,
                    label,
                    width,
                    height);
            }

            RenderTargetBitmap bitmap =
                new RenderTargetBitmap(
                    width,
                    height,
                    Dpi,
                    Dpi,
                    PixelFormats.Pbgra32);

            bitmap.Render(visual);

            bitmap.Freeze();

            return bitmap;
        }


        // ============================================================
        // SERVICE CON CÀI ĐẶT PHẦN RENDER
        // ============================================================

        protected abstract void RenderLabel(
            DrawingContext dc,
            LabelData label,
            int width,
            int height);


        // ============================================================
        // CREATE TSPL
        // ============================================================

        public byte[] CreateTsplBitmapCommand(
            LabelData label,
            int quantity = 1)
        {
            BitmapSource bitmap =
                CreateLabelBitmap(label);

            byte[] bitmapData =
                ConvertToMonochrome(bitmap);

            int width =
                bitmap.PixelWidth;

            int height =
                bitmap.PixelHeight;

            int bytesPerRow =
                (width + 7) / 8;

            using MemoryStream stream =
                new MemoryStream();

            string header =
                $"SIZE {WidthMm} mm,{HeightMm} mm\r\n" +
                $"GAP 2 mm,0\r\n" +
                $"DIRECTION 1\r\n" +
                $"CLS\r\n" +
                $"BITMAP 0,0,{bytesPerRow},{height},0,";

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

            string footer =
                $"\r\nPRINT 1,{quantity}\r\n";

            byte[] footerBytes =
                Encoding.ASCII.GetBytes(footer);

            stream.Write(
                footerBytes,
                0,
                footerBytes.Length);

            return stream.ToArray();
        }


        // ============================================================
        // MM → DOT
        // ============================================================

        protected int MmToDots(double mm)
        {
            return (int)Math.Round(
                mm / 25.4 * Dpi);
        }


        // ============================================================
        // FONT
        // ============================================================

        protected Typeface FontBold =>
            new Typeface(
                new FontFamily("Arial"),
                FontStyles.Normal,
                FontWeights.Bold,
                FontStretches.Normal);


        protected Typeface FontNormal =>
            new Typeface(
                new FontFamily("Arial"),
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);


        protected Typeface FontTitle =>
            new Typeface(
                new FontFamily("Arial"),
                FontStyles.Normal,
                FontWeights.Bold,
                FontStretches.Normal);


        // ============================================================
        // DRAW TITLE
        // ============================================================

        protected double DrawTitle(
            DrawingContext dc,
            string? text,
            double x,
            double y,
            double width,
            double fontSize)
        {
            text ??= "";

            if (string.IsNullOrWhiteSpace(text))
                return 0;

            FormattedText formatted =
                new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    FontTitle,
                    fontSize,
                    Brushes.Black,
                    1.0);

            formatted.MaxTextWidth = width;
            formatted.MaxTextHeight = 1000;
            formatted.TextAlignment =
                TextAlignment.Center;

            formatted.Trimming =
                TextTrimming.None;

            double drawX = x;

            dc.DrawText(
                formatted,
                new Point(
                    drawX,
                    y));

            return formatted.Height;
        }


        // ============================================================
        // DRAW LABEL
        // ============================================================

        protected double DrawLabel(
            DrawingContext dc,
            string title,
            string? value,
            double x,
            double y,
            double width,
            double fontSize)
        {
            value ??= "";

            // --------------------------------------------
            // TÁCH TITLE VÀ VALUE
            // --------------------------------------------

            FormattedText titleText =
                new FormattedText(
                    title,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    FontBold,
                    fontSize,
                    Brushes.Black,
                    1.0);

            dc.DrawText(
                titleText,
                new Point(x, y));


            double valueX =
                x + titleText.Width + 5;

            double valueWidth =
                width -
                (valueX - x);


            // --------------------------------------------
            // VALUE
            // --------------------------------------------

            FormattedText valueText =
                new FormattedText(
                    value,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    FontNormal,
                    fontSize,
                    Brushes.Black,
                    1.0);

            valueText.MaxTextWidth =
                Math.Max(10, valueWidth);

            valueText.Trimming =
                TextTrimming.None;

            dc.DrawText(
                valueText,
                new Point(
                    valueX,
                    y));


            double height =
                Math.Max(
                    titleText.Height,
                    valueText.Height);

            return height + 3;
        }


        // ============================================================
        // DRAW MULTILINE
        // ============================================================

        protected double DrawMultilineLabel(
            DrawingContext dc,
            string title,
            string? value,
            double x,
            double y,
            double width,
            double fontSize)
        {
            value ??= "";

            // --------------------------------------------
            // TITLE
            // --------------------------------------------

            FormattedText titleText =
                new FormattedText(
                    title,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    FontBold,
                    fontSize,
                    Brushes.Black,
                    1.0);

            dc.DrawText(
                titleText,
                new Point(
                    x,
                    y));


            // --------------------------------------------
            // VALUE XUỐNG DÒNG
            // --------------------------------------------

            double valueY =
                y + titleText.Height + 1;

            FormattedText valueText =
                new FormattedText(
                    value,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    FontNormal,
                    fontSize,
                    Brushes.Black,
                    1.0);

            valueText.MaxTextWidth =
                width;

            valueText.Trimming =
                TextTrimming.None;

            dc.DrawText(
                valueText,
                new Point(
                    x,
                    valueY));


            return titleText.Height +
                   valueText.Height +
                   4;
        }


        // ============================================================
        // DRAW LINE
        // ============================================================

        protected void DrawLine(
            DrawingContext dc,
            double x1,
            double y1,
            double x2,
            double y2,
            double thickness = 2)
        {
            Pen pen =
                new Pen(
                    Brushes.Black,
                    thickness);

            dc.DrawLine(
                pen,
                new Point(x1, y1),
                new Point(x2, y2));
        }


        // ============================================================
        // DRAW RIGHT
        // ============================================================

        protected void DrawTextRight(
            DrawingContext dc,
            string? text,
            double x,
            double y,
            double width,
            double fontSize)
        {
            text ??= "";

            FormattedText formatted =
                new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    FontNormal,
                    fontSize,
                    Brushes.Black,
                    1.0);

            formatted.MaxTextWidth =
                width;

            formatted.TextAlignment =
                TextAlignment.Right;

            formatted.Trimming =
                TextTrimming.None;

            dc.DrawText(
                formatted,
                new Point(
                    x,
                    y));
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

                    if (gray >= 128)
                    {
                        int byteIndex =
                            y * bytesPerRow +
                            x / 8;

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