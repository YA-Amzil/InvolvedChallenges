using System.Text.Json;
using Involved.HTF.Common;

public class Program
{
    public static async Task Main(string[] args)
    {
        HackTheFutureClient hackTheFutureClient = new HackTheFutureClient();

        // Log in met de opgegeven gegevens
        await hackTheFutureClient.Login("MechaCode", "f0aefee1-e656-4077-91b9-bbc2d25d276d");

        // Haal de commando's op via de client
        string response = await hackTheFutureClient.GetCommands();

        // JSON response parsen om de commando's te extraheren
        var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(response);
        string commands = responseData["commands"];

        // Commando's verwerken
        int depthPerMeter = 0;
        int totalDepth = 0;
        int totalDistance = 0;

        string[] commandList = commands.Split(',');

        foreach (string command in commandList)
        {
            string trimmedCommand = command.Trim();
            string[] parts = trimmedCommand.Split(' ');
            string action = parts[0];
            int value = int.Parse(parts[1]);

            switch (action)
            {
                case "Up":
                    depthPerMeter -= value;
                    break;
                case "Down":
                    depthPerMeter += value;
                    break;
                case "Forward":
                    totalDistance += value;
                    totalDepth += value * depthPerMeter;
                    break;
                default:
                    Console.WriteLine($"Onbekend commando: {action}");
                    break;
            }
        }

        int result = totalDepth * totalDistance;
        Console.WriteLine($"Resultaat (Diepte * Afstand): {result}");

        // Verzenden van het resultaat via de client
        await hackTheFutureClient.PostResult(result);
        Console.WriteLine("Resultaat succesvol verzonden!");
    }
}
