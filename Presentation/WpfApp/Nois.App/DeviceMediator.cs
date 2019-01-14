using log4net;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;

namespace Nois.App
{
    public class DeviceMediator : ViewModelMediator
    {
        #region variables

        #region ModbusTCP variable
        
        #endregion

        private static readonly ILog _log = LogManager.GetLogger(Type.GetType("Nois.App.App, Nois.App"));

        #endregion

        #region Utilities

        #endregion

        #region visibilities

        #endregion

        #region constructors

        public DeviceMediator() : base("DeviceMediator")
        {
        }

        #endregion

        public override void OnRegister()
        {
            base.OnRegister();
        }

        public override IList<string> ListNotificationInterests()
        {
            return new List<string>
            {

            };
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            switch (notification.Name)
            {
                
            }
        }

        
       

        public override int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
