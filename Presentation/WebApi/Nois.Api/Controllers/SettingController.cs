using System;
using AutoMapper;
using Nois.Helper;
using System.Collections.Generic;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using System.Net;
using Nois.Api.Models.Configuration;
using Nois.WebApi.Framework;
using Nois.Framework.Settings;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// Setting Api
    /// </summary>
    [RoutePrefix("settings")]
    public class SettingController : BaseApiController
    {
        /// <summary>
        /// Get list Setting
        /// </summary>
        /// <remarks>
        /// When user want to get list Setting, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list Setting", typeof(SettingListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(string name = null, string value = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var model = new SettingListModel();

            try
            {
                //Get list Setting
                var settings = _settingService.GetAll(name, value, pageIndex-1, pageSize);
                if (settings == null)
                {
                    //not create before
                    model.ErrorMessages.Add("List Setting is not found.");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }

                Mapper.Map(settings, model.SettingList);

                model.Total = settings.TotalCount;

                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }

        /// <summary>
        /// Get a Setting detail by id
        /// </summary>
        /// <remarks>
        /// When user want to get a Setting detail, use this API for getting
        /// </remarks>
        /// <param name="id">Setting identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a Setting detail", typeof(SettingDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var setting = _settingService.GetById(id);

            var model = new SettingDetailModel();
            if (setting == null)
            {
                model.ErrorMessages.Add("Can not find Setting.");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }

            Mapper.Map(setting, model);

            model.Messages = new List<string> { "Successful!" };

            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Get a Setting value by key
        /// </summary>
        /// <remarks>
        /// When user want to get a Setting detail, use this API for getting
        /// </remarks>
        /// <param name="key">Setting identity</param>
        /// <returns></returns>
        [Route("key/{key}")]
        [SwaggerResponse(200, "Returns the result of get a Setting detail", typeof(SettingValueModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult GetValueByKey(string key)
        {
            var setting = _settingService.GetSettingByKey(key, string.Empty);

            var model = new SettingDetailModel();
            if (setting == null)
            {
                model.ErrorMessages.Add("Can not find Setting.");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }

            var res = new SettingValueModel
            {
                Value = setting
            };

            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Add a Setting
        /// </summary>
        /// <remarks>
        /// When user want to add a Setting, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a Setting", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(SettingEditModel model)
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

                if(_settingService.CheckExist(s=>s.Name == model.Name))
                {
                    res.ErrorMessages.Add("Setting.NameExisting");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Create Setting
                var setting = Mapper.Map<SettingEditModel, Setting>(model);

                //Save to database
                _settingService.Insert(setting);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Update a Setting
        /// </summary>
        /// <remarks>
        /// When user want to update a Setting, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of update a Setting", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(SettingEditModel model)
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
                //Get Setting by identity
                var setting = _settingService.GetById(model.Id);
                if (setting == null)
                {
                    res.ErrorMessages.Add("Setting is not found.");
                    return Json(res);
                }
                if (_settingService.CheckExist(s => s.Name == model.Name, model.Id))
                {
                    res.ErrorMessages.Add("Setting.NameExisting");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Update Setting
                Mapper.Map(model, setting);

                //update
                _settingService.Update(setting);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a Setting
        /// </summary>
        /// <remarks>
        /// When user want to delete a Setting, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">Setting identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a Setting", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get Setting by identity
                var setting = _settingService.GetById(id);
                if (setting == null)
                {
                    res.ErrorMessages.Add("Setting is not found.");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                _settingService.Delete(setting);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add("Can not delete Setting.");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }
    }
}
