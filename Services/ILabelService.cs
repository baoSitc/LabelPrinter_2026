using LabelPrinter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LabelPrinter.Services
{
    public interface ILabelService
    {
        BitmapSource CreateLabelBitmap(LabelData label);

        byte[] CreateTsplBitmapCommand(
            LabelData label,
            int quantity = 1);
    }
}
