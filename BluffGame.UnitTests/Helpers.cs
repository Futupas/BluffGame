using Microsoft.Extensions.Configuration;

namespace BluffGame.UnitTests;

public class Helpers
{
    public static IConfiguration CONFIG;

    static Helpers()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            // .AddEnvironmentVariables() 
            .Build();
        
        CONFIG = config;
    }
    
    // public static IConfiguration InitConfiguration()
    // {
    //     var config = new ConfigurationBuilder()
    //         .AddJsonFile("../BluffGame/appsettings.json")
    //         // .AddEnvironmentVariables() 
    //         .Build();
    //     return config;
    // }

}