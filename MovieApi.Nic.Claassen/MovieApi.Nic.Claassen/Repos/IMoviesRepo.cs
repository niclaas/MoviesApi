using MovieApi.Nic.Claassen.Dto;
using MovieApi.Nic.Claassen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Nic.Claassen.Repos
{
	public interface IMoviesRepo
	{
		Metadata Create(Metadata metadata);
		Task<IEnumerable<Metadata>> Get(int id);
		Task<IEnumerable<StatDto>> GetStats();
	}
}
