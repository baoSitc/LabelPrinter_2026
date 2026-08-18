using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.Models
{
    public static class LabelSizeCatalog
    {
        public static List<LabelSize> All { get; } =
            new List<LabelSize>
            {
                new LabelSize
                {
                    Id = "A7",
                    Name = "Tem A7",
                    WidthMm = 75,
                    HeightMm = 100,
                    Dpi = 203
                },
                 new LabelSize
                {
                    Id = "50x50",
                    Name = "Tem 50 x 50",
                    WidthMm = 50,
                    HeightMm = 50,
                    Dpi = 203
                },
                new LabelSize
                {
                    Id = "50x30",
                    Name = "Tem 50 x 30",
                    WidthMm = 50,
                    HeightMm = 30,
                    Dpi = 203
                },

                new LabelSize
                {
                    Id = "75x50",
                    Name = "Tem 75 x 50",
                    WidthMm = 75,
                    HeightMm = 50,
                    Dpi = 203
                },

                new LabelSize
                {
                    Id = "100x150",
                    Name = "Tem 100 x 150",
                    WidthMm = 100,
                    HeightMm = 150,
                    Dpi = 203
                }
            };
    }
}
