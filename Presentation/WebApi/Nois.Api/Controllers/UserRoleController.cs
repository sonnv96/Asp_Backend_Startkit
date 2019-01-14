using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using Nois.Api.Controllers;
using AutoMapper;
using System;
using Nois.Helper;
using Nois.Api.Models.Users;
using Nois.WebApi.Framework;
using System.Net;
using System.Linq;
using Nois.Core.Domain.Users;

namespace Nois.Api.Modules.Users.Controllers
{
    /// <summary>
    /// User Role Api
    /// </summary>
    [RoutePrefix("userroles")]
    public class UserRoleController : BaseApiController
    {
        #region public methods

        /// <summary>
        /// Get list user role 
        /// </summary>
        /// <remarks>
        /// When user want to get list user role, use this API for getting
        /// </remarks>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="textSearch">text search</param>
        /// <param name="orderDescending">order by descending</param>
        /// <param name="isActive">order by descending</param>
        /// <param name="isDelete">order by descending</param>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list user role", typeof(UserRoleListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int pageIndex = 1, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false, bool? isActive = null, bool? isDelete = null)
        {
            var res = new UserRoleListModel();

            try
            {
                // get all roles from current user
                var userRoleIds = _workContext.CurrentUser.UserRoles.Select(r => r.Id).ToList();

                //Get list user role
                var entities = _userRoleService.Search(pageIndex - 1, pageSize, textSearch, sortField, orderDescending, isActive, isDelete, userRoleIds);

                //map data
                Mapper.Map(entities, res.UserRoles);

                res.Total = entities.TotalCount;

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Get a user role detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a user role detail, use this API for getting
        /// </remarks>
        /// <param name="id">user role identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a user role detail", typeof(UserRoleModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            //get list userrole by identity
            var userRole = _userRoleService.GetById(id);

            var res = new UserRoleDetailModel();

            if (userRole == null)
            {
                res.ErrorMessages.Add(_localizationService.GetResource("UserRole.NotFound"));
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }

            Mapper.Map(userRole, res);

            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Add a user role
        /// </summary>
        /// <remarks>
        /// When user want to add a user role, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a user role", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(UserRoleEditModel model)
        {
            var res = new ApiJsonResult();

            try
            {
                //Check is valid
                if (!ModelState.IsValid)
                {
                    res.ErrorMessages.AddRange(ModelState.ToListErrorMessage());
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                model.SystemName = null;
                //Check duplicate role name
                if (_userRoleService.CheckExist(x => x.Name == model.Name))
                {
                    res.ErrorMessages.Add(String.Format(_localizationService.GetResource("UserRole.NameExisting"), model.Name));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Create entity
                var entity = Mapper.Map<UserRoleEditModel, UserRole>(model);

                //Add permissions
                if (model.Permissions != null)
                {
                    //get list permission identity
                    var ids = model.Permissions.Select(x => x.Id).ToList();
                    //Get list permission
                    var permissions = _permissionService.GetByIds(ids);
                    if (permissions != null && permissions.Count() > 0)
                    {
                        //map data
                        Mapper.Map(permissions, entity.Permissions);
                    }
                }

                //Save to database
                _userRoleService.Insert(entity);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Update a user role
        /// </summary>
        /// <remarks>
        /// When user want to update a user role, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of update a user role", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(UserRoleEditModel model)
        {
            var res = new ApiJsonResult();

            try
            {
                //Check data is valid
                if (!ModelState.IsValid)
                {
                    res.ErrorMessages.AddRange(ModelState.ToListErrorMessage());
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Get user role by identity
                var entity = _userRoleService.GetById(model.Id);
                if (entity == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("UserRole.NotFound"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Update entity
                Mapper.Map(model, entity);

                //Add permissions
                if (model.Permissions != null)
                {
                    //get list permission identity
                    var ids = model.Permissions.Select(x => x.Id).ToList();
                    //Get list permission
                    var permissions = _permissionService.GetByIds(ids);
                    if (permissions != null && permissions.Count() >= 0)
                    {
                        //map data
                        Mapper.Map(permissions, entity.Permissions);
                    }
                }
                //update
                _userRoleService.Update(entity);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <remarks>
        /// When user want to delete a user role, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">user role identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a user role", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get user role by identity
                var entityById = _userRoleService.GetById(id);
                if (entityById == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("UserRole.NotFound"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //delete
                _userRoleService.Delete(entityById);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(_localizationService.GetResource("UserRole.NotDelete"));
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        #endregion
    }
}