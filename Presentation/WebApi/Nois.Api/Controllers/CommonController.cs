using System;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using Nois.Api.Models.Common;
using Nois.WebApi.Framework;
using System.Net;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// Common Api
    /// </summary>
    [RoutePrefix("commons")]
    public class CommonController : BaseApiController
    {
        /// <summary>
        /// get action name summary
        /// </summary>
        /// <returns></returns>
        [Route("actionnames")]
        [SwaggerResponse(200, "Returns the result of get list action name", typeof(ActionNameListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult GetActionNames()
        {
            var model = new ActionNameListModel();
            try
            {
                var actionNames = _actionNameService.GetAll();
                Mapper.Map(actionNames, model.Data);
                model.Total = actionNames.Count;
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(_localizationService.GetResource("get.error"));
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }
    }
}