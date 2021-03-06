﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CollegeFbsRankings.Domain.Conferences;
using CollegeFbsRankings.Domain.Teams;

namespace CollegeFbsRankings.Domain.Rankings
{
    public static partial class SingleDepthWins
    {
        public static class ConferenceStrength
        {
            public static Ranking<ConferenceRankingValue> Overall(IReadOnlyDictionary<Conference, IReadOnlyList<Team>> teamsByConference, IReadOnlyDictionary<Team, Data> performanceData)
            {
                return Ranking.Create(teamsByConference.Select(pair =>
                {
                    var conference = pair.Key;
                    var teamsInConference = pair.Value;

                    var writer = new StringWriter();
                    writer.WriteLine(conference.Name + " Teams:");

                    var maxTeamLength = teamsInConference.Max(team => team.Name.Length);

                    var conferenceData = new Data(0, 0, 0, 0, String.Empty);
                    foreach (var team in teamsInConference.OrderBy(t => t.Name))
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

                    return new ConferenceRankingValue(conference,
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
