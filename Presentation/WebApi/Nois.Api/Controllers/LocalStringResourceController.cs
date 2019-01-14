using System;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using Nois.Api.Models.LocalStringResources;
using Nois.WebApi.Framework;
using System.Net;
using Nois.Framework.Localization;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// LocaleStringResource Api
    /// </summary>
    [RoutePrefix("localestringresources")]
    public class LocaleStringResourceController : BaseApiController
    {
        private static readonly string ResourcePath = ConfigurationManager.AppSettings["localStringResourcePath"];
        private static readonly string ResourceExtension = ConfigurationManager.AppSettings["localStringResourceExtension"];

        /// <summary>
        /// Get a LocaleStringResource detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a LocaleStringResource detail, use this API for getting
        /// </remarks>
        /// <param name="id">LocaleStringResource identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a LocaleStringResource detail", typeof(LocaleStringResourceDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var res = new LocaleStringResourceDetailModel();
            try
            {
                //get localStringResource by identity
                var localeStringResource = _localizationService.GetById(id);
                if (localeStringResource == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("LocaleStringResource.NotFound"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //map data
                Mapper.Map(localeStringResource, res);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Add a LocaleStringResource
        /// </summary>
        /// <remarks>
        /// When user want to add a LocaleStringResource, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a LocaleStringResource", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(LocaleStringResourceEditModel model)
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
                //check language
                var language = _languageService.GetById(model.LanguageId);
                if (language == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("language.does.not.exist"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Create LocalStringResource
                var localStringResource = Mapper.Map<LocaleStringResourceEditModel, LocaleStringResource>(model);
                //Check ResourceName is existed?
                if (_localizationService.CheckExist(s => s.ResourceName == localStringResource.ResourceName && s.LanguageId == language.Id))
                {
                    res.ErrorMessages.Add(String.Format(_localizationService.GetResource("LocalStringResource.Existing"), model.ResourceName));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Save to database
                _localizationService.Insert(localStringResource);
                // rewrite local string file
                GenerateJsonFile(language);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update a LocaleStringResource
        /// </summary>
        /// <remarks>
        /// When user want to update a LocaleStringResource, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of update a LocaleStringResource", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(LocaleStringResourceEditModel model)
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
                //Get LocalStringResource by identity
                var localStringResource = _localizationService.GetById(model.Id);
                if (localStringResource == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("LocalStringResource.NotFound"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //check language
                if (localStringResource.LanguageId != model.LanguageId)
                {
                    var language = _localizationService.GetById(model.LanguageId);
                    if (language == null)
                    {
                        res.ErrorMessages.Add(_localizationService.GetResource("language.does.not.exist"));
                        return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                    }
                }
                //Update LocalStringResource
                Mapper.Map(model, localStringResource);

                //Check ResourceName is existed?
                if (_localizationService.CheckExist(s => s.ResourceName == model.ResourceName && s.LanguageId == model.LanguageId, localStringResource.Id))
                {
                    res.ErrorMessages.Add(String.Format(_localizationService.GetResource("LocalStringResource.Existing"), model.ResourceName));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //update
                _localizationService.Update(localStringResource);
                // rewrite local string file
                GenerateJsonFile(localStringResource.Language);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a LocalStringResource
        /// </summary>
        /// <remarks>
        /// When user want to delete a LocalStringResource, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">LocalStringResource identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a LocalStringResource", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get LocalStringResource by identity
                var localStringResource = _localizationService.GetById(id);
                if (localStringResource == null)
                {
                    res.ErrorMessages.Add(_localizationService.GetResource("LocalStringResource.NotFound"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                _localizationService.Delete(localStringResource);
                // rewrite local string file
                GenerateJsonFile(localStringResource.Language);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(_localizationService.GetResource("LocalStringResource.DeleteFailed"));
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        private void GenerateJsonFile(Language language)
        {
            // get all resource from db to write into a file
            var resource = language.LocaleStringResources.Select(x => new { Key = x.ResourceName, Value = x.ResourceValue }).ToList();
            //Convert list resource to Json format
            var resourceJson = "{" + String.Join(",", (resource.Select(r => String.Format("\"{0}\":\"{1}\"", r.Key, r.Value)))) + "}";

            // write into a file
            var path = CommonHelper.MapPath(ResourcePath);
            var exists = Directory.Exists(path);
            if (!exists)
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, language.LanguageCulture + ResourceExtension), resourceJson);
        }
    }
}
