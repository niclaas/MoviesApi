using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieApi.Nic.Claassen.Entities;
using MovieApi.Nic.Claassen.Repos;

namespace MovieApi.Nic.Claassen.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MetadataController : ControllerBase
	{
		private readonly IMoviesRepo _moviesRepo;

		public MetadataController(IMoviesRepo moviesRepo)
		{
			_moviesRepo = moviesRepo;
		}

		[HttpPost]
		[ProducesResponseType(201, Type = typeof(Metadata))]
		public IActionResult Post(Metadata model)
		{
			var result = _moviesRepo.Create(model);
			return CreatedAtAction(nameof(Get), new { MovieId = model.MovieId }, result);
		}


		[HttpGet]
		[ProducesResponseType(200, Type = typeof(ICollection<Metadata>))]
		[ProducesResponseType(404)]
		[Route("{movieId}")]
		public async Task<IActionResult> Get(int movieId)
		{
			var metadata = await _moviesRepo.Get(movieId);
			
			if (metadata == null || metadata.Count() == 0)
			{
				return NotFound();
			}

			return Ok(metadata);
		}
	}
}
