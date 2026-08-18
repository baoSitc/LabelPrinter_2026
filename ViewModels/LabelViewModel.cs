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
using System.Drawing.Printing;
using System.Collections.Specialized;

namespace LabelPrinter.ViewModels

{
    public class LabelViewModel : INotifyPropertyChanged
    {
        //in qua mạng
        private readonly NetworkPrinterService _networkPrinterService;
        //lựa chọn máy in
        public ObservableCollection<string> Printers { get; }
    = new ObservableCollection<string>();
        private string? _selectedPrinter;
        public string? SelectedPrinter
        {
            get => _selectedPrinter;

            set
            {
                if (_selectedPrinter != value)
                {
                    _selectedPrinter = value;

                    OnPropertyChanged(nameof(SelectedPrinter));

                    // Lưu máy in đã chọn
                    SavePrinterSetting();
                }
            }
        }
        private bool isPrinting;

        public ICommand PrintSelectedCommand { get; }
        public ICommand ImportExcelCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand PrintAllCommand { get; }
        public ICommand CancelPrintCommand { get; }
        private string printStatus = "";
        private readonly WordLabelService _wordLabelService;
        private readonly PrinterService _printerService;
        private readonly ExcelService _excelService;
        public ObservableCollection<LabelData> Labels { get; }
    = new ObservableCollection<LabelData>();



        //khổ Tem
        public ObservableCollection<LabelSize> LabelSizes { get; }
        private LabelSize _selectedLabelSize;

        public LabelSize SelectedLabelSize
        {
            get => _selectedLabelSize;
            set
            {
                if (_selectedLabelSize != value)
                {
                    _selectedLabelSize = value;
                    OnPropertyChanged(nameof(SelectedLabelSize));
                    //lưu khổ tem đã chọn
                    SaveLabelSizeSetting();
                }
            }
        }
        // =========================================================
        // LOAD DANH SÁCH MÁY IN
        // =========================================================

        private void LoadPrinters()
        {
            Printers.Clear();

            foreach (string printerName
                in PrinterSettings.InstalledPrinters)
            {
                Printers.Add(printerName);
            }
        }
        // =========================================================
        // LƯU MÁY IN
        // =========================================================

        private void SavePrinterSetting()
        {
            if (string.IsNullOrWhiteSpace(SelectedPrinter))
                return;

            Properties.Settings.Default.SelectedPrinter =
                SelectedPrinter;

            Properties.Settings.Default.Save();
        }
        // =========================================================
        // LƯU KHỔ TEM
        // =========================================================

        private void SaveLabelSizeSetting()
        {
            if (SelectedLabelSize == null)
                return;

            Properties.Settings.Default.SelectedLabelSizeId =
                SelectedLabelSize.Id;

            Properties.Settings.Default.Save();
        }
        // =========================================================
        // LOAD LỰA CHỌN ĐÃ LƯU
        // =========================================================

        private void LoadSavedSettings()
        {
            // -----------------------------------------------------
            // MÁY IN
            // -----------------------------------------------------

            string savedPrinter =
                Properties.Settings.Default.SelectedPrinter;

            if (!string.IsNullOrWhiteSpace(savedPrinter) &&
                Printers.Contains(savedPrinter))
            {
                SelectedPrinter = savedPrinter;
            }
            else if (Printers.Count > 0)
            {
                // Nếu chưa lưu hoặc máy in cũ không còn
                SelectedPrinter = Printers[0];
            }


            // -----------------------------------------------------
            // KHỔ TEM
            // -----------------------------------------------------

            string savedLabelSize =
                Properties.Settings.Default.SelectedLabelSizeId;

            LabelSize? labelSize =
                LabelSizes.FirstOrDefault(x =>
                    x.Id == savedLabelSize);

            if (labelSize != null)
            {
                SelectedLabelSize = labelSize;
            }
            else if (LabelSizes.Count > 0)
            {
                SelectedLabelSize = LabelSizes[0];
            }
        }
        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public LabelViewModel()
        {
            // =====================================================
            // KHỔ TEM
            // =====================================================

            LabelSizes =
                new ObservableCollection<LabelSize>(
                    LabelSizeCatalog.All);
            // =====================================================
            // SERVICES
            // =====================================================

            //SelectedLabelSize =
            //    LabelSizes.FirstOrDefault();

            _wordLabelService = new WordLabelService();


            _printerService =
                new PrinterService();

            _networkPrinterService =
                new NetworkPrinterService();

            _excelService =
                new ExcelService();
            // =====================================================
            // LOAD DANH SÁCH MÁY IN
            // =====================================================

            LoadPrinters();
            // =====================================================
            // LOAD LỰA CHỌN ĐÃ LƯU
            // =====================================================

            LoadSavedSettings();

            // =====================================================
            // COMMANDS
            // =====================================================
            ImportExcelCommand =
                new RelayCommand(ImportExcel);

            PrintSelectedCommand =
                new RelayCommand(PrintSelected);

            PrintAllCommand =
                new RelayCommand(async _ => await PrintAll());

            CancelPrintCommand =
                new RelayCommand(CancelPrint);
            CloseCommand =
                new RelayCommand(Close);
            // Theo dõi thêm / xóa dòng
            Labels.CollectionChanged += Labels_CollectionChanged;

        }
        private void Labels_CollectionChanged(
     object? sender,
     NotifyCollectionChangedEventArgs e)
        {
            // ==========================================
            // KHI THÊM
            // ==========================================

            if (e.NewItems != null)
            {
                foreach (LabelData item in e.NewItems)
                {
                    item.PropertyChanged += Label_PropertyChanged;
                }
            }


            // ==========================================
            // KHI XÓA
            // ==========================================

            if (e.OldItems != null)
            {
                foreach (LabelData item in e.OldItems)
                {
                    item.PropertyChanged -= Label_PropertyChanged;
                }
            }


            // ==========================================
            // TÍNH LẠI TỔNG
            // ==========================================

            UpdateTotals();
        }
        //public int TongSoMatHang
        //{
        //    get
        //    {
        //        return Labels.Count;
        //    }
        //}

        //public int TongSoTem
        //{
        //    get
        //    {
        //        return Labels.Sum(x => x.SoBanIn);
        //    }
        //}
        private void Label_PropertyChanged(
    object? sender,
    PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LabelData.SoBanIn))
            {
                UpdateTotals();
            }
        }
        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(TongSoMatHang));
            OnPropertyChanged(nameof(TongSoTem));
        }

        // =========================================================
        // HỦY IN
        // =========================================================

        private void CancelPrint(object? parameter)
        {
            if (!IsPrinting)
                return;


            IsPrinting = false;


            PrintStatus =
                "Đang dừng quá trình in...";
        }

        //public ObservableCollection<LabelData> Labels { get; }
        //    = new ObservableCollection<LabelData>();

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
        // IN TẤT CẢ
        // =========================================================

        private async Task PrintAll()
        {
            // -----------------------------------------------------
            // Kiểm tra dữ liệu
            // -----------------------------------------------------

            if (Labels == null || Labels.Count == 0)
            {
                MessageBox.Show(
                    "Không có dữ liệu để in.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // -----------------------------------------------------
            // Lấy các dòng có số tem > 0
            // -----------------------------------------------------

            var printItems =
                Labels
                    .Where(x => x.SoBanIn > 0)
                    .ToList();


            if (printItems.Count == 0)
            {
                MessageBox.Show(
                    "Không có mặt hàng nào có số bản in lớn hơn 0.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // -----------------------------------------------------
            // TỔNG SỐ TEM
            // -----------------------------------------------------

            int tongSoTem =
                printItems.Sum(x => x.SoBanIn);


            // -----------------------------------------------------
            // XÁC NHẬN
            // -----------------------------------------------------

            string message =
                $"Có {printItems.Count:N0} mặt hàng cần in.\n\n" +
                $"Tổng số tem: {tongSoTem:N0} tem.\n\n" +
                $"Bạn có chắc chắn muốn in tất cả không?";


            MessageBoxResult result =
                MessageBox.Show(
                    message,
                    "Xác nhận in tất cả",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (result != MessageBoxResult.Yes)
                return;


            // -----------------------------------------------------
            // BẮT ĐẦU IN
            // -----------------------------------------------------

            IsPrinting = true;

            int tongDaIn = 0;

            int matHangDaIn = 0;


            try
            {
                foreach (var label in printItems)
                {
                    if (!IsPrinting)
                        break;


                    matHangDaIn++;


                    PrintStatus =
                        $"Đang in {matHangDaIn}/{printItems.Count}: " +
                        $"{label.TenHang} - " +
                        $"{label.SoBanIn:N0} tem";


                    _wordLabelService.PrintLabel(
                        label,
                        SelectedLabelSize,
                        SelectedPrinter,
                        label.SoBanIn);


                    tongDaIn += label.SoBanIn;


                    await Task.Delay(300);
                }


                // =================================================
                // HOÀN THÀNH
                // =================================================

                if (IsPrinting)
                {
                    PrintStatus =
                        $"Hoàn tất: {tongDaIn:N0}/{tongSoTem:N0} tem";


                    MessageBox.Show(
                        $"Đã gửi lệnh in thành công.\n\n" +
                        $"Mặt hàng: {matHangDaIn:N0}\n" +
                        $"Tổng số tem: {tongDaIn:N0}",
                        "In hoàn tất",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    PrintStatus =
                        $"Đã dừng. Đã gửi {tongDaIn:N0} tem.";


                    MessageBox.Show(
                        $"Đã dừng quá trình in.\n\n" +
                        $"Đã gửi: {tongDaIn:N0} tem.",
                        "Dừng in",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                PrintStatus =
                    $"Lỗi sau khi gửi {tongDaIn:N0} tem.";


                MessageBox.Show(
                    $"Có lỗi xảy ra trong quá trình in.\n\n" +
                    $"Đã gửi: {tongDaIn:N0} tem.\n\n" +
                    $"Lỗi: {ex.Message}",
                    "Lỗi in",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsPrinting = false;
            }
        }
        // =========================================================
        // TRẠNG THÁI IN
        // =========================================================

        private bool _isPrinting;

        public bool IsPrinting
        {
            get => _isPrinting;

            set
            {
                if (_isPrinting != value)
                {
                    _isPrinting = value;

                    OnPropertyChanged(nameof(IsPrinting));
                }
            }
        }


        // =========================================================
        // TRẠNG THÁI
        // =========================================================

        private string _printStatus;

        public string PrintStatus
        {
            get => _printStatus;

            set
            {
                if (_printStatus != value)
                {
                    _printStatus = value;

                    OnPropertyChanged(nameof(PrintStatus));
                }
            }
        }

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


            if (SelectedLabelSize == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn khổ tem.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                _wordLabelService.PrintLabel(
                    SelectedLabel,
                    SelectedLabelSize,
                    SelectedPrinter,
                    SelectedLabel.SoBanIn);


                MessageBox.Show(
                    $"Đã gửi lệnh in thành công.\n\n" +
                    $"Tên hàng: {SelectedLabel.TenHang}\n" +
                    $"Số tem: {SelectedLabel.SoBanIn:N0}",
                    "In tem",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể in tem.\n\n" +
                    ex.Message,
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
        //Thoát chương trình
        void Close(object? parameter)
        {
            Application.Current.Shutdown();
        }
    }


}
