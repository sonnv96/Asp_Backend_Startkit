//using Nois.Api.Fixing;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("fixings")]
    public class FixingController : BaseApiController
    {
        /// <summary>
        /// For NOIS staff fix data
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of this action", typeof(string))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(string secureKey)
        {
            //var model = "";
            //try
            //{
            //    var fixingService = new FixingService();
            //    model = fixingService.FixData(secureKey);

            //    if(model != "Successful!")
            //        return BadRequest(model);

            //    return Ok(model);
            //}
            //catch (Exception ex)
            //{
            //    model = ex.Message;
            //    return BadRequest(model);
            //}
            return null;
        }
    }
}