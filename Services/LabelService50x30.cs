using LabelPrinter.Models;
using System.Windows.Media;

namespace LabelPrinter.Services
{
    public class LabelService50x30 : LabelServiceBase
    {
        protected override double WidthMm => 50;

        protected override double HeightMm => 30;


        protected override void RenderLabel(
            DrawingContext dc,
            LabelData label,
            int width,
            int height)
        {
            double margin = 14;

            double contentWidth =
                width - margin * 2;

            double y = 7;


            // ========================================================
            // TÊN HÀNG
            // ========================================================

            double titleHeight =
                DrawTitle(
                    dc,
                    label.TenHang,
                    margin,
                    y,
                    contentWidth,
                    16);

            y += titleHeight + 5;


            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 4;


            // ========================================================
            // THÔNG TIN
            // ========================================================

            y += DrawLabel(
                dc,
                "TP:",
                label.ThanhPhan,
                margin,
                y,
                contentWidth,
                10);

            y += DrawLabel(
                dc,
                "XX:",
                label.XuatXu,
                margin,
                y,
                contentWidth,
                10);

            y += DrawLabel(
                dc,
                "BQ:",
                label.BaoQuan,
                margin,
                y,
                contentWidth,
                10);

            y += DrawLabel(
                dc,
                "NSX:",
                label.NgaySanXuat,
                margin,
                y,
                contentWidth,
                10);

            y += DrawLabel(
                dc,
                "HSD:",
                label.HanSuDung,
                margin,
                y,
                contentWidth,
                10);


            // ========================================================
            // FOOTER
            // ========================================================

            DrawTextRight(
                dc,
                $"Tem: {label.SoBanIn}",
                margin,
                height - 12,
                contentWidth,
                8);
        }
    }
}