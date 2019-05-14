using System;
using System.Collections.Generic;

namespace UADAPI
{
    public class UserInterestData
    {
        public List<ModGenresInterests> UserInterest { get; set; } = new List<ModGenresInterests>();

        public DateTime LastSuggestionTime { get; set; }

        public List<AnimeSeriesInfo> LastSuggestion { get; set; }
    }
}
