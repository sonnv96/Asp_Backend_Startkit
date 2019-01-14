using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.Login.Views
{
    public class LoginBarModule : IModule
    {
        private readonly IRegionManager regionManager;
        public void Initialize()
        {
            regionManager.RegisterViewWithRegion("MainLoginBar", () =>
            {
                var loginBarMediator = (LoginBarMediator)Facade.Instance.RetrieveMediator("LoginBarMediator");
                return loginBarMediator.ViewComponent;
            });
        }
        public LoginBarModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
    }
}
