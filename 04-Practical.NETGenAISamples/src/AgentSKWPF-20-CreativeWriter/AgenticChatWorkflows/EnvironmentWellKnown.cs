using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentsSK_20_CreativeWriter;

public static class EnvironmentWellKnown
{
    // create a contructor to initialize the static variables
    static EnvironmentWellKnown()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<App>().Build();
        _deploymentName = config["AZURE_OPENAI_DEPLOYMENT"];
        _endpoint = config["AZURE_OPENAI_ENDPOINT"];
        _apiKey = config["AZURE_OPENAI_APIKEY"];
        _bingApiKey = config["BING_SEARCH_KEY"];
    }

    private static string? _deploymentName;
    public static string DeploymentName => _deploymentName ??= Environment.GetEnvironmentVariable("AzureOpenAI_Model");

    private static string? _endpoint;
    public static string Endpoint => _endpoint ??= Environment.GetEnvironmentVariable("AzureOpenAI_Endpoint");

    private static string? _apiKey;
    public static string ApiKey => _apiKey ??= Environment.GetEnvironmentVariable("AzureOpenAI_ApiKey");

    private static string? _bingApiKey;
    public static string BingApiKey => _bingApiKey ??= Environment.GetEnvironmentVariable("Bing_ApiKey");
}
