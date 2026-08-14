using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string tenHang = string.Empty;

        [ObservableProperty]
        private string maHang = string.Empty;

        [ObservableProperty]
        private decimal gia = 0;

        [ObservableProperty]
        private int soLuong = 1;

        [RelayCommand]
        private void InTem()
        {
            // Sẽ xử lý in tem ở bước sau
        }

        [RelayCommand]
        private void Xoa()
        {
            TenHang = string.Empty;
            MaHang = string.Empty;
            Gia = 0;
            SoLuong = 1;
        }
    }
}
