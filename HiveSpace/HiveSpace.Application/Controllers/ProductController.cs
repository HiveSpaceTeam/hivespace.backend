﻿using Microsoft.AspNetCore.Mvc;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.Product;
using System.Net;

namespace HiveSpace.Application.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    //[Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{productId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductDetail(int productId)
        {
            var result = await _productService.GetProductDetailAsync(productId);
            return Ok(result);
        }

        [HttpPost("search")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductSearchViewModel([FromBody] ProductSearchRequestDto param)
        {
            var result = await _productService.GetProductSearchViewModelAsync(param);
            return Ok(result);
        }

        [HttpGet("home={isHome}&pageSize={pageSize}&pageIndex={pageIndex}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductHomeViewModel(bool isHome = false, int pageSize = 50, int pageIndex = 0)
        {
            var result = await _productService.GetProductHomeViewModelAsync(pageSize, pageIndex);
            return Ok(result);
        }

        [HttpGet("categoryId={categoryId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(result);
        }
    }
}
