using Nois.Core.Domain.ScannedLogs;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System;
using System.Linq;
using Nois.WpfApp.Framework.Logger;
using Nois.Framework.Infrastructure;
using Nois.Services.ScannedLogs;
using Nois.Services.SortedCode;
using Nois.Framework.Settings;
using Nois.Framework.Localization;
using Nois.WpfApp.Framework;
using Nois.Core.Domain.Dimensions;

namespace Nois.App.Operation.Controllers
{
    public class OperationController : SimpleCommand
    {
        private static readonly INoisLogger _log = NoisLogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));
        public override void Execute(INotification notification)
        {
            try
            {
                var _scannedLogService = EngineContext.Instance.Resolve<IScannedLogService>();
                var _barcodeDataService = EngineContext.Instance.Resolve<ISortedCodeDataService>();
                var _localStringResourceService = EngineContext.Instance.Resolve<ILocalizationService>();

                var message = (string)notification.Body;
                //split message
                var data = message.Split(new[] { "//" }, StringSplitOptions.None);

                //check message is valid
                if (data.Count() != 4)
                {
                    _log.Info("Operation", "Abort: message is invalid");
                    //reject
                    return;
                }

                #region Convert data
                //index
                var index = data[0];
                _log.Info("Operation", "Index: " + index);

                //barcode
                var barcodeStrings = data[1].Replace("-", "").Split(';').ToList();
                if (barcodeStrings.Count != 3)
                {
                    _log.Info("Operation", "Abort: Barcode count invalid (valid = 3). Count = " + barcodeStrings.Count);
                    return;
                }

                _log.Info("Operation", string.Format("Barcode list: {0}, {1}, {2}", barcodeStrings[0], barcodeStrings[1], barcodeStrings[2]));

                //dimension
                var dimension = data[2].Split(';').Select(x => int.TryParse(x, out int n) ? n : 0).ToList();
                if (dimension.Count != 3)
                {
                    _log.Info("Operation", "Abort: Dimension invalid.");
                    return;
                }
                _log.Info("Operation", string.Format("Dimension: {0} x {1} x {2}", dimension[0], dimension[1], dimension[2]));

                //weight
                int.TryParse(data[3].Substring(0, 9), out int weight);
                _log.Info("Operation", "Weight: " + weight);
                #endregion

                //create scanned log
                var scannedLog = new ScannedLog
                {
                    Barcode1 = barcodeStrings[0],
                    Barcode2 = barcodeStrings[1],
                    Barcode3 = barcodeStrings[2],
                    Weight = weight,
                    Length = dimension[0],
                    Width = dimension[1],
                    Height = dimension[2],
                    Time = DateTime.Now
                };

                #region Check data
                //1. check barcode amount
                if (barcodeStrings.Where(x => !x.Contains("?")).Distinct().Count() > 3)
                {
                    _log.Info("Operation", "Reject: More than 3 different barcode. Count = " + barcodeStrings.Distinct().Count());
                    //reject
                    scannedLog.Remark = _localStringResourceService.GetResource("REMARK.MORE THAN 3 DIFFERENT BARCODE");
                    SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index);
                    return;
                }

                //2. check valid barcode
                var barcodes = _barcodeDataService.GetSortedCodes(barcodeStrings.Where(x => !x.Contains("?")).Distinct().ToList());
                var validBarcodes = barcodes.Where(x => x != null).Distinct().ToList();
                if (validBarcodes.Count == 0)
                {
                    _log.Info("Operation", "Reject: No barcode found. Count = 0");
                    //reject
                    scannedLog.Remark = _localStringResourceService.GetResource("REMARK.NO BARCODE FOUND");
                    SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index);
                    return;
                }

                //3. check barcode data amount
                if (validBarcodes.Count > 1)
                {
                    _log.Info("Operation", "Reject: Barcodes not belong to the same package. Count = " + validBarcodes.Count);
                    //reject
                    scannedLog.Remark = _localStringResourceService.GetResource("REMARK.BARCODES NOT BELONG TO THE SAME PACKAGE");
                    SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index);
                    return;
                }
                else
                {
                    //update
                    scannedLog.Setting = validBarcodes.FirstOrDefault().Setting;
                    scannedLog.SettingName = validBarcodes.FirstOrDefault().SettingName;
                    scannedLog.Mode = validBarcodes.FirstOrDefault().Mode;
                    scannedLog.Destination = validBarcodes.FirstOrDefault().Destination.Name;
                    scannedLog.ImportTime = validBarcodes.FirstOrDefault().ImportTime;
                    scannedLog.TransactionId = validBarcodes.FirstOrDefault().TransactionId;
                    _log.Info("Operation", "Scanned log info: setting = " + scannedLog.Setting + " " +
                                                "setting name = " + scannedLog.SettingName + " " +
                                                "mode = " + scannedLog.Mode + " " +
                                                "destination = " + scannedLog.Destination + " " +
                                                "import time = " + scannedLog.ImportTime + " " +
                                                "transaction id = " + scannedLog.TransactionId);
                }

                //4. check weight dimension
                if (dimension[0] == 0 || dimension[1] == 0 || dimension[2] == 0)
                {
                    _log.Info("Operation", "Reject: Dimension is not scanned. Dimension = " + dimension[0] + " x " + dimension[1] + " x " + dimension[2]);
                    //reject
                    scannedLog.Remark = _localStringResourceService.GetResource("REMARK.DIMENSION IS NOT SCANNED");
                    SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index);
                    return;
                }

                if (weight == 0)
                {
                    _log.Info("Operation", "Reject: Weight is not scanned. Weight = 0");
                    //reject
                    scannedLog.Remark = _localStringResourceService.GetResource("REMARK.WEIGHT IS NOT SCANNED");
                    SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index);
                    return;
                }

                var dim = GetDimension();
                if (dimension[0] > dim.Length || dimension[1] > dim.Width || dimension[2] > dim.Height)
                {
                    _log.Info("Operation", "Reject: Dimension is over the max values. Dimension = " + dimension[0] + " x " + dimension[1] + " x " + dimension[2]);
                    //reject
                    scannedLog.Remark = _localStringResourceService.GetResource("REMARK.DIMENSION IS OVER THE MAX VALUES");
                    SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index);
                    return;
                }

                if (weight > dim.Weight)
                {
                    _log.Info("Operation", "Reject: Weight is over the max values. Weight = " + weight);
                    //reject
                    scannedLog.Remark = _localStringResourceService.GetResource("REMARK.WEIGHT IS OVER THE MAX VALUES");
                    SendNotification(SendCommand.App_Operation_ScanFail, scannedLog, index);
                    return;
                }
                #endregion

                SendNotification(SendCommand.App_Operation_ValidBarcodes, validBarcodes);
                scannedLog.Remark = "";
                SendNotification(SendCommand.App_Operation_ScanSuccess, scannedLog, index);
            }
            catch (Exception ex)
            {
                _log.Info("Operation", "Operation controller error: " + ex.Message);
            }
        }

        public Dimension GetDimension()
        {
            var _settingService = EngineContext.Instance.Resolve<ISettingService>();
            int.TryParse(_settingService.GetSetting("Weight").Value, out int weight);
            int.TryParse(_settingService.GetSetting("Length").Value, out int length);
            int.TryParse(_settingService.GetSetting("Width").Value, out int width);
            int.TryParse(_settingService.GetSetting("Height").Value, out int height);

            var dimension = new Dimension
            {
                Weight = weight,
                Length = length,
                Width = width,
                Height = height
            };

            return dimension;
        }
    }
}
