﻿using Contracts.UI.Order.Returning;
using Contracts.UI.Watch;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Asa02_SalesOrdersModule.Controllers
{
    [ApiController, Route("/api/[controller]/[action]")]
    public class ReturningController : Controller
    {
        private readonly IReturningService _returningService;

        public ReturningController(IReturningService returningService)
        {
            _returningService = returningService;
        }

        [HttpGet]
        [Route("{userId:int}")]
        public IActionResult ReturnInvoices([FromRoute] int userId)
        {
            var invoices = _returningService.ReturnInvoices(userId);
            return Ok(invoices);
        }
        
        [HttpGet]
        [Route("{invoiceId:long}")]
        public async Task<IActionResult> ReturnedInvoiceItems([FromRoute] long invoiceId)
        {
            var items = await _returningService.ReturnedInvoiceItems(invoiceId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Return(ReturningRequestDto returningRequestDto)
        {
            await _returningService.Return(returningRequestDto);
            return Ok();
        }
    }
}