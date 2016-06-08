﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CollegeFbsRankings.Conferences;
using CollegeFbsRankings.Seasons;

namespace CollegeFbsRankings.Repositories.Memory
{
    public class MemoryDivisionQuery<T> : IDivisionQuery<T> where T : Division
    {
        private readonly IEnumerable<T> _items;

        public MemoryDivisionQuery(IEnumerable<T> divisions)
        {
            _items = divisions;
        }

        public IEnumerable<T> Execute()
        {
            return _items;
        }

        public IDivisionQuery<T> ByID(DivisionID id)
        {
            return new MemoryDivisionQuery<T>(_items.Where(e => e.ID == id));
        }

        public IDivisionQuery<T> ByName(string name)
        {
            return new MemoryDivisionQuery<T>(_items.Where(e => e.Name == name));
        }

        public IDivisionQuery<T> ForSeason(SeasonID season)
        {
            return new MemoryDivisionQuery<T>(_items.Where(e => e.Teams.Any(t => t.Games.Any(g => g.Season.ID == season))));
        }

        public IDivisionQuery<T> ForConference(ConferenceID conference)
        {
            return new MemoryDivisionQuery<T>(_items.Where(e => e.Conference.ID == conference));
        }

        public IDivisionQuery<FbsDivision> Fbs()
        {
            return new MemoryDivisionQuery<FbsDivision>(_items.OfType<FbsDivision>());
        }
    }
}
