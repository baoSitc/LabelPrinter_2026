using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabelPrinter.Models;
using LabelPrinter.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LabelPrinter.ViewModels

{
   
    public class LabelViewModel : INotifyPropertyChanged
    {

        private bool isPrinting;

        public ICommand PrintSelectedCommand { get; }

        private string printStatus = "";
        private readonly ExcelService _excelService;
        private readonly LabelService _labelService;
        private readonly PrinterService _printerService;

        public ICommand PrintTestCommand { get; }

        public ObservableCollection<LabelData> Labels { get; }
            = new ObservableCollection<LabelData>();

        private LabelData? _selectedLabel;

        public LabelData? SelectedLabel
        {
            get => _selectedLabel;
            set
            {
                _selectedLabel = value;
                OnPropertyChanged();
            }
        }
        // =========================================================
        // TRẠNG THÁI IN
        // =========================================================
        private void PrintSelected(object? parameter)
        {
            if (SelectedLabel == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn mặt hàng cần in.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (SelectedLabel.SoBanIn <= 0)
            {
                MessageBox.Show(
                    "Số bản in phải lớn hơn 0.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                int soBanIn = SelectedLabel.SoBanIn;

                byte[] data =
                    _labelService.CreateTsplBitmapCommand(
                        SelectedLabel,
                        soBanIn);

                _printerService.PrintRaw(
                    "TD-401",
                    data);

                MessageBox.Show(
                    $"Đã gửi lệnh in {soBanIn} tem.\n\n" +
                    $"Tên hàng: {SelectedLabel.TenHang}",
                    "In tem",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể in tem.\n\n{ex.Message}",
                    "Lỗi in",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }




        public int TongSoMatHang
        {
            get => Labels.Count;
        }

        public int TongSoTem
        {
            get => Labels.Sum(x => x.SoBanIn);
        }

        public ICommand ImportExcelCommand { get; }

        public LabelViewModel()
        {
            _excelService = new ExcelService();
            _labelService = new LabelService();
            _printerService = new PrinterService();

            ImportExcelCommand = new RelayCommand(ImportExcel);
            PrintSelectedCommand = new RelayCommand(PrintSelected);

        }
       

        private void ImportExcel(object? parameter)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn file Excel",
                Filter = "Excel (*.xlsx)|*.xlsx",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var data = _excelService.ImportLabels(
                    dialog.FileName);

                Labels.Clear();

                foreach (var item in data)
                {
                    Labels.Add(item);
                }

                OnPropertyChanged(nameof(TongSoMatHang));
                OnPropertyChanged(nameof(TongSoTem));

                MessageBox.Show(
                    $"Import thành công!\n\n" +
                    $"Số mặt hàng: {TongSoMatHang}\n" +
                    $"Tổng số tem: {TongSoTem}",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi Import Excel",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler?
            PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
