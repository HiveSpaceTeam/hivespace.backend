using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.Paging;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using System.Net;

namespace HiveSpace.Application.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableContent)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto param)
        {
            var result = await _orderService.CreateOrder(param);
            return Ok(result);
        }


        [HttpPost("paging")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableContent)]
        public async Task<IActionResult> GetPaging([FromBody] PagingRequestDto param)
        {
            var result = await _orderService.GetPaging(param);
            return Ok(result);
        }
    }
}
