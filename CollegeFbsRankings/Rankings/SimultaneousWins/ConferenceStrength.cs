﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CollegeFbsRankings.Conferences;
using CollegeFbsRankings.Teams;

namespace CollegeFbsRankings.Rankings
{
    public static partial class SimultaneousWins
    {
        public static class ConferenceStrength
        {
            public static Ranking<FbsConferenceRankingValue> Overall(IEnumerable<FbsConference> conferences, Dictionary<Team, Data> performanceData)
            {
                return Ranking.Create(conferences.Select(conference =>
                {
                    var writer = new StringWriter();
                    writer.WriteLine(conference.Name + " Teams:");

                    var maxTeamLength = conference.Teams.Max(team => team.Name.Length);

                    var conferenceData = new Data(0, 0, 0.0, String.Empty);                    
                    foreach (var team in conference.Teams.OrderBy(t => t.Name))
                    {
                        var teamData = performanceData[team];

                        writer.WriteLine("    {0,-" + maxTeamLength + "}: Team = {1:F8} ({2,2} / {3,2}), Opponent = {4:F8}",
                            team.Name,
                            teamData.TeamValue,
                            teamData.WinTotal,
                            teamData.GameTotal,
                            teamData.OpponentValue);

                        conferenceData = Data.Combine(conferenceData, teamData);
                    }

                    var gameTotal = conferenceData.GameTotal;
                    var winTotal = conferenceData.WinTotal;
                    var teamValue = conferenceData.TeamValue;
                    var opponentValue = conferenceData.OpponentValue;
                    var performanceValue = conferenceData.PerformanceValue;

                    writer.WriteLine();
                    writer.WriteLine("Team Value    : {0:F8} ({1} / {2})", teamValue, winTotal, gameTotal);
                    writer.WriteLine("Opponent Value: {0:F8}", opponentValue);
                    writer.WriteLine("Performance   : {0:F8}", performanceValue);

                    return new FbsConferenceRankingValue(conference,
                        new[]
                        {
                            performanceValue,
                            teamValue,
                            opponentValue
                        },
                        new IComparable[]
                        {
                            conference.Name
                        },
                        writer.ToString());
                }));
            }
        }
    }
}
