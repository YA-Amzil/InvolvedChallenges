﻿namespace Involved.HTF.Common;

using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class HackTheFutureClient : HttpClient
{
    public HackTheFutureClient()
    {
        BaseAddress = new Uri("https://app-htf-2024.azurewebsites.net/");
    }

    public async Task Login(string teamname, string password)
    {
        var response = await GetAsync($"/api/team/token?teamname={teamname}&password={password}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("You weren't able to log in, did you provide the correct credentials?");
        else
        {
            Console.WriteLine("You were able to log in, you can now get the commands.");
        }
        var token = await response.Content.ReadFromJsonAsync<AuthResponse>();
        DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
    }

    public async Task<string> GetCommands()
    {
        var response = await GetAsync("/api/a/easy/puzzle");
        if (!response.IsSuccessStatusCode)
            throw new Exception("You weren't able to get the commands, did you provide the correct credentials?");
        else
        {
            Console.WriteLine("You were able to get the commands, you can now process them.");
        }
        return await response.Content.ReadAsStringAsync();
    }


    public async Task PostResult(int result)
    {
        // Post the integer directly as JSON
        var response = await this.PostAsJsonAsync("/api/a/easy/puzzle", result);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"You weren't able to post the result, did you provide the correct credentials? Response: {response.StatusCode} - {responseContent}");
        }
        else
        {
            Console.WriteLine("You were able to post the result, you can now check the results.");
        }
    }

    public async Task<Dictionary<string, string>> GetAlphabetMapping()
    {
        var response = await GetAsync("/api/b/easy/alphabet");
        if (!response.IsSuccessStatusCode)
            throw new Exception("You weren't able to get the alphabet mapping, did you provide the correct credentials?");
        else
        {
            Console.WriteLine("You were able to get the alphabet mapping, you can now decode the message.");
        }
        return await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
    }

    public async Task<string> GetSampleCode()
    {
        var response = await GetAsync("/api/b/easy/puzzle");
        if (!response.IsSuccessStatusCode)
            throw new Exception("You weren't able to get the sample code, did you provide the correct credentials?");
        else
        {
            Console.WriteLine("You were able to get the sample code, you can now decode it.");
        }
        return await response.Content.ReadAsStringAsync();
    }

    public async Task PostDecodedSample(string decodedSample)
    {
        var response = await this.PostAsJsonAsync("/api/b/easy/puzzle", decodedSample);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"You weren't able to post the decoded sample, did you provide the correct credentials? Response: {response.StatusCode} - {responseContent}");
        }
        else
        {
            Console.WriteLine("You were able to post the decoded sample, you can now check the results.");
        }
    }

}

public class AuthResponse
{
    public string Token { get; set; }
}