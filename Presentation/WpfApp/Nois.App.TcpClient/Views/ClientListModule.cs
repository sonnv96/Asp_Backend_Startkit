using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.TcpClient.Views
{
    public class ClientListModule : IModule
    {
        private readonly IRegionManager regionManager;
        public void Initialize()
        {
            regionManager.RegisterViewWithRegion("ClientViewRegion", () =>
            {
                var clientViewMediator = (ClientListMediator)Facade.Instance.RetrieveMediator("ClientListMediator");
                return clientViewMediator.ViewComponent;
            });
        }
        public ClientListModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
    }
}
