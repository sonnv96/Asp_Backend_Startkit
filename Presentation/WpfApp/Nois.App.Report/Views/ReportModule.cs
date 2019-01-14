using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.Report.Views
{
    public class ReportModule : IModule
    {
        private readonly IRegionManager regionManager;
        public void Initialize()
        {
            regionManager.RegisterViewWithRegion("ReportModule", () =>
            {
                var reportMediator = (ReportMediator)Facade.Instance.RetrieveMediator("ReportMediator");
                return reportMediator.ViewComponent;
            });
        }
        public ReportModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
    }
}
