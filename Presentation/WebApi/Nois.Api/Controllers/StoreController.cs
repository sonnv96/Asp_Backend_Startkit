using System;
using AutoMapper;
using Nois.WebApi.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using System.Net;
using Nois.Api.Models.Stores;
using Nois.Core.Domain;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// Store Api
    /// </summary>
    [RoutePrefix("stores")]
    public class StoreController : BaseApiController
    {
        /// <summary>
        /// Get list Store
        /// </summary>
        /// <remarks>
        /// When user want to get list Store, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list Store", typeof(StoreListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false)
        {
            var model = new StoreListModel();

            try
            {
                //Get list Store
                var stores = _storeService.GetProductListPaging(pageIndex, pageSize, textSearch, sortField, orderDescending);
                if (stores == null)
                {
                    //not create before
                    model.ErrorMessages.Add("Store.NotFound");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }

                Mapper.Map(stores, model.StoreList);


                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }

        /// <summary>
        /// Get a Store detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a Store detail, use this API for getting
        /// </remarks>
        /// <param name="id">Store identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a Store detail", typeof(StoreDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var store = _storeService.GetById(id);

            var model = new StoreDetailModel();
            if (store == null)
            {
                model.ErrorMessages.Add("Store.NotFound");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }

            Mapper.Map(store, model);

            model.Messages = new List<string> { "Successful!" };

            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Add a Store
        /// </summary>
        /// <remarks>
        /// When user want to add a Store, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a Store", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(StoreEditModel model)
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

                //Create Store
                var store = Mapper.Map<StoreEditModel, Store>(model);

                //Save to database
                _storeService.Insert(store);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Update a Store
        /// </summary>
        /// <remarks>
        /// When user want to update a Store, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of update a Store", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(StoreEditModel model)
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
                //Get Store by identity
                var store = _storeService.GetById(model.Id);
                if (store == null)
                {
                    res.ErrorMessages.Add("Store.NotFound.");
                    return Json(res);
                }

                //Update Store
                Mapper.Map(model, store);

                //update
                _storeService.Update(store);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a Store
        /// </summary>
        /// <remarks>
        /// When user want to delete a Store, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">Store identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a Store", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get Store by identity
                var store = _storeService.GetById(id);
                if (store == null)
                {
                    res.ErrorMessages.Add("Store.NotFound.");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                _storeService.Delete(store);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add("Store.CannotDelete");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }
    }
}
