using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.TcpServer.Views
{
    public class ServerListModule : IModule
    {
        private readonly IRegionManager regionManager;
        public void Initialize()
        {
            regionManager.RegisterViewWithRegion("ServerViewRegion", () =>
            {
                var serverViewMediator = (ServerListMediator)Facade.Instance.RetrieveMediator("ServerListMediator");
                return serverViewMediator.ViewComponent;
            });
        }
        public ServerListModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
    }
}
