﻿using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Extensions;
using Warehouse.API.Services.Errors.Models;
using Warehouse.Core.Application.SiteManagement.Models;

namespace Warehouse.API.Controllers.API
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected IActionResult Result<TResult>(Result<TResult> result) {
            return result.Match(obj => Ok(obj), Problem);
        }

        protected IActionResult Result<TResult, TContract>(Result<TResult> result, Func<TResult, TContract> mapper) {
            return result.Match(obj => Ok(mapper(obj)), Problem);
        }

        protected IActionResult Paged<TResult>(IPagedEnumerable<TResult> collection, long pageSize)
        {
            return Ok(new PagedResponse<TResult>(collection, pageSize));
        }

        protected IActionResult Problem(Exception exception)
        {
            switch (exception)
            {
                case ValidationException validationException:
                {
                    return BadRequest(validationException.Errors.ToProblemDetails(Request.Path));
                }
                default:
                {
                    var codeInfo = exception?.GetHttpStatusCodeInfo();
                    return Problem(title: "An error occurred while processing your request.",
                        statusCode: (int)(codeInfo?.Code ?? HttpStatusCode.InternalServerError));
                }
            }
        }
    }
}
