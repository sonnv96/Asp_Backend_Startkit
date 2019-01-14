using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Nois.Framework;
using Nois.Framework.Infrastructure;
using Nois.WebApi.Framework;
using Nois.WebApi.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace Nois.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "*");
            cors.SupportsCredentials = true;
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            EngineContext.Initialize(new ApiEngine());

            //RegisterDependencies(config);
            ResgisterMapper();
        }
        //private static void RegisterDependencies(HttpConfiguration config)
        //{
        //    //we create new instance of ContainerBuilder
        //    var builder = new ContainerBuilder();
        //    var container = builder.Build();

        //    //register dependencies provided by other assemblies
        //    builder = new ContainerBuilder();

        //    var drTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
        //    .Where(type => !String.IsNullOrEmpty(type.Namespace))
        //    .Where(type => type.GetInterfaces().Contains(typeof(IDependencyRegistrar)));

        //    var drInstances = new List<IDependencyRegistrar>();
        //    foreach (var drType in drTypes)
        //        drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
        //    //sort
        //    drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
        //    foreach (var dependencyRegistrar in drInstances)
        //        dependencyRegistrar.Register(builder);
        //    builder.Update(container);

        //    EngineContext.Instance.ContainerManager = new ContainerManager(container);
        //    DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        //}

        private static void ResgisterMapper()
        {
            Mapper.Initialize(cfg =>
            {
                var drTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.GetInterfaces().Contains(typeof(IMapperRegistrar)));

                var drInstances = new List<IMapperRegistrar>();
                foreach (var drType in drTypes)
                    drInstances.Add((IMapperRegistrar)Activator.CreateInstance(drType));
                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var mapper in drInstances)
                    mapper.Register(cfg);
            });
        }
    }
}