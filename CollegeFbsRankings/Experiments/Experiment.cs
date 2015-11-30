﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CollegeFbsRankings.Games;
using CollegeFbsRankings.Teams;

namespace CollegeFbsRankings.Experiments
{
    public static partial class Experiment
    {
        public class Data
        {
            public readonly int GameTotal;
            public readonly int WinTotal;

            public readonly double PerformanceValue;
            public readonly double TeamValue;
            public readonly double OpponentValue;

            public readonly string Summary;

            public Data(int gameTotal, int winTotal, double performanceValue, string summary)
            {
                GameTotal = gameTotal;
                WinTotal = winTotal;

                PerformanceValue = performanceValue;
                TeamValue = (GameTotal > 0) ? (double)WinTotal / GameTotal : 0.0;
                OpponentValue = PerformanceValue - TeamValue;

                Summary = summary;
            }

            public static Dictionary<Team, Data> Overall(IEnumerable<Team> teams, int week)
            {
                return Get(teams, t => t.Games.Where(g => g.Week <= week).Completed());
            }

            public static Dictionary<Team, Data> Fbs(IEnumerable<Team> teams, int week)
            {
                return Get(teams, t => t.Games.Where(g => g.Week <= week).Completed().Fbs());
            }

            private static Dictionary<Team, Data> Get(
                IEnumerable<Team> teams,
                Func<Team, IEnumerable<ITeamCompletedGame>> teamGameFilter)
            {
                var basicData = new Dictionary<Team, BasicData>();
                foreach (var team in teams)
                {
                    var index = basicData.Count;
                    var games = teamGameFilter(team).ToList();

                    basicData.Add(team, new BasicData(index, games));
                }

                var n = basicData.Count;
                var a = new Matrix(n);
                var b = new Vector(n);

                foreach (var pair in basicData)
                {
                    var team = pair.Key;
                    var teamData = pair.Value;

                    a.Set(teamData.Index, teamData.Index, 1.0);
                    b.Set(teamData.Index, teamData.WinPercentage);
                    foreach (var game in teamData.Games.Won())
                    {
                        var opponentIndex = basicData[game.Opponent].Index;
                        var existingValue = a.Get(teamData.Index, opponentIndex);

                        a.Set(teamData.Index, opponentIndex, existingValue - (1.0 / teamData.GameTotal));
                    }
                }

                var luDecomp = a.LUDecompose();
                var x = luDecomp.LUSolve(b);

                var results = new Dictionary<Team, Data>();
                foreach (var pair in basicData)
                {
                    var team = pair.Key;
                    var teamData = pair.Value;
                    
                    var writer = new StringWriter();
                    writer.WriteLine(team.Name + " Games:");
                    
                    if (teamData.GameTotal > 0)
                    {
                        var maxOpponentLength = teamData.Games.Max(game => game.Opponent.Name.Length);
                        var maxTeamTitleLength = team.Name.Length + maxOpponentLength + 6;

                        foreach (var game in teamData.Games)
                        {
                            var opponentData = basicData[game.Opponent];
                            var opponentValue = (game.IsWin) ? x.Get(opponentData.Index) : 0.0;

                            var teamTitle = String.Format("{0} beat {1}",
                                game.WinningTeam.Name,
                                game.LosingTeam.Name);

                            writer.WriteLine("    Week {0,-2} {1,-" + maxTeamTitleLength + "} = {2,2}-{3,2} ({4,2} / {5,2}) ({6:F8})",
                                game.Week,
                                teamTitle,
                                game.WinningTeamScore,
                                game.LosingTeamScore,
                                opponentData.WinTotal,
                                opponentData.GameTotal,
                                opponentValue);
                        }
                    }
                    else
                    {
                        writer.WriteLine("    [None]");
                    }
                    var performanceValue = x.Get(teamData.Index);

                    results.Add(team, new Data(
                            teamData.GameTotal,
                            teamData.WinTotal,
                            performanceValue,
                            writer.ToString()));
                }

                return results;
            }

            private class BasicData
            {
                public readonly int Index;
                public readonly IReadOnlyList<ITeamCompletedGame> Games;

                public readonly int GameTotal;
                public readonly int WinTotal;

                public readonly double WinPercentage;

                public BasicData(int index, IReadOnlyList<ITeamCompletedGame> games)
                {
                    Index = index;
                    Games = games;

                    GameTotal = Games.Count;
                    WinTotal = Games.Won().Count();

                    if (GameTotal > 0)
                        WinPercentage = (double)WinTotal / GameTotal;
                    else
                        WinPercentage = 0.0;
                }
            }
        }
    }
}
