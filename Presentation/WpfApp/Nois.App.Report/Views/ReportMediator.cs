using CsvHelper;
using CsvHelper.Configuration;
using Nois.App.Pager.Views;
using Nois.Core.Domain.ScannedLogs;
using Nois.Framework.Data;
using Nois.Framework.Infrastructure;
using Nois.Framework.Services;
using Nois.Services.ScannedLogs;
using Nois.Services.SortedCode;
using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace Nois.App.Report.Views
{
    public class ReportMediator : ViewModelMediator
    {
        private IScannedLogService _scannedLogService;
        private ISortedCodeDataService _barcodeDataService;
        private PagerViewModel<ScannedLogViewModel> _pagerModel;

        public ReportMediator()
            : base("ReportMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var report = new Report();
            report.DataContext = this;
            ViewComponent = report;

            _scannedLogService = EngineContext.Instance.Resolve<IScannedLogService>();
            _barcodeDataService = EngineContext.Instance.Resolve<ISortedCodeDataService>();

            _pagerModel = new PagerViewModel<ScannedLogViewModel>(GetProductScanned, DataChange);
            this.Report.pagger.DataContext = _pagerModel;

            _pagerModel.Load();

            Operators = new Dictionary<FilterOperator, string>
            {
                { FilterOperator.Equal, "=" },
                { FilterOperator.More, ">" },
                { FilterOperator.Less, "<" },
                { FilterOperator.MoreEqual, ">=" },
                { FilterOperator.LessEqual, "<=" }
            };
        }

        private Report Report
        {
            get { return (Report)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_Report_UpdateLog
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_Report_UpdateLog:
                    var scannedLogModel = (ScannedLogViewModel)notification.Body;
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        ScannedLogs.Add(scannedLogModel);
                        _pagerModel.Load();
                    });
                    return;
            }
        }

        private ICollection<ScannedLogViewModel> _scannedLogs;
        public ICollection<ScannedLogViewModel> ScannedLogs
        {
            get { return _scannedLogs; }
            set
            {
                _scannedLogs = value;
                RaisePropertyChanged("ScannedLogs");
            }
        }

        private IDictionary<FilterOperator, string> _operators;
        public IDictionary<FilterOperator, string> Operators
        {
            get { return _operators; }
            set
            {
                _operators = value;
                RaisePropertyChanged("Operators");
            }
        }

        #region Filter property
        private string _destination;
        public string Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                RaisePropertyChanged("Destination");
            }
        }

        private string _chute;
        public string Chute
        {
            get { return _chute; }
            set
            {
                _chute = value;
                RaisePropertyChanged("Chute");
            }
        }

        private int? _setting;
        public int? Setting
        {
            get { return _setting; }
            set
            {
                _setting = value;
                RaisePropertyChanged("Setting");
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private DateTime? _fromTime;
        public DateTime? FromTime
        {
            get { return _fromTime; }
            set
            {
                _fromTime = value;
                RaisePropertyChanged("FromTime");
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        private DateTime? _toTime;
        public DateTime? ToTime
        {
            get { return _toTime; }
            set
            {
                _toTime = value;
                RaisePropertyChanged("ToTime");
            }
        }

        private string _barcode;
        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                RaisePropertyChanged("Barcode");
            }
        }

        private string _settingName;
        public string SettingName
        {
            get { return _settingName; }
            set
            {
                _settingName = value;
                RaisePropertyChanged("SettingName");
            }
        }

        private int? _weight;
        public int? Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                RaisePropertyChanged("Weight");
            }
        }

        private KeyValuePair<FilterOperator, string> _weightOperator;
        public KeyValuePair<FilterOperator, string> WeightOperator
        {
            get { return string.IsNullOrEmpty(_weightOperator.Value) ? new KeyValuePair<FilterOperator, string>(FilterOperator.Equal, "=") : _weightOperator; }
            set
            {
                _weightOperator = value;
                RaisePropertyChanged("WeightOperator");
            }
        }

        private int? _length;
        public int? Length
        {
            get { return _length; }
            set
            {
                _length = value;
                RaisePropertyChanged("Length");
            }
        }

        private KeyValuePair<FilterOperator, string> _lengthOperator;
        public KeyValuePair<FilterOperator, string> LengthOperator
        {
            get { return string.IsNullOrEmpty(_lengthOperator.Value) ? new KeyValuePair<FilterOperator, string>(FilterOperator.Equal, "=") : _lengthOperator; }
            set
            {
                _lengthOperator = value;
                RaisePropertyChanged("LengthOperator");
            }
        }

        private int? _width;
        public int? Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
            }
        }

        private KeyValuePair<FilterOperator, string> _widthOperator;
        public KeyValuePair<FilterOperator, string> WidthOperator
        {
            get { return string.IsNullOrEmpty(_widthOperator.Value) ? new KeyValuePair<FilterOperator, string>(FilterOperator.Equal, "=") : _widthOperator; }
            set
            {
                _widthOperator = value;
                RaisePropertyChanged("WidthOperator");
            }
        }

        private int? _height;
        public int? Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }

        private KeyValuePair<FilterOperator, string> _heightOperator;
        public KeyValuePair<FilterOperator, string> HeightOperator
        {
            get { return string.IsNullOrEmpty(_heightOperator.Value) ? new KeyValuePair<FilterOperator, string>(FilterOperator.Equal, "=") : _heightOperator; }
            set
            {
                _heightOperator = value;
                RaisePropertyChanged("HeightOperator");
            }
        }
        #endregion

        public ICommand FilterCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _pagerModel.Load();
                });
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Destination = null;
                    Chute = null;
                    Setting = null;
                    FromDate = null;
                    FromTime = null;
                    ToDate = null;
                    ToTime = null;
                    Barcode = null;
                    SettingName = null;
                    Weight = null;
                    Length = null;
                    Width = null;
                    Height = null;
                    _pagerModel.Load();
                });
            }
        }

        public ICommand ExportCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SaveFileDialog dialog = new SaveFileDialog()
                    {
                        FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-sort",
                        Filter = "CSV (*.csv)|*.csv"
                    };

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        DateTime? from = null;
                        DateTime? to = null;
                        if (FromDate.HasValue)
                        {
                            from = FromDate.Value.Date;
                            if (FromTime.HasValue)
                            {
                                from = new DateTime(FromDate.Value.Year, FromDate.Value.Month, FromDate.Value.Day, FromTime.Value.Hour, FromTime.Value.Minute, 0, 0);
                            }
                        }
                        if (ToDate.HasValue)
                        {
                            to = ToDate.Value.Date;
                            if (ToTime.HasValue)
                            {
                                to = new DateTime(ToDate.Value.Year, ToDate.Value.Month, ToDate.Value.Day, ToTime.Value.Hour, ToTime.Value.Minute, 59, 999);
                            }
                            else
                            {
                                to = new DateTime(ToDate.Value.Year, ToDate.Value.Month, ToDate.Value.Day, 23, 59, 59, 999);
                            }
                        }
                        var scannedLogs = _scannedLogService.Search(Barcode, Destination, Chute, from, to, Setting, SettingName, Weight, WeightOperator, Length, LengthOperator, Width, WidthOperator, Height, HeightOperator);
                        using (TextWriter writer = new StreamWriter(dialog.FileName))
                        {
                            var csv = new CsvWriter(writer);
                            csv.Configuration.RegisterClassMap<ScannedLogMap>();
                            csv.Configuration.Delimiter = ",";
                            csv.WriteRecords(EntitiesToModels(scannedLogs));
                        }
                    }
                });
            }
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        private ICollection<ScannedLogViewModel> EntitiesToModels(ICollection<ScannedLog> entities)
        {
            var models = new ObservableCollection<ScannedLogViewModel>(entities.Select(scannedLog => new ScannedLogViewModel
            {
                Barcode1 = scannedLog.Barcode1,
                Barcode2 = scannedLog.Barcode2,
                Barcode3 = scannedLog.Barcode3,
                SortedCode = scannedLog.SortedCode,
                Chute = scannedLog.Chute,
                Destination = scannedLog.Destination,
                Dimension = scannedLog.Length + ";" + scannedLog.Width + ";" + scannedLog.Height,
                Setting = scannedLog.Setting,
                SettingName = scannedLog.SettingName,
                Time = scannedLog.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                Weight = scannedLog.Weight,
                Length = scannedLog.Length,
                Width = scannedLog.Width,
                Height = scannedLog.Height,
                Mode = scannedLog.Mode == 1 ? "True" : "False",
                ImportTime = scannedLog.ImportTime?.ToString("yyyy/MM/dd HH:mm:ss"),
                Remark = scannedLog.Remark,
                TransactionId = scannedLog.TransactionId
            }).ToList());

            return models;
        }

        private IPagedList<ScannedLogViewModel> GetProductScanned(int pageSize, int pageIndex)
        {
            DateTime? from = null;
            DateTime? to = null;
            if (FromDate.HasValue)
            {
                from = FromDate.Value.Date;
                if (FromTime.HasValue)
                {
                    from = new DateTime(FromDate.Value.Year, FromDate.Value.Month, FromDate.Value.Day, FromTime.Value.Hour, FromTime.Value.Minute, 0, 0);
                }
            }
            if (ToDate.HasValue)
            {
                to = ToDate.Value.Date;
                if (ToTime.HasValue)
                {
                    to = new DateTime(ToDate.Value.Year, ToDate.Value.Month, ToDate.Value.Day, ToTime.Value.Hour, ToTime.Value.Minute, 59, 999);
                }
                else
                {
                    to = new DateTime(ToDate.Value.Year, ToDate.Value.Month, ToDate.Value.Day, 23, 59, 59, 999);
                }
            }

            var scannedLogs = _scannedLogService.Search(pageIndex, pageSize, Barcode, Destination, Chute, from, to, Setting, SettingName, Weight, WeightOperator, Length, LengthOperator, Width, WidthOperator, Height, HeightOperator);

            return new PagedList<ScannedLogViewModel>(EntitiesToModels(scannedLogs), pageIndex, pageSize, scannedLogs.TotalCount) as IPagedList<ScannedLogViewModel>;
        }

        private void DataChange(IPagedList<ScannedLogViewModel> data)
        {
            ScannedLogs = new ObservableCollection<ScannedLogViewModel>(data);
        }
    }

    public sealed class ScannedLogMap : ClassMap<ScannedLogViewModel>
    {
        public ScannedLogMap()
        {
            Map(m => m.Barcode1).Name("Barcode1");
            Map(m => m.Barcode2).Name("Barcode2");
            Map(m => m.Barcode3).Name("Barcode3");
            Map(m => m.SortedCode).Name("SortedCode");
            Map(m => m.Dimension).Name("Dimension");
            Map(m => m.Weight).Name("Weight");
            Map(m => m.ImportTime).Name("DateTime");
            Map(m => m.Destination).Name("Destination");
            Map(m => m.Chute).Name("SortedChute");
            Map(m => m.Time).Name("SortedTime");
            Map(m => m.Setting).Name("Setting");
            Map(m => m.Mode).Name("CheckDuplicate");
            Map(m => m.Remark).Name("Remark");
            Map(m => m.TransactionId).Name("TransactionId");

            Map(m => m.Length).Ignore();
            Map(m => m.Width).Ignore();
            Map(m => m.Height).Ignore();
            Map(m => m.SettingName).Ignore();
        }
    }
}
