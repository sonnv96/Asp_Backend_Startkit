using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using Nois.Api.Controllers;
using AutoMapper;
using System;
using Nois.Helper;
using System.Linq;
using Nois.Api.Models.Users;
using Nois.Core.Domain.Users;
using Nois.WebApi.Framework;
using System.Net;
using Nois.Core;
using System.Web;
using Nois.Framework.Loggings;

namespace Nois.Api.Modules.Users.Controllers
{
    /// <summary>
    /// User Api
    /// </summary>
    [RoutePrefix("users")]
    public class UserController : BaseApiController
    {
        #region public method

        /// <summary>
        /// Get list user 
        /// </summary>
        /// <remarks>
        /// When user want to get list user, use this API for getting
        /// </remarks>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="textSearch">text search</param>
        /// <param name="orderDescending">order by descending</param>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list user", typeof(UserListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int pageIndex = 1, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false)
        {
            var res = new UserListModel();

            try
            {
                //Get list user
                var entities = _userService.Search(pageIndex - 1, pageSize, textSearch, sortField, orderDescending, includeCurrentUser: false);

                //Only get roles active
                foreach (var user in entities)
                    user.UserRoles = user.UserRoles.Where(x => x.IsActive && !x.Deleted).ToList();

                //map data for model
                res.Total = entities.TotalCount;
                Mapper.Map(entities, res.Users);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Add a user
        /// </summary>
        /// <remarks>
        /// When user want to add a user, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a user", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(UserAddModel model)
        {
            var res = new ApiJsonResult();

            try
            {
                model.Email = model.Email.ToLower().Trim();

                //Check is valid
                if (!ModelState.IsValid)
                {
                    res.ErrorMessages.AddRange(ModelState.ToListErrorMessage());
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Check exist username
                var isExist = _userService.CheckExist(s => s.Username == model.Username);
                if (isExist)
                {
                    res.ErrorMessages.Add("User.UsernameExist");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Check exist email in used by another
                if (!string.IsNullOrEmpty(model.Email) && _userService.CheckExist(s => s.Email == model.Email))
                {
                    res.ErrorMessages.Add("User.EmailInUsedByAnother");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Create entity
                var entity = Mapper.Map<UserAddModel, User>(model);

                //Add user role
                if (model.UserRoles != null && model.UserRoles.Any())
                {
                    //get list user role identity
                    var ids = model.UserRoles.Select(x => x.Id).ToList();
                    //Get list user role from database
                    var userRoles = _userRoleService.GetByIds(ids);

                    // Check valid user roles input
                    if (userRoles.Count() != ids.Count)
                    {
                        res.ErrorMessages.Add("User.InvalidRoles");
                        return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                    }

                    var rolesInactive = userRoles.Where(x => !x.IsActive || x.Deleted);
                    if (rolesInactive.Any())
                    {
                        res.ErrorMessages.Add(string.Join(",", rolesInactive.Select(x => x.Name)));
                        res.ErrorMessages.Add("User.InactiveOrDeletedRoles");
                        return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                    }

                    // set roles for user
                    Mapper.Map(userRoles, entity.UserRoles);
                }

                //update fields cannot be map
                entity.UserGuid = Guid.NewGuid();

                //password stalf
                entity.PasswordSalt = Constant.PasswordSalt;
                //hash password
                var pwd = _encryptionService.CreatePasswordHash(Constant.DefaultPassword, entity.PasswordSalt);
                entity.Password = pwd;

                //insert new user to db
                _userService.Insert(entity);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Get a user detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a user detail, use this API for getting
        /// </remarks>
        /// <param name="id">user identity</param>
        /// <returns></returns>
        [Route("detail/{id}")]
        [SwaggerResponse(200, "Returns the result of get a user detail", typeof(UserDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var res = new UserDetailModel();
            try
            {
                //get user by identity
                var user = _userService.GetById(id);

                if (user == null)
                {
                    //not create before
                    res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                user.UserRoles = user.UserRoles.Where(x => x.IsActive && !x.Deleted).ToList();

                //map data
                Mapper.Map(user, res);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <remarks>
        /// When user want to update a user, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("detail/{id}")]
        [SwaggerResponse(200, "Returns the result of update a user", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult UpdateByDetail(int id, UserEditModel model)
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

                //Check exist in used by another
                if (!string.IsNullOrEmpty(model.Email) && _userService.CheckExist(x => x.Id != id && x.Email == model.Email))
                {
                    res.ErrorMessages.Add("User.EmailInUsedByAnother");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Get user by identity
                var entity = _userService.GetById(id);
                if (entity == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Update entity
                Mapper.Map(model, entity);

                if (model.UserRoles == null || !model.UserRoles.Any())
                {
                    //Remove current roles
                    entity.UserRoles.Clear();
                }
                else
                {
                    // Get new add role from model
                    var idRolesAdd = model.UserRoles.Where(x => entity.UserRoles.All(y => y.Id != x.Id)).Select(x => x.Id).ToList();
                    //Get list user role from db
                    var userRoles = _userRoleService.GetByIds(idRolesAdd);
                    if (userRoles.Count() != idRolesAdd.Count)
                    {
                        res.ErrorMessages.Add("User.InvalidRoles");
                        return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                    }

                    // get roles inActive or deleted
                    var invalidRoles = userRoles.Where(x => !x.IsActive || x.Deleted).ToList();
                    if (invalidRoles.Any())
                    {
                        res.ErrorMessages.Add(string.Join(",", invalidRoles.Select(x => x.Name)));
                        res.ErrorMessages.Add("User.InactiveOrDeletedRoles");
                        return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                    }

                    var idRolesRemove = entity.UserRoles.Where(x => model.UserRoles.All(y => y.Id != x.Id)).Select(x => x.Id);
                    userRoles.AddRange(entity.UserRoles.Where(x => !x.IsActive || x.Deleted || !idRolesRemove.Contains(x.Id)));

                    //remove current roles
                    entity.UserRoles.Clear();

                    //set new roles for user
                    Mapper.Map(userRoles, entity.UserRoles);
                }

                //update
                _userService.Update(entity);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Get a user detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a user profile, use this API for getting
        /// </remarks>
        /// <param name="id">user identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a user profile", typeof(UserProfileModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Profile(int id)
        {
            var res = new UserProfileModel();
            try
            {
                // check permission
                if (_workContext.CurrentUserId != id)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.Not.Permission",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //get user by identity
                var user = _userService.GetById(id);

                if (user == null)
                {
                    //not create before
                    res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //map data
                Mapper.Map(user, res);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Update a user profile
        /// </summary>
        /// <remarks>
        /// When user want to update a profile, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of update a user", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(int id, UserProfileEditModel model)
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

                // prevent insecure direct object references
                if (_workContext.CurrentUserId != id)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.Not.Permission",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Get user by identity
                var entity = _userService.GetById(id);
                if (entity == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //mapping model to user entity
                Mapper.Map(model, entity);

                //update
                _userService.Update(entity);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <remarks>
        /// When user want to delete a user, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">user identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a user", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get user by identity
                var entityById = _userService.GetById(id);
                if (entityById == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //delete
                _userService.Delete(entityById);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Get list permission by user identity
        /// </summary>
        /// <remarks>
        /// When user want to get list permission by user identity, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("{id}/permissions")]
        [SwaggerResponse(200, "Returns the result of get list permission by user identity", typeof(PermissionListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult ListByUserId(int id)
        {
            //Permission List Model
            var res = new PermissionListModel();

            try
            {
                //get user by identity
                var user = _userService.GetById(id);
                if (user == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Get list Permission
                var entities = user.UserRoles.SelectMany(ur => ur.Permissions).Distinct().ToList();

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
        /// change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("changepassword")]
        [SwaggerResponse(200, "Returns the result of change password", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult ChangePassword(ChangePasswordModel model)
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
                var user = _userService.GetById(_workContext.CurrentUserId);
                //Verify current password
                if (_encryptionService.CreatePasswordHash(model.CurrentPassword, user.PasswordSalt) != user.Password)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.CurrentPasswordNotMatch",0));
                    return new HttpApiActionResult(HttpStatusCode.NotFound, res);
                }
                user.Password = _encryptionService.CreatePasswordHash(model.NewPassword, user.PasswordSalt);
                _userService.Update(user);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("resetpassword")]
        [SwaggerResponse(200, "Returns the result of reset password", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult ResetPassword(ResetPasswordModel model)
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

                var user = _userService.GetById(model.Id);
                //check user has existed
                if (user == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("User.NotFound",0));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //reset password
                user.Password = _encryptionService.CreatePasswordHash(Constant.DefaultPassword, user.PasswordSalt);
                _userService.Update(user);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Import users
        /// </summary>
        /// <returns></returns>
        [Route("import")]
        [SwaggerResponse(200, "Returns the result of delete a Product", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Import()
        {
            var res = new ApiJsonResult();

            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    var file = httpRequest.Files[0];

                    _userService.ImportUser(file.InputStream, file.FileName);

                    res.Messages.Add("User.ImportSuccess");
                    return new HttpApiActionResult(HttpStatusCode.OK, res);
                }
                else
                {
                    res.ErrorMessages.Add("User.NoFileChoosen");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
            }
            catch (Exception ex)
            {
                _logService.Error(ex.Message);
                res.ErrorMessages.Add("User.ErrorImport");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        #endregion
    }
}