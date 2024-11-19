using System.Text.Json;
using Involved.HTF.Common;
using Involved.HTF.Common.Dto;
using System.Net.Http.Headers;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create an instance of the client
        HackTheFutureClient hackTheFutureClient = new HackTheFutureClient();

        // Log in with provided credentials
        await hackTheFutureClient.Login("MechaCode", "f0aefee1-e656-4077-91b9-bbc2d25d276d");

        // Get the sample data
        var quatralianNumbers = await hackTheFutureClient.GetQuatralianNumbers();

        // Decode, sum, and re-encode the Quatralian numbers
        string result = ProcessQuatralianNumbers(quatralianNumbers);

        // Post the result
        await hackTheFutureClient.PostQuatralianResult(result);

        Console.WriteLine("Result successfully sent!");
    }

    private static string ProcessQuatralianNumbers(string[] quatralianNumbers)
    {
        int sum = 0;
        foreach (var number in quatralianNumbers)
        {
            sum += DecodeQuatralian(number);
        }
        return EncodeQuatralian(sum);
    }

    private static int DecodeQuatralian(string quatralianNumber)
    {
        int sum = 0;
        foreach (var part in quatralianNumber.Split(' '))
        {
            if (part == "Ⱄ") // Special case for zero
            {
                sum = sum * 10; // Shift left by one decimal position for zeros
            }
            else
            {
                sum *= 10; // Handle multi-digit cases correctly
                sum += part.Count(c => c == '.') * 1;
                sum += part.Count(c => c == '|') * 5;
            }
        }
        return sum;
    }

    private static string EncodeQuatralian(int number)
    {
        if (number == 0) return "Ⱄ";

        List<string> parts = new List<string>();
        while (number > 0)
        {
            int remainder = number % 10;
            number /= 10;

            if (remainder == 0)
            {
                parts.Insert(0, "Ⱄ");
            }
            else
            {
                int bars = remainder / 5;
                int dots = remainder % 5;
                string part = new string('|', bars) + new string('.', dots);
                parts.Insert(0, part);
            }
        }

        return string.Join(" ", parts);
    }
}
