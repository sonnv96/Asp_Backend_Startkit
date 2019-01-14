using System;
using AutoMapper;
using System.Collections.Generic;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using System.Net;
using Nois.Api.Models.Configuration;
using Nois.WebApi.Framework;
using Nois.Helper;
using System.Linq;
using Nois.Framework.Loggings;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// Log Api
    /// </summary>
    [RoutePrefix("logs")]
    public class LogController : BaseApiController
    {
        /// <summary>
        /// Get list Log
        /// </summary>
        /// <remarks>
        /// When user want to get list Log, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list Log", typeof(LogListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int? logLevelId = null, DateTime? createdFrom = null, DateTime? createdTo = null, string message = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var model = new LogListModel();

            try
            {
                
                //Get list Log
                var logs = _logService.GetAllLogs(createdFrom, createdTo, message, (LogLevel?)logLevelId, pageIndex-1, pageSize);
                if (logs == null)
                {
                    //not create before
                    model.ErrorMessages.Add("List Log is not found.");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }

                Mapper.Map(logs, model.LogList);
                model.Total = logs.TotalCount;

                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }

        /// <summary>
        /// Get a Log detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a Log detail, use this API for getting
        /// </remarks>
        /// <param name="id">Log identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a Log detail", typeof(LogDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var log = _logService.GetLogById(id);

            var model = new LogDetailModel();
            if (log == null)
            {
                model.ErrorMessages.Add("Can not find Log.");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }

            Mapper.Map(log, model);

            model.Messages = new List<string> { "Successful!" };

            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Delete a Log
        /// </summary>
        /// <remarks>
        /// When user want to delete a Log, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">Log identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a Log", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get Log by identity
                var log = _logService.GetLogById(id);
                if (log == null)
                {
                    res.ErrorMessages.Add("Log is not found.");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                _logService.DeleteLog(log);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add("Can not delete Log.");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Get list Log level
        /// </summary>
        /// <remarks>
        /// When user want to get list Log level, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("levels")]
        [SwaggerResponse(200, "Returns the result of get list Log level", typeof(LogLevelListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult LogLevelList()
        {
            var model = new LogLevelListModel();

            try
            {
                model.LogLevelList = LogLevel.Debug.ToDictionary().Select(d => new LogLevelModel { Id = d.Key, Name = d.Value }).ToList();

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
