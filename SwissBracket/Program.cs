using System;
using System.Collections.Generic;
using System.Linq;

namespace SwissBracket
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Team> teams = InitializeTeams();
            List<Match> matches = new List<Match>();

            // Lists to keep track of teams with 3 wins or 3 losses
            List<Team> winnersGroup = new List<Team>();
            List<Team> losersGroup = new List<Team>();

            // Simulate tournament rounds
            SimulateTournament(teams, winnersGroup, losersGroup);

            // Output final results and knockout stage
            OutputResults(winnersGroup, losersGroup);
            SimulateKnockoutStage(winnersGroup);

            // Wait for user input before closing
            Console.WriteLine("Press Enter to close...");
            Console.ReadLine();
        }

        static List<Team> InitializeTeams()
        {
            List<Team> teams = new List<Team>
            {
                new Team("Adidas All Stars"),
                new Team("Atletico de Madrid"),
                //new Team("Barcelona"),
                new Team("Bayern Munich"),
                //new Team("Bergamo Calcio"),
                new Team("Chelsea"),
                //new Team("Tottenham Hotspurs"),
                //new Team("Inter"),
                new Team("Italy"),
                new Team("Leicester City"),
                //new Team("Manchester City"),
                //new Team("Manchester United"),
                new Team("New England"),
                //new Team("Real Madrid"),
                //new Team("Real Sociedad"),
                new Team("Spain"),
             //---------------------------//
                // separation of teams //
             //--------------------------//
                new Team("Argentina"),
                //new Team("Arsenal"),
                //new Team("Belgium"),
                //new Team("Brazil"),
                //new Team("Borussia Dortmund"),
                //new Team("England"),
                new Team("France"),
                new Team("Germany"),
                new Team("Inter Miami"),
                //new Team("Liverpool"),
                new Team("Napoli"),
                new Team("Portugal"),
                //new Team("Paris Saint Germain"),
                new Team("RB Leipzig"),
                new Team("Soccer Aid"),
                //new Team("Villarreal CF"),
            };

            return teams;
        }

        static List<Match> InitializeMatches(List<Team> teams)
        {
            List<Match> matches = new List<Match>();

            // Pair up teams with the same record
            for (int i = 0; i < teams.Count; i += 2)
            {
                matches.Add(new Match { Team1 = teams[i], Team2 = teams[i + 1] });
            }

            return matches;
        }

        static void SimulateTournament(List<Team> teams, List<Team> winnersGroup, List<Team> losersGroup)
        {
            Random rand = new Random();
            int round = 1;

            while (teams.Count > 0)
            {
                // Pair up teams with the same record
                teams = teams.OrderBy(t => t.Wins).ThenBy(t => t.Losses).ToList();
                List<Match> matches = InitializeMatches(teams);

                // Shuffle matches for randomness
                matches = matches.OrderBy(m => rand.Next()).ToList();

                // Simulate all matches in the current round
                foreach (var match in matches)
                {
                    SimulateMatch(match, rand);
                }

                // Remove teams with 3 wins or 3 losses
                var teamsToRemove = teams.Where(t => t.Wins >= 3 || t.Losses >= 3).ToList();
                foreach (var team in teamsToRemove)
                {
                    if (team.Wins >= 3)
                        winnersGroup.Add(team);
                    else if (team.Losses >= 3)
                        losersGroup.Add(team);

                    teams.Remove(team);
                }

                round++;
            }
        }

        static void SimulateMatch(Match match, Random rand)
        {
            double winProbabilityTeam1 = match.Team1.GetAdjustedWinProbability(match.Team2);
            double number = rand.NextDouble();

            if (number < winProbabilityTeam1)
            {
                match.Winner = match.Team1;
                match.Loser = match.Team2;
                match.Team1.Wins++;
                match.Team2.Losses++;
            }
            else
            {
                match.Winner = match.Team2;
                match.Loser = match.Team1;
                match.Team2.Wins++;
                match.Team1.Losses++;
            }

            match.isMatchPlayed = true;
        }



        static void OutputResults(List<Team> winnersGroup, List<Team> losersGroup)
        {
            Console.WriteLine();
            Console.WriteLine("Final Standings:");
            Console.WriteLine("===================\n");

            var groupedLosers = losersGroup.OrderBy(t => t.Wins).GroupBy(t => t.Wins);

            Console.WriteLine();
            Console.WriteLine("Eliminated:\n---------------------------");
            foreach (var group in groupedLosers)
            {
                Console.WriteLine();

                if (group.Key == 1)
                {
                    Console.WriteLine($"{group.Key} Win");
                }
                else
                {
                    Console.WriteLine($"{group.Key} Wins");
                }

                foreach (var team in group)
                {
                    Console.WriteLine($"{team.name} - Wins: {team.Wins}, Losses: {team.Losses}\n---------------------------");
                }
            }
        }




        static void SimulateKnockoutStage(List<Team> winnersGroup)
        {
            Random rand = new Random();

            // Ensure winnersGroup has exactly 8 teams for the knockout stage
            if (winnersGroup.Count != 8)
            {
                winnersGroup = winnersGroup.OrderByDescending(t => t.Wins).ThenByDescending(t => t.Losses).Take(8).ToList();
            }

            // Quarterfinals
            Console.WriteLine("\nQuarterfinals (QF - 6 Points):\n---------------------------");
            List<Match> quarterfinals = InitializeMatches(winnersGroup);
            foreach (var match in quarterfinals)
            {
                SimulateMatch(match, rand);
                Console.WriteLine($"QF: {match.Team1.name} vs {match.Team2.name} - Loser: {match.Loser.name}\n---------------------------");
                Console.WriteLine();
            }

            List<Team> semifinalists = quarterfinals.Select(m => m.Winner).ToList();

            // Semifinals
            Console.WriteLine("\nSemifinals (SF - 9 Points):\n---------------------------");
            List<Match> semifinals = InitializeMatches(semifinalists);
            foreach (var match in semifinals)
            {
                SimulateMatch(match, rand);
                Console.WriteLine($"SF: {match.Team1.name} vs {match.Team2.name} - Loser: {match.Loser.name}\n---------------------------");
                Console.WriteLine();
            }

            List<Team> finalists = semifinals.Select(m => m.Winner).ToList();

            // Final
            Console.WriteLine("\nFinal (winner - 16 Points | Runner-Up - 12 Points):\n---------------------------");
            Match final = new Match { Team1 = finalists[0], Team2 = finalists[1] };
            SimulateMatch(final, rand);
            Console.WriteLine($"Final: {final.Team1.name} vs {final.Team2.name}\n--------------------------- \nWinner: {final.Winner.name}\n");

            // Display the runner-up
            Team runnerUp = finalists.First(team => team != final.Winner);
            Console.WriteLine($"Runner-Up: {runnerUp.name}\n");
        }


    }

    public class Team
    {
        public string name { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public Team(string name)
        {
            this.name = name;
            this.Wins = 0;
            this.Losses = 0;
        }

        public double GetAdjustedWinProbability(Team opponent)
        {
            if (name == "New England" || name == "Inter Miami")
            {
                if (opponent.name == "Inter Miami" || opponent.name == "New England")
                {
                    return 0.5;
                }
                else if (opponent.name == "Soccer Aid" || opponent.name == "Adidas All Stars")
                {
                    return 0.6;
                }
                else
                {
                    return 0.8;
                }
            }
            else if (name == "Soccer Aid" || name == "Adidas All Stars")
            {
                if (opponent.name == "New England" || opponent.name == "Inter Miami")
                {
                    return 0.4;
                }
                else if (opponent.name == "Soccer Aid" || opponent.name == "Adidas All Stars")
                {
                    return 0.5;
                }
                else
                {
                    return 0.75;
                }
            }
            else
            {
                if (opponent.name == "New England" || opponent.name == "Inter Miami")
                {
                    return 0.2;
                }
                else if (opponent.name == "Adidas All Stars" || opponent.name == "Soccer Aid")
                {
                    return 0.3;
                }
                return 0.5;
            }
        }
    }

    public class Match
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public Team Winner { get; set; }
        public Team Loser { get; set; }
        public bool isMatchPlayed { get; set; }
    }

}
