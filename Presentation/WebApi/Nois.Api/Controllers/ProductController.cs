using System;
using AutoMapper;
using Nois.WebApi.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using System.Net;
using Nois.Api.Models.Products;
using Nois.Core.Domain;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// Product Api
    /// </summary>
    [RoutePrefix("products")]
    public class ProductController : BaseApiController
    {
        /// <summary>
        /// Get list Product
        /// </summary>
        /// <remarks>
        /// When user want to get list Product, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list Product", typeof(ProductListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(string textSearch = null, int? categoryId = null, DateTime? dateFrom = null, DateTime? dateTo = null, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool orderDescending = false)
        {
            var model = new ProductListModel();

            try
            {
                //Get list Product
                var products = _productService.GetProductListPaging(textSearch, categoryId, dateFrom, dateTo, pageIndex, pageSize, sortField, orderDescending);
                if (products == null)
                {
                    //not create before
                    model.ErrorMessages.Add("Product.NotFound");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }
                model.Total = products.TotalCount;
                Mapper.Map(products, model.ProductList);


                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }

        /// <summary>
        /// Get a Product detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a Product detail, use this API for getting
        /// </remarks>
        /// <param name="id">Product identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a Product detail", typeof(ProductDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var product = _productService.GetById(id);

            var model = new ProductDetailModel();
            if (product == null)
            {
                model.ErrorMessages.Add("Product.NotFound");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }

            Mapper.Map(product, model);

            model.Messages = new List<string> { "Successful!" };

            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Add a Product
        /// </summary>
        /// <remarks>
        /// When user want to add a Product, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a Product", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(ProductEditModel model)
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

                //Create Product
                var product = Mapper.Map<ProductEditModel, Product>(model);

                //Save to database
                _productService.Insert(product);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Update a Product
        /// </summary>
        /// <remarks>
        /// When user want to update a Product, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of update a Product", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(ProductEditModel model)
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
                //Get Product by identity
                var product = _productService.GetById(model.Id);
                if (product == null)
                {
                    res.ErrorMessages.Add("Product.NotFound.");
                    return Json(res);
                }

                //Update Product
                Mapper.Map(model, product);

                //update
                _productService.Update(product);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a Product
        /// </summary>
        /// <remarks>
        /// When user want to delete a Product, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">Product identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a Product", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get Product by identity
                var product = _productService.GetById(id);
                if (product == null)
                {
                    res.ErrorMessages.Add("Product.NotFound.");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                _productService.Delete(product);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add("Product.CannotDelete");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }


        /// <summary>
        /// Get list Product by store
        /// </summary>
        /// <remarks>
        /// When user want to get list Product, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("GetProductByStore")]
        [SwaggerResponse(200, "Returns the result of get list Product", typeof(ProductListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult ListProductByStore(int? storeId = null, int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false)
        {
            var model = new ProductListModel();

            try
            {
                //Get list Product
                var products = _productService.GetProductByStore(storeId, pageIndex, pageSize, textSearch, sortField, orderDescending);
                if (products == null)
                {
                    //not create before
                    model.ErrorMessages.Add("Product.NotFound");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }
                Mapper.Map(products, model.ProductList);


                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }

        /// <summary>
        /// Get list Product by store
        /// </summary>
        /// <remarks>
        /// When user want to get list Product, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("GetProductByCategory")]
        [SwaggerResponse(200, "Returns the result of get list Product", typeof(ProductListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult ListProductByCategory(int? categoryId = null, int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false)
        {
            var model = new ProductListModel();

            try
            {
                //Get list Product
                var products = _productService.GetProductByCategory(categoryId, pageIndex, pageSize, textSearch, sortField, orderDescending);
                if (products == null)
                {
                    //not create before
                    model.ErrorMessages.Add("Product.NotFound");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }
                Mapper.Map(products, model.ProductList);


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
