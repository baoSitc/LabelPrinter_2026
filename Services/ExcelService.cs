using ClosedXML.Excel;
using LabelPrinter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.Services
{
    public class ExcelService
    {
        public List<LabelData> ImportLabels(string filePath)
        {
            var result = new List<LabelData>();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Chưa chọn file Excel.");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(
                    "Không tìm thấy file Excel.",
                    filePath);

            using var workbook = new XLWorkbook(filePath);

            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                throw new Exception("File Excel không có Sheet.");

            var headerRow = worksheet.FirstRowUsed();

            if (headerRow == null)
                throw new Exception("File Excel không có dữ liệu.");

            // Tìm vị trí các cột
            int tenHangColumn = FindColumn(
                headerRow,
                "TenHang");

            int thanhPhanColumn = FindColumn(
                headerRow,
                "ThanhPhan");

            int baoQuanColumn = FindColumn(
                headerRow,
                "BaoQuan");

            int xuatXuColumn = FindColumn(
                headerRow,
                "XuatXu");
            int ngaySanXuatColumn = FindColumn(
               headerRow,
               "NgaySanXuat");

            int hanSuDungColumn = FindColumn(
                headerRow,
                "HanSuDung");

            int huongDanSuDungColumn = FindColumn(
                headerRow,
                "HuongDanSuDung");

            int nhaPhanPhoiColumn = FindColumn(
                headerRow,
                "NhaPhanPhoi");

            int diaChiNhaPhanPhoiColumn = FindColumn(
                headerRow,
                "DiaChiNhaPhanPhoi");

            int noiSanXuatColumn = FindColumn(
                headerRow,
                "NoiSanXuat");

            int diaChiSanXuatColumn = FindColumn(
                headerRow,
                "DiaChiSanXuat");


            int soBanInColumn = FindColumn(
                headerRow,
                "SoBanIn");


            var lastRow = worksheet.LastRowUsed();

            if (lastRow == null)
                throw new Exception("File Excel không có dữ liệu.");

            for (int row = headerRow.RowNumber() + 1;
                 row <= lastRow.RowNumber();
                 row++)
            {
                var tenHang = worksheet
                    .Cell(row, tenHangColumn)
                    .GetString()
                    .Trim();

                var thanhPhan = worksheet
                    .Cell(row, thanhPhanColumn)
                    .GetString()
                    .Trim();

                var baoQuan = worksheet
                    .Cell(row, baoQuanColumn)
                    .GetString()
                    .Trim();

                var xuatXu = worksheet
                    .Cell(row, xuatXuColumn)
                    .GetString()
                    .Trim();
                var ngaySanXuat = worksheet
                 .Cell(row, ngaySanXuatColumn)
                 .GetString()
                 .Trim();
                var hanSuDung = worksheet
                    .Cell(row, hanSuDungColumn)
                    .GetString()
                    .Trim();
                var huongDanSuDung = worksheet
                    .Cell(row, huongDanSuDungColumn)
                    .GetString()
                    .Trim();
                var nhaPhanPhoi = worksheet
                    .Cell(row, nhaPhanPhoiColumn)
                    .GetString()
                    .Trim();
                var diaChiNhaPhanPhoi = worksheet
                    .Cell(row, diaChiNhaPhanPhoiColumn)
                    .GetString()
                    .Trim();
                var noiSanXuat = worksheet
                    .Cell(row, noiSanXuatColumn)
                    .GetString()
                    .Trim();
                var diaChiSanXuat = worksheet
                    .Cell(row, diaChiSanXuatColumn)
                    .GetString()
                    .Trim();


                // Bỏ qua dòng trống
                if (string.IsNullOrWhiteSpace(tenHang) &&
                    string.IsNullOrWhiteSpace(thanhPhan) &&
                    string.IsNullOrWhiteSpace(baoQuan) &&
                    string.IsNullOrWhiteSpace(xuatXu))
                {
                    continue;
                }

            

                // Số lượng
                int soLuong = 1;

                var soLuongCell = worksheet.Cell(
                    row,
                    soBanInColumn);

                if (!soLuongCell.IsEmpty())
                {
                    if (soLuongCell.DataType == XLDataType.Number)
                    {
                        soLuong = soLuongCell.GetValue<int>();
                    }
                    else
                    {
                        int.TryParse(
                            soLuongCell.GetString(),
                            out soLuong);
                    }
                }

                if (soLuong <= 0)
                    soLuong = 1;

                result.Add(new LabelData
                {
                    ThanhPhan = thanhPhan,
                    TenHang = tenHang.ToUpper(),
                    BaoQuan = baoQuan?? "-18 độ C",
                    XuatXu = xuatXu,
                    NgaySanXuat = ngaySanXuat?? DateTime.MinValue.ToString("dd/MM/yyyy"),
                    HanSuDung = hanSuDung,
                    HuongDanSuDung = string.IsNullOrWhiteSpace(huongDanSuDung)
                            ? "Nấu chín trước khi dùng"
                            : huongDanSuDung,
                    NhaPhanPhoi = string.IsNullOrWhiteSpace(nhaPhanPhoi)
                            ? "Công Ty TNHH Thực Phẩm VietSuun Food"
                            : nhaPhanPhoi,
                    DiaChiNhaPhanPhoi = string.IsNullOrWhiteSpace(diaChiNhaPhanPhoi)
                            ? "763/5/4/19 Trường Chinh, Phường Tây Thạnh, TP.HCM"
                            : diaChiNhaPhanPhoi,
                    NoiSanXuat = string.IsNullOrWhiteSpace(noiSanXuat)
                            ? "Chi nhánh " + nhaPhanPhoi
                            : noiSanXuat,
                    DiaChiSanXuat = string.IsNullOrWhiteSpace(diaChiSanXuat)
                            ? "57 Liên Khu 2-10, Phường Bình Hưng Hòa, TP.HCM"
                            : diaChiSanXuat ,
                    SoBanIn = soLuong
                });
            }

            return result;
        }

        private int FindColumn(
            IXLRow headerRow,
            string columnName)
        {
            foreach (var cell in headerRow.CellsUsed())
            {
                if (string.Equals(
                    cell.GetString().Trim(),
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return cell.Address.ColumnNumber;
                }
            }

            throw new Exception(
                $"Không tìm thấy cột '{columnName}' trong file Excel.");
        }
    }
}
