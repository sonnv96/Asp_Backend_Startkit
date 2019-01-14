using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using Nois.Api.Controllers;
using AutoMapper;
using System;
using Nois.Api.Models.Users;
using Nois.WebApi.Framework;
using System.Net;

namespace Nois.Api.Modules.Users.Controllers
{
    /// <summary>
    /// Permission Api
    /// </summary>
    [RoutePrefix("permissions")]
    public class PermissionController : BaseApiController
    {
        #region Methods
        #region Permission
        /// <summary>
        /// Get list permission 
        /// </summary>
        /// <remarks>
        /// When user want to get list permission, use this API for getting
        /// </remarks>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="textSearch">text search</param>
        /// <param name="orderDescending">order by descending</param>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list permission", typeof(PermissionListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int pageIndex = 1, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false)
        {
            //Permission List Model
            var res = new PermissionListModel();

            try
            {
                //Get list Permission
                var entities = _permissionService.Search(pageIndex - 1, pageSize, textSearch, sortField, orderDescending);
                
                //map data for model
                Mapper.Map(entities, res.PermissionList);
                
                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of update permission", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(int id, EditPermissionModel model)
        {
            var res = new ApiJsonResult();
            try
            {
                //Get list Permission
                var permission = _permissionService.GetById(id);
                if (permission == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("permission.does.not.exist"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //map data for model
                Mapper.Map(model, permission);
                _permissionService.Update(permission);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(_localizationService.GetResource("update.error"));
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }
        #endregion
        #endregion
    }
}