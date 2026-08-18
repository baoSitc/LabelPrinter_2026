using LabelPrinter.Models;
using System.Windows.Media;

namespace LabelPrinter.Services
{
    public class LabelService75x50 : LabelServiceBase
    {
        protected override double WidthMm => 75;

        protected override double HeightMm => 50;


        protected override void RenderLabel(
            DrawingContext dc,
            LabelData label,
            int width,
            int height)
        {
            double margin = 20;

            double contentWidth =
                width - margin * 2;

            double y = 10;


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
                    20);

            y += titleHeight + 8;


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

            y += DrawLabel(
                dc,
                "Thành phần:",
                label.ThanhPhan,
                margin,
                y,
                contentWidth,
                13);

            y += DrawLabel(
                dc,
                "Xuất xứ:",
                label.XuatXu,
                margin,
                y,
                contentWidth,
                13);

            y += DrawLabel(
                dc,
                "Bảo quản:",
                label.BaoQuan,
                margin,
                y,
                contentWidth,
                13);

            y += DrawLabel(
                dc,
                "NSX:",
                label.NgaySanXuat,
                margin,
                y,
                contentWidth,
                13);

            y += DrawLabel(
                dc,
                "HSD:",
                label.HanSuDung,
                margin,
                y,
                contentWidth,
                13);


            y += 4;


            // ========================================================
            // NPP
            // ========================================================

            y += DrawLabel(
                dc,
                "NPP:",
                label.NhaPhanPhoi,
                margin,
                y,
                contentWidth,
                12);


            // ========================================================
            // ĐỊA CHỈ
            // ========================================================

            y += DrawMultilineLabel(
                dc,
                "Đ/c:",
                label.DiaChiNhaPhanPhoi,
                margin,
                y,
                contentWidth,
                11);


            // ========================================================
            // FOOTER
            // ========================================================

            DrawTextRight(
                dc,
                $"Tem: {label.SoBanIn}",
                margin,
                height - 15,
                contentWidth,
                9);
        }
    }
}