using System.Linq;
using Involved.HTF.Common;
using Involved.HTF.Common.Dto;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create an instance of the client
        HackTheFutureClient hackTheFutureClient = new HackTheFutureClient();

        // Log in with provided credentials
        await hackTheFutureClient.Login("MechaCode", "f0aefee1-e656-4077-91b9-bbc2d25d276d");

        // Get the sample code
        var battleData = await hackTheFutureClient.GetSample();

        // Process the battle
        var result = ProcessBattle(battleData);

        // Post the result
        await hackTheFutureClient.PostResult(result.winningTeam, result.remainingHealth);
        Console.WriteLine("Result successfully sent!");
    }

    private static (string winningTeam, int remainingHealth) ProcessBattle(BattleOfNovaCentauriDto battleData)
    {
        var teamA = battleData.TeamA.ToList(); // Convert to list for easier removal
        var teamB = battleData.TeamB.ToList(); // Convert to list for easier removal

        // Battle loop continues until one team is fully defeated
        while (teamA.Any(a => a.Health > 0) && teamB.Any(b => b.Health > 0))
        {
            // Get the first living alien from both teams
            var alienA = teamA.FirstOrDefault(a => a.Health > 0);
            var alienB = teamB.FirstOrDefault(b => b.Health > 0);

            if (alienA == null || alienB == null) break; // If any team is fully defeated, break the loop

            // Determine who attacks first based on speed
            if (alienA.Speed > alienB.Speed)
            {
                Attack(alienA, alienB);
                if (alienB.Health > 0) Attack(alienB, alienA);
            }
            else if (alienB.Speed > alienA.Speed)
            {
                Attack(alienB, alienA);
                if (alienA.Health > 0) Attack(alienA, alienB);
            }
            else // Same speed: team A attacks first
            {
                Attack(alienA, alienB);
                if (alienB.Health > 0) Attack(alienB, alienA);
            }

            // Remove defeated aliens
            teamA.RemoveAll(a => a.Health <= 0);
            teamB.RemoveAll(b => b.Health <= 0);
        }

        // Calculate the total remaining health for each team
        var remainingHealthA = teamA.Sum(a => a.Health);
        var remainingHealthB = teamB.Sum(b => b.Health);

        // Return the winner and the total remaining health
        return remainingHealthA > remainingHealthB ? ("TeamA", remainingHealthA) : ("TeamB", remainingHealthB);
    }

    private static void Attack(Alien attacker, Alien defender)
    {
        // The attacker reduces the defender's health
        defender.Health -= attacker.Strength;
    }
}
