using Nois.App.Operation.Models;
using Nois.Core.Domain.Chutes;
using Nois.Core.Domain.Mappings;
using Nois.Core.Domain.ScannedLogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Nois.WpfApp.Framework.View;
using Nois.WpfApp.Framework.Logger;
using Nois.Services.Mappings;
using Nois.Services.ScannedLogs;
using Nois.Services.Chutes;
using Nois.Services.SortedCode;
using Nois.Framework.Localization;
using Nois.Framework.Infrastructure;
using Nois.App.Pager.Views;
using Nois.WpfApp.Framework;
using Nois.Core.Domain.SortedCode;
using CsvHelper;
using CsvHelper.Configuration;
using Nois.Framework.Data;
using Nois.Framework.Services;

namespace Nois.App.Operation.Views
{
    public class OperationMediator : ViewModelMediator
    {
        private static readonly INoisLogger _log = NoisLogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));
        private IMappingService _mappingService;
        private IScannedLogService _scannedLogService;
        private IChuteService _chuteService;
        private ISortedCodeDataService _barcodeDataService;
        private ILocalizationService _localStringResourceService;
        private DateTime _startTime;
        private Thread _autoReport;
        private PagerViewModel<ScannedLogViewModel> _pagerModel;
        private int _timeAutoReport, _countAutoReport;
        private string _ftpHost, _ftpUsername, _ftpPassword;
        private List<ScannedTrackModel> _tempList;

        private List<SortedCodeData> _validBarcodes;

        public OperationMediator()
            : base("OperationMediator")
        {
        }

        public override void OnRegister()
        {
            base.OnRegister();

            var operation = new Operation();
            operation.DataContext = this;
            ViewComponent = operation;

            _mappingService = EngineContext.Instance.Resolve<IMappingService>();
            _scannedLogService = EngineContext.Instance.Resolve<IScannedLogService>();
            _chuteService = EngineContext.Instance.Resolve<IChuteService>();
            _barcodeDataService = EngineContext.Instance.Resolve<ISortedCodeDataService>();
            _localStringResourceService = EngineContext.Instance.Resolve<ILocalizationService>();

            _timeAutoReport = int.Parse(ConfigurationManager.AppSettings["TimeAutoReport"]);
            _countAutoReport = int.Parse(ConfigurationManager.AppSettings["CountAutoReport"]);
            _ftpHost = ConfigurationManager.AppSettings["FTPHost"];
            _ftpUsername = ConfigurationManager.AppSettings["FTPUsername"];
            _ftpPassword = ConfigurationManager.AppSettings["FTPPassword"];
            _allData = _scannedLogService.GetScannedLogsHavenotSent().Select(s => ToModel(s)).ToList();// new List<ScannedLogViewModel>();
            _pagerModel = new PagerViewModel<ScannedLogViewModel>(GetProductScanned, DataChange);
            this.Operation.pagger.DataContext = _pagerModel;
            _pagerModel.Load();

            Columns = new ObservableCollection<SelectColumn>(GlobalVariable.BarcodeFields.Select(b => new SelectColumn { Title = b, IsSelected = true, CheckboxCheck = ColumnChecked }));
            ColumnText = $"{Columns.Count - 1} Selected Columns";

            var mappingList = _mappingService.GetAll();
            Mappings = new ObservableCollection<Mapping>(mappingList);

            _validBarcodes = new List<SortedCodeData>();
            //ScannedLogs = new ObservableCollection<ScannedLogViewModel>();
            _tempList = new List<ScannedTrackModel>();

            _startTime = DateTime.Now;
            Refresh();

            int.TryParse(ConfigurationManager.AppSettings["Receiver"], out _receiver);
            int.TryParse(ConfigurationManager.AppSettings["Sender"], out _sender);
            int.TryParse(ConfigurationManager.AppSettings["Status"], out _status);
            int.TryParse(ConfigurationManager.AppSettings["Update"], out _update);
            int.TryParse(ConfigurationManager.AppSettings["Heartbeat"], out _heartbeat);
        }

        private Operation Operation
        {
            get { return (Operation)ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {
                SendCommand.App_Operation_DataReceived,
                SendCommand.App_Operation_ScanSuccess,
                SendCommand.App_Operation_ScanFail,
                SendCommand.App_Operation_ValidBarcodes,
                SendCommand.App_Stop,
                SendCommand.App_Operation_ChangeStatusPort,
                SendCommand.App_Shell
            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                case SendCommand.App_Shell:
                    var defaultMapping = _mappingService.GetDefaultMapping();
                    if (defaultMapping != null)
                    {
                        SelectedMapping = defaultMapping;
                        ApplyMap();
                    }
                    return;
                case SendCommand.App_Operation_ChangeStatusPort:
                    var p = (int)notification.Body;
                    var status = notification.Type;
                    if (p == Sender)
                    {
                        SenderStatus = status.Equals("Connected") ? "Green" : "Red";
                        return;
                    }
                    if (p == Receiver)
                    {
                        ReceiverStatus = status.Equals("Connected") ? "Green" : "Red";
                        return;
                    }
                    if (p == Status)
                    {
                        StatusStatus = status.Equals("Connected") ? "Green" : "Red";
                        return;
                    }
                    if (p == Update)
                    {
                        UpdateStatus = status.Equals("Connected") ? "Green" : "Red";
                        return;
                    }
                    if (p == Heartbeat)
                    {
                        HeartbeatStatus = status.Equals("Connected") ? "Green" : "Red";
                        return;
                    }
                    return;
                case SendCommand.App_Operation_DataReceived:
                    var clientMessage = (string)notification.Body;
                    var port = int.Parse(notification.Type);
                    //remove <STX><ETX>
                    var message = clientMessage.Substring(1, clientMessage.Length - 2);
                    if (port == Receiver)
                    {
                        _log.Info("Receiver", "Receive from port " + port + ":" + clientMessage);
                        if (MappingModels != null)
                        {
                            SendNotification(CommandName.OperationController, message);
                        }
                        return;
                    }
                    if (port == Sender)
                    {
                        _log.Info("Sender", "Receive from port " + port + ":" + clientMessage);
                        return;
                    }
                    if (port == Status)
                    {
                        _log.Info("Status", "Receive from port " + port + ":" + clientMessage);
                        //split message
                        var m = message.Substring(0, 6);
                        var data = m.Replace(" ", "").Split(';');
                        _log.Info("Status", "Device: " + data[0]);
                        _log.Info("Status", "Status: " + data[1]);
                        switch (data[0])
                        {
                            case "A001":
                                A001 = data[1];
                                break;
                            case "A002":
                                A002 = data[1];
                                break;
                            case "A003":
                                A003 = data[1];
                                break;
                            case "A004":
                                A004 = data[1];
                                break;
                            case "A005":
                                A005 = data[1];
                                break;
                            case "A006":
                                A006 = data[1];
                                break;
                            case "A007":
                                A007 = data[1];
                                break;
                            case "A008":
                                A008 = data[1];
                                break;
                            case "A009":
                                A009 = data[1];
                                break;
                            case "A010":
                                A010 = data[1];
                                break;
                            case "A011":
                                A011 = data[1];
                                break;
                            case "A012":
                                A012 = data[1];
                                break;
                            case "A013":
                                A013 = data[1];
                                break;
                            case "A014":
                                A014 = data[1];
                                break;
                            case "A015":
                                A015 = data[1];
                                break;
                            case "A016":
                                A016 = data[1];
                                break;
                            case "A017":
                                A017 = data[1];
                                break;
                            case "A018":
                                A018 = data[1];
                                break;
                            case "A019":
                                A019 = data[1];
                                break;
                            case "A020":
                                A020 = data[1];
                                break;
                            case "A021":
                                A021 = data[1];
                                break;
                            case "A022":
                                A022 = data[1];
                                break;
                            case "A023":
                                A023 = data[1];
                                break;
                            case "A024":
                                A024 = data[1];
                                break;
                            case "A025":
                                A025 = data[1];
                                break;
                            case "A026":
                                A026 = data[1];
                                break;
                            default:
                                break;
                        }
                        return;
                    }
                    if (port == Update)
                    {
                        _log.Info("Update", "Receive from port " + port + ":" + clientMessage);
                        //split message
                        var data = message.Split(new[] { "//" }, StringSplitOptions.None);
                        _log.Info("Update", "Index: " + data[0]);
                        _log.Info("Update", "Chute no: " + data[1]);
                        int.TryParse(data[0], out int i);
                        var log = _tempList.OrderBy(t => t.ScannedTime).LastOrDefault(x => x.Index == i);
                        if (log != null && (DateTime.Now - log.ScannedTime).TotalMilliseconds <= 5 * 60 * 1000)
                        {
                            //update entity
                            log.ScannedLog.PLCIndex = i;
                            log.ScannedLog.ActualSortedChute = Chutes.FirstOrDefault(x => x.ChuteNo.Equals(data[1])).Name;
                            log.ScannedLog.ActualSortedTime = DateTime.Now;
                            _scannedLogService.Update(log.ScannedLog);
                            //remove from temp list
                            _tempList.Remove(log);
                            _log.Info("Update", "Update chute result: Updated");
                        }
                        else
                        {
                            _log.Info("Update", "Update chute result: The Index is not found so can't update result");
                        }
                        return;
                    }
                    if (port == Heartbeat)
                    {
                        _log.Info("Heartbeat", "Receive from port " + port + ":" + clientMessage);
                        return;
                    }
                    return;
                case SendCommand.App_Operation_ScanSuccess:
                    var scannedLog = (ScannedLog)notification.Body;
                    int.TryParse(notification.Type, out int index);
                    var mappingModel = MappingModels.FirstOrDefault(x => x.Destinations.Select(y => y.Name).ToList().Contains(scannedLog.Destination));

                    //5. check destiantion
                    if (mappingModel == null)
                    {
                        _log.Info("Operation", "Reject: Destination is not found");
                        //reject
                        scannedLog.Remark = _localStringResourceService.GetResource("REMARK.DESTINATION IS NOT FOUND");
                        SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index.ToString());
                        return;
                    }

                    //check chute is active
                    if (!mappingModel.IsActive)
                    {
                        _log.Info("Operation", string.Format("Reject: Chute {0} is inactive.", mappingModel.Chute));
                        //reject
                        scannedLog.Remark = _localStringResourceService.GetResource("REMARK.CHUTE IS INACTIVE");
                        SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index.ToString());
                        return;
                    }

                    //6. check mode
                    if (scannedLog.Mode == 1)
                    {
                        if (_validBarcodes.Select(x => x.Flag).ToList().Contains(true))
                        {
                            _log.Info("Operation", string.Format("Reject: Barcode {0} was scanned least one time before", _validBarcodes.FirstOrDefault(x => x.Flag).SortedCode));
                            //reject
                            scannedLog.Remark = _localStringResourceService.GetResource("REMARK.ONE OF THE BARCODE WAS SCANNED LEAST ONE TIME BEFORE");
                            SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index.ToString());
                            return;
                        }
                        _barcodeDataService.UpdateFlag(_validBarcodes.Select(b => b.Id).ToList());
                    }

                    //check reject chute
                    if (_chuteService.CheckIsReject(scannedLog.Chute))
                    {
                        _log.Info("Operation", "Reject: Reject by host");
                        //reject
                        scannedLog.Remark = _localStringResourceService.GetResource("REMARK.REJECT BY HOST");
                    }

                    //update
                    UpdateData(scannedLog, mappingModel, index);
                    return;
                case SendCommand.App_Operation_ScanFail:
                    scannedLog = (ScannedLog)notification.Body;
                    int.TryParse(notification.Type, out index);
                    mappingModel = MappingModels.FirstOrDefault(x => x.ChuteNo.Equals("000"));

                    //update
                    UpdateData(scannedLog, mappingModel, index);
                    return;
                case SendCommand.App_Operation_ValidBarcodes:
                    _validBarcodes = notification.Body as List<SortedCodeData>;
                    return;
                case SendCommand.App_Stop:
                    _log.Info("Operation", "Log out");
                    var currentActiveMap = _mappingService.GetAll().FirstOrDefault(x => x.IsActive);
                    if (currentActiveMap != null)
                    {
                        currentActiveMap.IsActive = false;
                        _mappingService.Update(currentActiveMap);
                    }
                    SelectedMapping = null;
                    MappingModels = null;
                    if (_autoReport != null)
                    {
                        _autoReport.Abort();
                    }
                    return;
            }
        }

        #region Chute
        private ICollection<Chute> _chutes;
        public ICollection<Chute> Chutes
        {
            get { return _chutes; }
            set
            {
                _chutes = value;
                RaisePropertyChanged("Chutes");
            }
        }

        private Chute _chute1;
        public Chute Chute1
        {
            get { return _chute1; }
            set
            {
                _chute1 = value;
                RaisePropertyChanged("Chute1");
            }
        }

        private Chute _chute2;
        public Chute Chute2
        {
            get { return _chute2; }
            set
            {
                _chute2 = value;
                RaisePropertyChanged("Chute2");
            }
        }

        private Chute _chute3;
        public Chute Chute3
        {
            get { return _chute3; }
            set
            {
                _chute3 = value;
                RaisePropertyChanged("Chute3");
            }
        }

        private Chute _chute4;
        public Chute Chute4
        {
            get { return _chute4; }
            set
            {
                _chute4 = value;
                RaisePropertyChanged("Chute4");
            }
        }

        private Chute _chute5;
        public Chute Chute5
        {
            get { return _chute5; }
            set
            {
                _chute5 = value;
                RaisePropertyChanged("Chute5");
            }
        }

        private Chute _chute6;
        public Chute Chute6
        {
            get { return _chute6; }
            set
            {
                _chute6 = value;
                RaisePropertyChanged("Chute6");
            }
        }

        private Chute _chute7;
        public Chute Chute7
        {
            get { return _chute7; }
            set
            {
                _chute7 = value;
                RaisePropertyChanged("Chute7");
            }
        }

        private Chute _chute8;
        public Chute Chute8
        {
            get { return _chute8; }
            set
            {
                _chute8 = value;
                RaisePropertyChanged("Chute8");
            }
        }

        private Chute _chute9;
        public Chute Chute9
        {
            get { return _chute9; }
            set
            {
                _chute9 = value;
                RaisePropertyChanged("Chute9");
            }
        }

        private Chute _chute10;
        public Chute Chute10
        {
            get { return _chute10; }
            set
            {
                _chute10 = value;
                RaisePropertyChanged("Chute10");
            }
        }

        private Chute _chute11;
        public Chute Chute11
        {
            get { return _chute11; }
            set
            {
                _chute11 = value;
                RaisePropertyChanged("Chute11");
            }
        }

        private Chute _chute12;
        public Chute Chute12
        {
            get { return _chute12; }
            set
            {
                _chute12 = value;
                RaisePropertyChanged("Chute12");
            }
        }

        private Chute _chute13;
        public Chute Chute13
        {
            get { return _chute13; }
            set
            {
                _chute13 = value;
                RaisePropertyChanged("Chute13");
            }
        }

        private Chute _chute14;
        public Chute Chute14
        {
            get { return _chute14; }
            set
            {
                _chute14 = value;
                RaisePropertyChanged("Chute14");
            }
        }

        private Chute _chute15;
        public Chute Chute15
        {
            get { return _chute15; }
            set
            {
                _chute15 = value;
                RaisePropertyChanged("Chute15");
            }
        }

        private Chute _chute16;
        public Chute Chute16
        {
            get { return _chute16; }
            set
            {
                _chute16 = value;
                RaisePropertyChanged("Chute16");
            }
        }

        private Chute _chute17;
        public Chute Chute17
        {
            get { return _chute17; }
            set
            {
                _chute17 = value;
                RaisePropertyChanged("Chute17");
            }
        }

        private Chute _chute18;
        public Chute Chute18
        {
            get { return _chute18; }
            set
            {
                _chute18 = value;
                RaisePropertyChanged("Chute18");
            }
        }
        #endregion

        #region Map
        private ICollection<Mapping> _mappings;
        public ICollection<Mapping> Mappings
        {
            get { return _mappings; }
            set
            {
                _mappings = value;
                RaisePropertyChanged("Mappings");
            }
        }

        private ICollection<MappingModel> _mappingModels;
        public ICollection<MappingModel> MappingModels
        {
            get { return _mappingModels; }
            set
            {
                _mappingModels = value;
                RaisePropertyChanged("MappingModels");
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

        private Mapping _selectedMapping;
        public Mapping SelectedMapping
        {
            get { return _selectedMapping; }
            set
            {
                _selectedMapping = value;
                RaisePropertyChanged("SelectedMapping");
            }
        }
        #endregion

        #region Status
        private int _sender;
        public int Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                RaisePropertyChanged("Sender");
            }
        }
        private int _receiver;
        public int Receiver
        {
            get { return _receiver; }
            set
            {
                _receiver = value;
                RaisePropertyChanged("Receiver");
            }
        }
        private int _update;
        public int Update
        {
            get { return _update; }
            set
            {
                _update = value;
                RaisePropertyChanged("Update");
            }
        }
        private int _status;
        public int Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }
        private int _heartbeat;
        public int Heartbeat
        {
            get { return _heartbeat; }
            set
            {
                _heartbeat = value;
                RaisePropertyChanged("Heartbeat");
            }
        }
        private string _senderStatus = "Red";
        public string SenderStatus
        {
            get { return _senderStatus; }
            set
            {
                _senderStatus = value;
                RaisePropertyChanged("SenderStatus");
            }
        }
        private string _receiverStatus = "Red";
        public string ReceiverStatus
        {
            get { return _receiverStatus; }
            set
            {
                _receiverStatus = value;
                RaisePropertyChanged("ReceiverStatus");
            }
        }
        private string _updateStatus = "Red";
        public string UpdateStatus
        {
            get { return _updateStatus; }
            set
            {
                _updateStatus = value;
                RaisePropertyChanged("UpdateStatus");
            }
        }
        private string _statusStatus = "Red";
        public string StatusStatus
        {
            get { return _statusStatus; }
            set
            {
                _statusStatus = value;
                RaisePropertyChanged("StatusStatus");
            }
        }
        private string _heartbeatStatus = "Red";
        public string HeartbeatStatus
        {
            get { return _heartbeatStatus; }
            set
            {
                _heartbeatStatus = value;
                RaisePropertyChanged("HeartbeatStatus");
            }
        }
        private string _a001;
        public string A001
        {
            get { return _a001; }
            set
            {
                _a001 = value;
                RaisePropertyChanged("A001");
            }
        }
        private string _a002;
        public string A002
        {
            get { return _a002; }
            set
            {
                _a002 = value;
                RaisePropertyChanged("A002");
            }
        }
        private string _a003;
        public string A003
        {
            get { return _a003; }
            set
            {
                _a003 = value;
                RaisePropertyChanged("A003");
            }
        }
        private string _a004;
        public string A004
        {
            get { return _a004; }
            set
            {
                _a004 = value;
                RaisePropertyChanged("A004");
            }
        }
        private string _a005;
        public string A005
        {
            get { return _a005; }
            set
            {
                _a005 = value;
                RaisePropertyChanged("A005");
            }
        }
        private string _a006;
        public string A006
        {
            get { return _a006; }
            set
            {
                _a006 = value;
                RaisePropertyChanged("A006");
            }
        }
        private string _a007;
        public string A007
        {
            get { return _a007; }
            set
            {
                _a007 = value;
                RaisePropertyChanged("A007");
            }
        }
        private string _a008;
        public string A008
        {
            get { return _a008; }
            set
            {
                _a008 = value;
                RaisePropertyChanged("A008");
            }
        }
        private string _a009;
        public string A009
        {
            get { return _a009; }
            set
            {
                _a009 = value;
                RaisePropertyChanged("A009");
            }
        }
        private string _a010;
        public string A010
        {
            get { return _a010; }
            set
            {
                _a010 = value;
                RaisePropertyChanged("A010");
            }
        }
        private string _a011;
        public string A011
        {
            get { return _a011; }
            set
            {
                _a011 = value;
                RaisePropertyChanged("A011");
            }
        }
        private string _a012;
        public string A012
        {
            get { return _a012; }
            set
            {
                _a012 = value;
                RaisePropertyChanged("A012");
            }
        }
        private string _a013;
        public string A013
        {
            get { return _a013; }
            set
            {
                _a013 = value;
                RaisePropertyChanged("A013");
            }
        }
        private string _a014;
        public string A014
        {
            get { return _a014; }
            set
            {
                _a014 = value;
                RaisePropertyChanged("A014");
            }
        }
        private string _a015;
        public string A015
        {
            get { return _a015; }
            set
            {
                _a015 = value;
                RaisePropertyChanged("A015");
            }
        }
        private string _a016;
        public string A016
        {
            get { return _a016; }
            set
            {
                _a016 = value;
                RaisePropertyChanged("A016");
            }
        }
        private string _a017;
        public string A017
        {
            get { return _a017; }
            set
            {
                _a017 = value;
                RaisePropertyChanged("A017");
            }
        }
        private string _a018;
        public string A018
        {
            get { return _a018; }
            set
            {
                _a018 = value;
                RaisePropertyChanged("A018");
            }
        }
        private string _a019;
        public string A019
        {
            get { return _a019; }
            set
            {
                _a019 = value;
                RaisePropertyChanged("A019");
            }
        }
        private string _a020;
        public string A020
        {
            get { return _a020; }
            set
            {
                _a020 = value;
                RaisePropertyChanged("A020");
            }
        }
        private string _a021;
        public string A021
        {
            get { return _a021; }
            set
            {
                _a021 = value;
                RaisePropertyChanged("A021");
            }
        }
        private string _a022;
        public string A022
        {
            get { return _a022; }
            set
            {
                _a022 = value;
                RaisePropertyChanged("A022");
            }
        }
        private string _a023;
        public string A023
        {
            get { return _a023; }
            set
            {
                _a023 = value;
                RaisePropertyChanged("A023");
            }
        }
        private string _a024;
        public string A024
        {
            get { return _a024; }
            set
            {
                _a024 = value;
                RaisePropertyChanged("A024");
            }
        }
        private string _a025;
        public string A025
        {
            get { return _a025; }
            set
            {
                _a025 = value;
                RaisePropertyChanged("A025");
            }
        }
        private string _a026;
        public string A026
        {
            get { return _a026; }
            set
            {
                _a026 = value;
                RaisePropertyChanged("A026");
            }
        }
        #endregion

        private ObservableCollection<SelectColumn> _columns;
        public ObservableCollection<SelectColumn> Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                RaisePropertyChanged("Columns");
            }
        }

        private string _columnText;
        public string ColumnText
        {
            get { return _columnText; }
            set
            {
                _columnText = value;
                RaisePropertyChanged("ColumnText");
            }
        }

        private int _start;
        public int Start
        {
            get { return _start; }
            set
            {
                _start = value;
                RaisePropertyChanged("Start");
            }
        }

        private int _end;
        public int End
        {
            get { return _end; }
            set
            {
                _end = value;
                RaisePropertyChanged("Start");
            }
        }

        private int _total;
        public int Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged("Total");
            }
        }

        public ICommand ColumnChecked
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var column = (SelectColumn)obj;
                    if (column.Title == "All")
                        foreach (var col in Columns)
                            col.IsSelected = column.IsSelected;
                    else
                        Columns[0].IsSelected = Columns.Skip(1).All(c => c.IsSelected);
                    ColumnText = $"{Columns.Skip(1).Count(c => c.IsSelected)} Selected Columns";
                });
            }
        }

        public ICommand ApplyCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _log.Info("Operation", "Apply click");
                    ApplyMap();
                });
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _log.Info("Operation", "Reset click");
                    if (MappingModels != null)
                    {
                        foreach (var item in MappingModels)
                        {
                            item.Quantity = 0;
                        }
                    }
                });
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Refresh();
                });
            }
        }

        public ICommand ComboboxCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var mappingList = _mappingService.GetAll();
                    Mappings = new ObservableCollection<Mapping>(mappingList);
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

        public void Refresh()
        {
            try
            {
                Chutes = _chuteService.GetAll();
                Chute1 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("001"));
                Chute2 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("002"));
                Chute3 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("003"));
                Chute4 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("004"));
                Chute5 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("005"));
                Chute6 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("006"));
                Chute7 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("007"));
                Chute8 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("008"));
                Chute9 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("009"));
                Chute10 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("010"));
                Chute11 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("011"));
                Chute12 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("012"));
                Chute13 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("013"));
                Chute14 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("014"));
                Chute15 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("015"));
                Chute16 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("016"));
                Chute17 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("017"));
                Chute18 = Chutes.FirstOrDefault(x => x.ChuteNo.Equals("018"));
            }
            catch (Exception ex)
            {
                _log.Info("Operation", "Can't get list of chute. " + ex.Message);
            }
        }

        private void UpdateData(ScannedLog scannedLog, MappingModel mappingModel, int index)
        {
            //update chute
            scannedLog.Chute = mappingModel.Chute;
            _log.Info("Operation", "Receive to " + mappingModel.Chute);

            //insert log
            _scannedLogService.Insert(scannedLog);

            //update list
            var scannedLogModel = ToModel(scannedLog);

            Application.Current.Dispatcher.Invoke(delegate
            {
                _allData.Add(scannedLogModel);
                _pagerModel.Load();
            });

            //update quantity
            mappingModel.Quantity++;

            //update report
            SendNotification(SendCommand.App_Report_UpdateLog, scannedLogModel);

            //send plc command
            var command = string.Format("{0}{1}//{2}{3}", (char)2, index.ToString("d3"), mappingModel.ChuteNo, (char)3);
            SendNotification(SendCommand.App_Client_Write, command);

            //add to tempList
            _tempList.Add(new ScannedTrackModel { Index = index, ScannedLog = scannedLog, ScannedTime = DateTime.Now });
        }
        private ScannedLogViewModel ToModel(ScannedLog scannedLog)
        {
            var scannedLogModel = new ScannedLogViewModel
            {
                Id = scannedLog.Id,
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
                ImportTime = scannedLog.ImportTime?.ToString("yyyy/MM/dd HH:mm:ss"),
                Weight = scannedLog.Weight,
                Mode = scannedLog.Mode == 1 ? "True" : "False",
                Length = scannedLog.Length,
                Width = scannedLog.Width,
                Height = scannedLog.Height,
                Remark = scannedLog.Remark,
                TransactionId = scannedLog.TransactionId
            };
            return scannedLogModel;
        }
        private void ApplyMap()
        {
            if (SelectedMapping != null)
            {
                var list = SelectedMapping.MappingItems.Select(mappingItem => new MappingModel
                {
                    ChuteNo = mappingItem.Chute.ChuteNo,
                    Chute = mappingItem.Chute.Name,
                    Destinations = new ObservableCollection<Destination>(mappingItem.Destinations),
                    Quantity = 0,
                    IsActive = mappingItem.Chute.IsActive
                }).ToList();
                MappingModels = new ObservableCollection<MappingModel>(list);

                //update active map
                var oldActiveMap = _mappingService.GetAll().FirstOrDefault(x => x.IsActive);
                if (oldActiveMap != null)
                {
                    oldActiveMap.IsActive = false;
                    _mappingService.Update(oldActiveMap);
                }
                SelectedMapping.IsActive = true;
                _mappingService.Update(SelectedMapping);

                //auto report
                if (_autoReport == null || !_autoReport.IsAlive)
                {
                    _autoReport = new Thread(AutoReport) { IsBackground = true };
                    _autoReport.Start();
                }
            }
            else
            {
                _log.Info("Operation", "Abort: Selected mapping is null");
            }
        }

        private void AutoReport()
        {
            while (true)
            {
                try
                {
                    var curTime = DateTime.Now;
                    var runningTime = (curTime - _startTime).TotalMinutes;
                    var reports = _allData.Where(x => string.IsNullOrEmpty(x.Remark)).ToList();
                    if ((runningTime >= _timeAutoReport && reports.Count > 0) || reports.Count >= _countAutoReport)
                    {
                        //create csv file
                        var dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var fileName = string.Format("{0}-sort.csv", dt);
                        var filePath = ConfigurationManager.AppSettings["SortFolder"] + "\\" + fileName;
                        var fi = new FileInfo(filePath);
                        if (!fi.Directory.Exists)
                        {
                            Directory.CreateDirectory(fi.DirectoryName);
                        }
                        using (TextWriter writer = new StreamWriter(filePath))
                        {
                            var csv = new CsvWriter(writer);
                            csv.Configuration.RegisterClassMap<ScannedLogMap>();
                            csv.Configuration.Delimiter = ",";
                            csv.WriteRecords(AddToReport(reports));
                        }
                        _log.Info("Operation", "Write file : " + fi.FullName);
                        //send report to host
                        //using (WebClient client = new WebClient())
                        //{
                        //    client.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                        //    client.UploadFile(_ftpHost + fileName, "STOR", fi.FullName);
                        //    _log.Info("Send file to ftp: " + fileName);
                        //}
                        //reset data
                        //fi.Delete();
                        _startTime = DateTime.Now;
                        _allData = new List<ScannedLogViewModel>();
                        _pagerModel.Load();
                        if (MappingModels != null)
                        {
                            foreach (var item in MappingModels)
                            {
                                item.Quantity = 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Can't send report to server\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _log.Info("Operation", "Can't send report to server\n" + ex.Message);
                }
                Thread.Sleep(5000);
            }
        }

        private List<ScannedLogViewModel> _allData;

        private IPagedList<ScannedLogViewModel> GetProductScanned(int pageSize, int pageIndex)
        {
            var total = _allData.Count();

            var data = _allData.OrderByDescending(s => s.Time).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return new PagedList<ScannedLogViewModel>(data, pageIndex, pageSize, total) as IPagedList<ScannedLogViewModel>;
        }

        private void DataChange(IPagedList<ScannedLogViewModel> data)
        {
            ScannedLogs = new ObservableCollection<ScannedLogViewModel>(data);
        }

        private ICollection<ScannedLogViewModel> AddToReport(ICollection<ScannedLogViewModel> scannedLogModels)
        {
            foreach (var scannedLogModel in scannedLogModels)
            {
                var barcodes = _barcodeDataService.GetSortedCodes(scannedLogModel);
                if (barcodes == null)
                    continue;
                if (barcodes.Count == 1)
                {
                    scannedLogModel.SortedCode = barcodes.FirstOrDefault()?.SortedCode;
                    continue;
                }
            }
            _scannedLogService.UpdateSendToHost(scannedLogModels.Select(s => s.Id).ToList());
            return scannedLogModels;
        }
    }
    public sealed class ScannedLogMap : ClassMap<ScannedLogViewModel>
    {
        public ScannedLogMap()
        {
            Map(m => m.SortedCode).Name("SortedCode");
            Map(m => m.Dimension).Name("Dimension");
            Map(m => m.Weight).Name("Weight");
            Map(m => m.ImportTime).Name("DateTime");
            Map(m => m.Destination).Name("Destination");
            Map(m => m.Chute).Name("SortedChute");
            Map(m => m.Time).Name("SortedTime");
            Map(m => m.Setting).Name("Setting");
            Map(m => m.Mode).Name("CheckDuplicate");
            Map(m => m.TransactionId).Name("TransactionId");

            Map(m => m.Barcode1).Ignore();
            Map(m => m.Barcode2).Ignore();
            Map(m => m.Barcode3).Ignore();
            Map(m => m.Length).Ignore();
            Map(m => m.Width).Ignore();
            Map(m => m.Height).Ignore();
            Map(m => m.SettingName).Ignore();
            Map(m => m.Remark).Ignore();
        }
    }
}