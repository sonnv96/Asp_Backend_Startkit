using System;
using AutoMapper;
using Nois.WebApi.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using System.Net;
using Nois.Api.Models.OrderItems;
using Nois.Core.Domain;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// OrderItem Api
    /// </summary>
    [RoutePrefix("orderitems")]
    public class OrderItemController : BaseApiController
    {
        /// <summary>
        /// Get list OrderItem
        /// </summary>
        /// <remarks>
        /// When user want to get list OrderItem, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list OrderItem", typeof(OrderItemListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List()
        {
            var model = new OrderItemListModel();

            try
            {
                //Get list OrderItem
                var orderItems = _orderItemService.GetAll();
                if (orderItems == null)
                {
                    //not create before
                    model.ErrorMessages.Add("OrderItem.NotFound");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
                }

                Mapper.Map(orderItems, model.OrderItemList);


                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }

        /// <summary>
        /// Get a OrderItem detail 
        /// </summary>
        /// <remarks>
        /// When user want to get a OrderItem detail, use this API for getting
        /// </remarks>
        /// <param name="id">OrderItem identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of get a OrderItem detail", typeof(OrderItemDetailModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            var orderItem = _orderItemService.GetById(id);

            var model = new OrderItemDetailModel();
            if (orderItem == null)
            {
                model.ErrorMessages.Add("OrderItem.NotFound");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }

            Mapper.Map(orderItem, model);

            model.Messages = new List<string> { "Successful!" };

            return new HttpApiActionResult(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Add a OrderItem
        /// </summary>
        /// <remarks>
        /// When user want to add a OrderItem, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a OrderItem", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(OrderItemEditModel model)
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

                //Create OrderItem
                var orderItem = Mapper.Map<OrderItemEditModel, OrderItem>(model);

                //Save to database
                _orderItemService.Insert(orderItem);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Update a OrderItem
        /// </summary>
        /// <remarks>
        /// When user want to update a OrderItem, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of update a OrderItem", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPut]
        public IHttpActionResult Update(OrderItemEditModel model)
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
                //Get OrderItem by identity
                var orderItem = _orderItemService.GetById(model.Id);
                if (orderItem == null)
                {
                    res.ErrorMessages.Add("OrderItem.NotFound.");
                    return Json(res);
                }

                //Update OrderItem
                Mapper.Map(model, orderItem);

                //update
                _orderItemService.Update(orderItem);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a OrderItem
        /// </summary>
        /// <remarks>
        /// When user want to delete a OrderItem, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">OrderItem identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a OrderItem", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var res = new ApiJsonResult();

            try
            {
                //Get OrderItem by identity
                var orderItem = _orderItemService.GetById(id);
                if (orderItem == null)
                {
                    res.ErrorMessages.Add("OrderItem.NotFound.");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                _orderItemService.Delete(orderItem);

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add("OrderItem.CannotDelete");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }
    }
}
