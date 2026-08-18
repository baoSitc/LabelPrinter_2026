using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LabelPrinter.Models
{
    public class LabelData : INotifyPropertyChanged
    {
        private string? _tenHang;
        private string? _thanhPhan;
        private string? _xuatXu;
        private string? _baoQuan;
        private string? _ngaySanXuat;
        private string? _hanSuDung;
        private string? _huongDanSuDung;
        private string? _nhaPhanPhoi;
        private string? _diaChiNhaPhanPhoi;
        private string? _noiSanXuat;
        private string? _diaChiSanXuat;
        private int _soBanIn=1;


        public string? TenHang
        {
            get => _tenHang;
            set
            {
                if (_tenHang != value)
                {
                    _tenHang = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? ThanhPhan
        {
            get => _thanhPhan;
            set
            {
                if (_thanhPhan != value)
                {
                    _thanhPhan = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? XuatXu
        {
            get => _xuatXu;
            set
            {
                if (_xuatXu != value)
                {
                    _xuatXu = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? BaoQuan
        {
            get => _baoQuan;
            set
            {
                if (_baoQuan != value)
                {
                    _baoQuan = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? NgaySanXuat
        {
            get => _ngaySanXuat;
            set
            {
                if (_ngaySanXuat != value)
                {
                    _ngaySanXuat = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? HanSuDung
        {
            get => _hanSuDung;
            set
            {
                if (_hanSuDung != value)
                {
                    _hanSuDung = value;
                    OnPropertyChanged();
                }
            }
        }

       public string? HuongDanSuDung
        {
            get => _huongDanSuDung;
            set
            {
                if (_huongDanSuDung != value)
                {
                    _huongDanSuDung = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? NhaPhanPhoi
        {
            get => _nhaPhanPhoi;
            set
            {
                if (_nhaPhanPhoi != value)
                {
                    _nhaPhanPhoi = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? DiaChiNhaPhanPhoi
        {
            get => _diaChiNhaPhanPhoi;
            set
            {
                if (_diaChiNhaPhanPhoi != value)
                {
                    _diaChiNhaPhanPhoi = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? NoiSanXuat
        {
            get => _noiSanXuat;
            set
            {
                if (_noiSanXuat != value)
                {
                    _noiSanXuat = value;
                    OnPropertyChanged();
                }
            }
        }


        public string? DiaChiSanXuat
        {
            get => _diaChiSanXuat;
            set
            {
                if (_diaChiSanXuat != value)
                {
                    _diaChiSanXuat = value;
                    OnPropertyChanged();
                }
            }
        }


        public int SoBanIn
        {
            get => _soBanIn;
            set
            {
                if (_soBanIn != value)
                {
                    _soBanIn = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;


        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}