using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.Models
{
    public class LabelSize
    {
        public string Id { get; set; } = "";

        public string Name { get; set; } = "";

        /// <summary>
        /// Chiều rộng tem tính bằng mm
        /// </summary>
        public double WidthMm { get; set; }

        /// <summary>
        /// Chiều cao tem tính bằng mm
        /// </summary>
        public double HeightMm { get; set; }

        /// <summary>
        /// DPI máy in
        /// </summary>
        public int Dpi { get; set; } = 203;

        public override string ToString()
        {
            return $"{Name} ({WidthMm} x {HeightMm} mm)";
        }
    }
}
