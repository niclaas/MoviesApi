using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Nic.Claassen.Dto;
using MovieApi.Nic.Claassen.Entities;
using MovieApi.Nic.Claassen.Repos;

namespace MovieApi.Nic.Claassen.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MoviesController : ControllerBase
	{
		private readonly IMoviesRepo _moviesRepo;

		public MoviesController(IMoviesRepo moviesRepo)
		{
			_moviesRepo = moviesRepo;
		}

		[HttpGet]
		[Route("stats")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<StatDto>))]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Stats()
		{
			var stats = await _moviesRepo.GetStats();
			
			if (stats == null)
			{
				return NotFound();
			}

			return Ok(stats);
		}
	}
}
