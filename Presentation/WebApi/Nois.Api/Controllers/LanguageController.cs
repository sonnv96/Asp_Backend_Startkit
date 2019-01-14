using System;
using AutoMapper;
using Nois.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using Nois.WebApi.Framework;
using System.Net;
using Nois.Api.Models.Languages;
using Nois.Api.Models.LocalStringResources;
using Nois.Framework.Localization;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// Language Api
    /// </summary>
    [RoutePrefix("Languages")]
    public class LanguageController : BaseApiController
    {
        #region Languages
        /// <summary>
        /// Get list Language
        /// </summary>
        /// <remarks>
        /// When user want to get list Language, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list Language", typeof(LanguageListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var model = new LanguageListModel();

            try
            {
                //Get list Language
                var languages = _languageService.GetAll(pageIndex - 1, pageSize);

                Mapper.Map(languages, model.LanguageList);

                model.Total = languages.TotalCount;
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Get a Language detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a Language detail, use this API for getting
        /// </remarks>
        /// <param name="id">Language identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a Language detail", typeof(LanguageDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var model = new LanguageDetailModel();

            try
            {
                var Language = _languageService.GetById(id);

                if (Language == null)
                {
                    model.ErrorMessages.Add(_localizationService.GetResource("Language.does.not.exist"));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }

                Mapper.Map(Language, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Add a Language
        /// </summary>
        /// <remarks>
        /// When user want to add a Language, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a Language", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(LanguageEditModel model)
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
                //Check Language is existed?
                if (_languageService.CheckExist(s => s.Name == model.Name))
                {
                    res.ErrorMessages.Add(String.Format(_localizationService.GetResource("Language.Existing"), model.Name));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Create Language
                var language = Mapper.Map<LanguageEditModel, Language>(model);

                //Save to database
                _languageService.Insert(language);

            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update a Language
        /// </summary>
        /// <remarks>
        /// When user want to update a Language, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of update a Language", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(LanguageEditModel model)
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
                //Get Language by identity
                var Language = _languageService.GetById(model.Id);
                if (Language == null)
                {
                    res.ErrorMessages.Add("Language.does.not.exist");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                //Check Language is existed?
                if (_languageService.CheckExist(s => s.Name == model.Name, Language.Id))
                {
                    res.ErrorMessages.Add(String.Format(_localizationService.GetResource("Language.Existing"), model.Name));
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                //Update Language
                Mapper.Map(model, Language);

                //update
                _languageService.Update(Language);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete a Language
        /// </summary>
        /// <remarks>
        /// When user want to delete a Language, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">Language identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a Language", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get Language by identity
                var Language = _languageService.GetById(id);
                if (Language == null)
                {
                    res.ErrorMessages.Add("Language.does.not.exist");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
                _languageService.Delete(Language);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
            return new HttpApiActionResult(HttpStatusCode.OK, res);
        }

        #endregion
        #region Local resource string
        /// <summary>
        /// Get list LocalStringResource
        /// </summary>
        /// <remarks>
        /// When user want to get list LocalStringResource, use this API for getting
        /// </remarks>
        /// <param name="id">Language id</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="textSearch">text search</param>
        /// <param name="orderDescending">order by descending</param>
        /// <returns></returns>
        [Route("{id}/localstringresources")]
        [SwaggerResponse(200, "Returns the result of get list LocalStringResource", typeof(LocaleStringResourceListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult LocalStringResourceList(int id, int pageIndex = 1, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false)
        {
            var res = new LocaleStringResourceListModel();

            try
            {
                //Get list LocalStringResource
                var localStringResources = _localizationService.Search(id, pageIndex - 1, pageSize, textSearch, sortField, orderDescending);

                //map data
                Mapper.Map(localStringResources, res.LocalStringResourceList);

                res.Total = localStringResources.TotalCount;

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }
        #endregion
    }
}
