using MovieApi.Nic.Claassen.Dto;
using MovieApi.Nic.Claassen.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MovieApi.Nic.Claassen.Repos
{
	public class MoviesRepo : IMoviesRepo
	{
		private static List<Metadata> Database = new List<Metadata>();

		public Metadata Create(Metadata metadata)
		{
			Database.Add(metadata);
			return metadata;
		}

		public async Task<IEnumerable<Metadata>> Get(int id)
		{
			var metadataList = await LoadMetadata();
			return metadataList
				.Where(x => x.MovieId == id && !String.IsNullOrWhiteSpace(x.Language) && !String.IsNullOrWhiteSpace(x.Title) && !String.IsNullOrWhiteSpace(x.Duration) && x.ReleaseYear > 0)
				.GroupBy(x => x.Language)
				.Select(x => x.OrderByDescending(y => y.Id).FirstOrDefault())
				.OrderBy(x => x.Language)
				.ToList();
		}

		public async Task<IEnumerable<StatDto>> GetStats()
		{
			var metadataList = (await LoadMetadata())
				.GroupBy(x => x.MovieId)
				.Select(x => x.Where(y => y.Language == "EN").First());

			var statsList = await LoadStats();

			var stats = statsList.GroupBy(x => x.MovieId).Select(x => new
			{
				x.First().MovieId,
				AverageWatchDurationS = x.Sum(y => y.Duration) / x.Count(),
				Watches = x.Count()
			});

			return stats.Join(metadataList, s => s.MovieId, m => m.MovieId, (s, m) => new StatDto
			{
				MovieId = m.MovieId,
				Title = m.Title,
				AverageWatchDurationS = s.AverageWatchDurationS,
				Watches = s.Watches,
				ReleaseYear = m.ReleaseYear
			})
			.OrderBy(x => x.Watches)
			.ThenByDescending(x => x.ReleaseYear);
		}

		private async Task<List<Metadata>> LoadMetadata()
		{
			List<Metadata> list = new List<Metadata>();

			using (var stream = Assembly
				.GetExecutingAssembly()
				.GetManifestResourceStream("MovieApi.Nic.Claassen.metadata.csv"))
			{
				using (var reader = new StreamReader(stream))
				{
					bool first = true;
					while (!reader.EndOfStream)
					{
						string line = await reader.ReadLineAsync();
						if (!first)
						{
							var columns = ExtractColumns(line);

							list.Add(new Metadata
							{
								Id = Int32.Parse(columns.ElementAt(0)),
								MovieId = Int32.Parse(columns.ElementAt(1)),
								Title = columns.ElementAt(2),
								Language = columns.ElementAt(3),
								Duration = columns.ElementAt(4),
								ReleaseYear = Int32.Parse(columns.ElementAt(5))
							});
						}
						first = false;
					}
				}
			}

			return list;
		}

		private async Task<List<Stat>> LoadStats()
		{
			List<Stat> list = new List<Stat>();

			using (var stream = Assembly
				.GetExecutingAssembly()
				.GetManifestResourceStream("MovieApi.Nic.Claassen.stats.csv"))
			{
				using (var reader = new StreamReader(stream))
				{
					bool first = true;
					while (!reader.EndOfStream)
					{
						string line = await reader.ReadLineAsync();
						if (!first)
						{
							var columns = ExtractColumns(line);

							list.Add(new Stat
							{
								MovieId = Int32.Parse(columns.ElementAt(0)),
								Duration = Int32.Parse(columns.ElementAt(1))
							});

						}
						first = false;
					}
				}
			}

			return list;
		}

		private ICollection<string> ExtractColumns(string line)
		{
			bool insideQuotes = false;
			string buffer = string.Empty;
			List<string> columns = new List<string>();

			foreach (var c in line)
			{
				if (c == '"')
				{
					insideQuotes = !insideQuotes;
				}
				else if (c == ',' && !insideQuotes)
				{
					columns.Add(buffer);
					buffer = string.Empty;
				}
				else
				{
					buffer += c;
				}
			}

			columns.Add(buffer);

			return columns;
		}
	}
}
