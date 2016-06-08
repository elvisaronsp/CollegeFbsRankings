﻿using System.Collections.Generic;

using CollegeFbsRankings.Seasons;

namespace CollegeFbsRankings.Repositories
{
    public interface ISeasonQuery : IQuery<IEnumerable<Season>>
    {
        ISeasonQuery ByID(SeasonID id);

        ISeasonQuery ForYear(int year);

        ISeasonQuery ForYears(int minYear, int maxYear);
    }
}