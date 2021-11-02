using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Nic.Claassen.Dto
{
	public class StatDto
	{
		public int MovieId { get; set; }
		public string Title { get; set; }
		public Decimal AverageWatchDurationS { get; set; }
		public int Watches { get; set; }
		public int ReleaseYear { get; set; }
	}
}
