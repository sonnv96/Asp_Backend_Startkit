using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.Operation.Views
{
    public class OperationModule : IModule
    {
        private readonly IRegionManager regionManager;
        public void Initialize()
        {
            regionManager.RegisterViewWithRegion("OperationModule", () =>
            {
                var operationMediator = (OperationMediator)Facade.Instance.RetrieveMediator("OperationMediator");
                return operationMediator.ViewComponent;
            });
        }
        public OperationModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
    }
}
