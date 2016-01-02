﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CollegeFbsRankings.Games;
using CollegeFbsRankings.Teams;

namespace CollegeFbsRankings.Rankings
{
    public static partial class SingleDepthWins
    {
        public static class GameStrength
        {
            public static IReadOnlyList<Ranking.GameValue> Overall(IEnumerable<IGame> games, Dictionary<Team, Data> performanceData)
            {
                return Calculate(games, performanceData);
            }

            public static Dictionary<int, IReadOnlyList<Ranking.GameValue>> ByWeek(IEnumerable<IGame> games, Dictionary<Team, Data> performanceData)
            {
                return games.GroupBy(g => g.Week).ToDictionary(group => group.Key, group => Calculate(group, performanceData));
            }

            private static IReadOnlyList<Ranking.GameValue> Calculate(IEnumerable<IGame> games, Dictionary<Team, Data> performanceData)
            {
                return games.Select(game =>
                {
                    var writer = new StringWriter();
                    writer.WriteLine("Week {0} {1} vs. {2} ({3}):",
                        game.Week,
                        game.HomeTeam.Name,
                        game.AwayTeam.Name,
                        game.Date);

                    var maxTeamLength = Math.Max(game.HomeTeam.Name.Length, game.AwayTeam.Name.Length);

                    var homeTeamData = performanceData[game.HomeTeam];

                    writer.WriteLine("    {0,-" + maxTeamLength + "}: Team = {1,2} / {2,2}, Opponent = {3,2} / {4,2}",
                        game.HomeTeam.Name,
                        homeTeamData.WinTotal,
                        homeTeamData.GameTotal,
                        homeTeamData.OpponentWinTotal,
                        homeTeamData.OpponentGameTotal);

                    var awayTeamData = performanceData[game.AwayTeam];

                    writer.WriteLine("    {0,-" + maxTeamLength + "}: Team = {1,2} / {2,2}, Opponent = {3,2} / {4,2}",
                        game.AwayTeam.Name,
                        awayTeamData.WinTotal,
                        awayTeamData.GameTotal,
                        awayTeamData.OpponentWinTotal,
                        awayTeamData.OpponentGameTotal);

                    var gameData = Data.Combine(homeTeamData, awayTeamData);

                    var teamGameTotal = gameData.GameTotal;
                    var teamWinTotal = gameData.WinTotal;
                    var teamValue = gameData.TeamValue;
                    var opponentGameTotal = gameData.OpponentGameTotal;
                    var opponentWinTotal = gameData.OpponentWinTotal;
                    var opponentValue = gameData.OpponentValue;
                    var performanceValue = gameData.PerformanceValue;

                    writer.WriteLine();
                    writer.WriteLine("Team Wins    : {0,2} / {1,2} ({2:F8})", teamWinTotal, teamGameTotal, teamValue);
                    writer.WriteLine("Opponent Wins: {0,2} / {1,2} ({2:F8})", opponentWinTotal, opponentGameTotal, opponentValue);
                    writer.WriteLine("Performance  : {0:F8}", performanceValue);

                    return new Ranking.GameValue(game,
                        new[]
                        {
                            performanceValue,
                            teamValue,
                            opponentValue
                        },
                        new IComparable[]
                        {
                            game.Week,
                            game.HomeTeam.Name,
                            game.AwayTeam.Name,
                            game.Date
                        },
                        writer.ToString());
                }).Sorted();
            }
        }
    }
}