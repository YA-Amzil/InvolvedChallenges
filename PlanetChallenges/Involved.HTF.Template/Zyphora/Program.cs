using System.Text.Json;
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

        // Get the sample data
        var zyphoraData = await hackTheFutureClient.GetZyphoraData();

        // Calculate the arrival time
        DateTime arrivalTime = CalculateArrivalTime(zyphoraData);

        // Format the result as required including milliseconds and 'Z' for UTC
        string result = arrivalTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        // Post the result
        await hackTheFutureClient.PostZyphoraResult(result);

        Console.WriteLine("Result successfully sent!");
    }

    private static DateTime CalculateArrivalTime(ZyphoraTheWaitingWorldDto data)
    {
        // Calculate the total travel time in minutes (to and from the planet)
        double totalTravelTimeInMinutes = (2 * data.Distance) / data.TravelSpeed;

        // Calculate the corresponding travel time span
        TimeSpan travelTime = TimeSpan.FromMinutes(totalTravelTimeInMinutes);

        // Add the travel time to the send date time to get the raw arrival time
        DateTime rawArrivalTime = data.SendDateTime.ToUniversalTime().Add(travelTime);

        // Calculate the number of full days spanned by the travel time
        int fullDays = (int)(travelTime.TotalMinutes / (data.DayLength * 60));

        // Add the number of full days to adjust for the planet's day length
        DateTime adjustedArrivalTime = rawArrivalTime.AddDays(fullDays);

        // Return the adjusted arrival time
        return adjustedArrivalTime;
    }
}

