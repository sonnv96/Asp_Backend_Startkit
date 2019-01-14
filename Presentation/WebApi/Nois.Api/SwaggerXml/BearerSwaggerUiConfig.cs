using Nois.WebApi.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swashbuckle.Application;
using System.Reflection;

namespace Nois.Api.SwaggerXml
{
    /// <summary>
    /// 
    /// </summary>
    public class BearerSwaggerUiConfig : ISwaggerUi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void Apply(Assembly assembly, SwaggerUiConfig config)
        {
            config.InjectJavaScript(assembly, "Nois.Api.SwaggerXml.SwaggerUIEnableBearerToken.js");
        }
    }
}