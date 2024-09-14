﻿using Contracts.UI.SecondCart;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Asa02_SalesOrdersModule.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SecondCartController : ControllerBase
    {
        private readonly ISecondCartService _secondCardService;

        public SecondCartController(ISecondCartService secondCardService)
        {
            _secondCardService = secondCardService;
        }

        [HttpGet("{userId:int}")]
        public IEnumerable<InvoiceItem> GetItems(int userId)
        {
            return _secondCardService.GetSecondCart(userId).InvoiceItems;
        }

        [HttpPatch]
        public async Task<IActionResult> CartToSecond([FromBody] ProductToSecondCartRequestDto productToSecondCardRequestDto)
        {
            await _secondCardService.CartToSecondCart(productToSecondCardRequestDto);
            return Ok("Successful");
        }

        [HttpPatch]
        public async Task<IActionResult> SecondToCart([FromBody] ProductToSecondCartRequestDto productToSecondCardRequestDto)
        {
            await _secondCardService.SecondCartToCart(productToSecondCardRequestDto);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItem([FromBody] ProductToSecondCartRequestDto productToSecondCardRequestDto)
        {
            await _secondCardService.DeleteItemFromTheSecondCart(productToSecondCardRequestDto);
            return Ok();
        }
    }
}