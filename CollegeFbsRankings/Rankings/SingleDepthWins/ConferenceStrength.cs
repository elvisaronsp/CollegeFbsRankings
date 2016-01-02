﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CollegeFbsRankings.Conferences;
using CollegeFbsRankings.Teams;

namespace CollegeFbsRankings.Rankings
{
    public static partial class SingleDepthWins
    {
        public static class ConferenceStrength
        {
            public static IReadOnlyList<Ranking.ConferenceValue<TTeam>> Overall<TTeam>(IEnumerable<Conference<TTeam>> conferences, Dictionary<Team, Data> performanceData) where TTeam : Team
            {
                return conferences.Select(conference =>
                {
                    var writer = new StringWriter();
                    writer.WriteLine(conference.Name + " Teams:");

                    var maxTeamLength = conference.Teams.Max(team => team.Name.Length);

                    var conferenceData = new Data(0, 0, 0, 0, String.Empty);
                    foreach (var team in conference.Teams.OrderBy(t => t.Name))
                    {
                        var teamData = performanceData[team];

                        writer.WriteLine("    {0,-" + maxTeamLength + "}: Team = {1,2} / {2,2}, Opponent = {3,2} / {4,2}",
                            team.Name,
                            teamData.WinTotal,
                            teamData.GameTotal,
                            teamData.OpponentWinTotal,
                            teamData.OpponentGameTotal);

                        conferenceData = Data.Combine(conferenceData, teamData);
                    }

                    var teamGameTotal = conferenceData.GameTotal;
                    var teamWinTotal = conferenceData.WinTotal;
                    var teamValue = conferenceData.TeamValue;
                    var opponentGameTotal = conferenceData.OpponentGameTotal;
                    var opponentWinTotal = conferenceData.OpponentWinTotal;
                    var opponentValue = conferenceData.OpponentValue;
                    var performanceValue = conferenceData.PerformanceValue;

                    writer.WriteLine();
                    writer.WriteLine("Team Wins    : {0,2} / {1,2} ({2:F8})", teamWinTotal, teamGameTotal, teamValue);
                    writer.WriteLine("Opponent Wins: {0,2} / {1,2} ({2:F8})", opponentWinTotal, opponentGameTotal, opponentValue);
                    writer.WriteLine("Performance  : {0:F8}", performanceValue);

                    return new Ranking.ConferenceValue<TTeam>(conference,
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
                }).Sorted();
            }
        }
    }
}