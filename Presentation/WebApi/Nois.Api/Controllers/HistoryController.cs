using Nois.WebApi.Framework;
using Nois.Api.Models.Histories;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Web.Http;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// History Api
    /// </summary>
    [RoutePrefix("histories")]
    public class HistoryController : BaseApiController
    {
        /// <summary>
        /// Get list History
        /// </summary>
        /// <remarks>
        /// When user want to get list History, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("{entityTypeName}/{id}")]
        [SwaggerResponse(200, "Returns the result of get list History", typeof(HistoryListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int id, string entityTypeName)
        {
            var model = new HistoryListModel();

            try
            {
                model.HistoryList = _historyService.GetHistories(entityTypeName, id);
                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }
    }
}