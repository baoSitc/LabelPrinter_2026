using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.Models
{
    public class LabelData
    {
        public string ThanhPhan { get; set; } = "";
        public string TenHang { get; set; } = "";
        public string BaoQuan { get; set; } = "";
        public string XuatXu { get; set; } = "";
        public string NgaySanXuat { get; set; } = "";
        public string HanSuDung { get; set; } = "";
        public string HuongDanSuDung { get; set; } = "";
        public string NhaPhanPhoi { get; set; } = "";
        public string DiaChiNhaPhanPhoi { get; set; } = "";
        public string NoiSanXuat { get; set; } = "";
        public string DiaChiSanXuat { get; set; } = "";
        public int SoBanIn { get; set; } = 1;
    }
}
