using FluentValidation;
using Fly.Flight.API.Models;
using Fly.Flight.API.Models.Requests;
using Fly.Flight.Application.Queries;
using Fly.Flight.Domain.Entities;
using Fly.Flight.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fly.Flight.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        private readonly IValidator<SearchFlightsQuery> _validator;
        public FlightController(MediatR.IMediator mediator, IValidator<SearchFlightsQuery> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Flights>>>> Search([FromQuery] SearchFlightsQuery flightSearchRequest)
        {
            var isParamValid = await _validator.ValidateAsync(flightSearchRequest);
            if (!isParamValid.IsValid)
                return BadRequest(isParamValid.Errors.Select(x=>x.ErrorMessage));
           
            var flights = await _mediator.Send(flightSearchRequest);
            return Ok(new ApiResponse<IEnumerable<Flights>>
            {
                Data = flights,
                Message = flights.Any() ? "found" : "nothing found",
                statuscode=flights.Any()? StatusCodes.Status200OK: StatusCodes.Status204NoContent
            }) ;
        }
    }
}
