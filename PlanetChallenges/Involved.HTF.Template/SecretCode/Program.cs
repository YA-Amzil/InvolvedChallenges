using System.Text.Json;
using Involved.HTF.Common;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create an instance of the client
        HackTheFutureClient hackTheFutureClient = new HackTheFutureClient();

        // Log in with provided credentials
        await hackTheFutureClient.Login("MechaCode", "f0aefee1-e656-4077-91b9-bbc2d25d276d");

        // Get the sample code
        string jsonResponse = await hackTheFutureClient.GetSampleCode();

        // Parse JSON response to extract commands
        var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
        if (responseData == null || !responseData.TryGetValue("alienMessage", out string commands))
        {
            Console.WriteLine("Failed to get alienMessage from responseData.");
            return;
        }

        // Retrieve the alphabet mapping
        var alphabetMapping = await hackTheFutureClient.GetAlphabetMapping();

        // Create reverse mapping dictionary
        var reverseAlphabetMapping = alphabetMapping.ToDictionary(pair => pair.Value, pair => pair.Key);

        // Decode the commands
        string result = DecodeCommands(commands, reverseAlphabetMapping);

        // Post the decoded result
        await hackTheFutureClient.PostDecodedSample(result);
        Console.WriteLine("Result successfully sent!");
    }

    private static string DecodeCommands(string commands, Dictionary<string, string> reverseAlphabetMapping)
    {
        var decodedString = new StringBuilder();
        foreach (var commandChar in commands)
        {
            string charString = commandChar.ToString();
            if (reverseAlphabetMapping.TryGetValue(charString, out string mappedValue))
            {
                decodedString.Append(mappedValue);
            }
            else
            {
                decodedString.Append(charString); // Keep the original character if not found in mapping
            }
        }
        return decodedString.ToString();
    }
}
