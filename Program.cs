using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            HttpResponseMessage response = await client.GetAsync("https://www.timeanddate.no/vaer/?continent=europe&low=c");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            // Above three lines can be replaced with new helper method below
            // string responseBody = await client.GetStringAsync(uri);

            Console.WriteLine(responseBody);
            Console.ReadLine(); 

        }
    }
}
