using LabelPrinter.Models;
using System.Windows.Media;

namespace LabelPrinter.Services
{
    public class LabelService100x150 : LabelServiceBase
    {
        protected override double WidthMm => 100;

        protected override double HeightMm => 150;


        protected override void RenderLabel(
            DrawingContext dc,
            LabelData label,
            int width,
            int height)
        {
            double margin = 35;

            double contentWidth =
                width - margin * 2;

            double y = 25;


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
                    35);

            y += titleHeight + 20;


            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 15;


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
                24);

            y += DrawLabel(
                dc,
                "Xuất xứ:",
                label.XuatXu,
                margin,
                y,
                contentWidth,
                24);

            y += DrawLabel(
                dc,
                "Bảo quản:",
                label.BaoQuan,
                margin,
                y,
                contentWidth,
                24);

            y += DrawLabel(
                dc,
                "Ngày sản xuất:",
                label.NgaySanXuat,
                margin,
                y,
                contentWidth,
                24);

            y += DrawLabel(
                dc,
                "Hạn sử dụng:",
                label.HanSuDung,
                margin,
                y,
                contentWidth,
                24);


            y += 12;


            DrawLine(
                dc,
                margin,
                y,
                width - margin,
                y);

            y += 15;


            // ========================================================
            // NPP
            // ========================================================

            y += DrawLabel(
                dc,
                "Nhà phân phối:",
                label.NhaPhanPhoi,
                margin,
                y,
                contentWidth,
                24);


            y += DrawMultilineLabel(
                dc,
                "Địa chỉ:",
                label.DiaChiNhaPhanPhoi,
                margin,
                y,
                contentWidth,
                22);


            // ========================================================
            // NƠI SX
            // ========================================================

            y += DrawLabel(
                dc,
                "Nơi sản xuất:",
                label.NoiSanXuat,
                margin,
                y,
                contentWidth,
                24);


            y += DrawMultilineLabel(
                dc,
                "Địa chỉ:",
                label.DiaChiSanXuat,
                margin,
                y,
                contentWidth,
                22);


            // ========================================================
            // BARCODE LINE
            // ========================================================

            double barcodeY =
                height - 180;

            DrawLine(
                dc,
                margin,
                barcodeY,
                width - margin,
                barcodeY);


            // ========================================================
            // SỐ TEM
            // ========================================================

            DrawTextRight(
                dc,
                $"Số tem: {label.SoBanIn}",
                margin,
                height - 55,
                contentWidth,
                18);
        }
    }
}