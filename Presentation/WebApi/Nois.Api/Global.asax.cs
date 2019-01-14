using FluentValidation.WebApi;
using Nois.Api.App_Start;
using Nois.Framework.Histories;
using Nois.WebApi.Framework.Tasks;
using System;
using System.Web.Http;
using System.Web.Routing;

namespace Nois.Api
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration);//, provider => provider.ValidatorFactory = new NoisValidatorFactory());

            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();

            HistoryManager.Initialize();

            //update action name every web start
            PermissionConfig.UpdatePermission();
        }
    }
}