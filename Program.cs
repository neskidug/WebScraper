using AngleSharp;
using AngleSharp.Common;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScraper
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            //Henter sidens indhold og putter det ind i en string.
            HttpResponseMessage response = await client.GetAsync("https://www.timeanddate.no/vaer/?continent=europe&low=c");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            
            //Bruger AngleSharp til at gøre teksten klar til at kunnes browses nemt.
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(responseBody));

            //Søger gennem dokumentet efter stedet, hvor table tr findes, hvilket er det vi skal bruge. Indholdet deles ind i nodes.
            var nodesList = document.QuerySelectorAll("table tr");
            // Den første node samling er den med formen - GetRange tager alle noder FRA index 1, dvs. vi skipper node 0 som er formen.
            var nodes = nodesList.ToList().GetRange(1, (nodesList.ToList().Count()-1)); 
            //Nodes puttes i et array.
            var childNodes = nodes.Select(node => node.ChildNodes.ToArray());
            
            //Vi laver en stringbuilder, så teksten fra nodes kan tilføjes.
            StringBuilder sb = new StringBuilder();

            //For hver node[] i childnodes og hver node i node[] tilføjes indholdet til stringbuilderen.
            foreach (var s in childNodes)
            {
                foreach (var c in s)
                {
                    sb.Append(c.Text());
                }
            }

            string weather = sb.ToString();
            //Regex benyttes til at finde stedet, hvor dag og klokkeslet findes og erstatter det med et mellemrum.
            weather = Regex.Replace(weather, "[æ ø å a-z]{3}\\s[0-9]{2}:[0-9]{2}"," ");
            //Regex benyttes til at finde og fjerne det ekstra mellemrum, som er før °C, og indsætte ; så vi har et sted at dele teksten op.
            weather = Regex.Replace(weather, "\\s°C", "°C;");
            //Regex benyttes til at finde og fjerne * og mellemrum i teksten.
            weather = Regex.Replace(weather, "[*]\\s", "");
            
            //Der laves et string array til at indeholde indholdet fra vore tidligere string, som nu bliver delt op efter hvert ;
            string[] forPrinting = weather.Split(';');
            //Arrayet sorteres alfabetisk.
            Array.Sort(forPrinting);
            //for loopen benyttes til at printe hver linje i arrayet. int i er lig 1, da der er en tom linje i starten af udskriften.
            for (int i = 1; i < forPrinting.Length; i++)
            {
                Console.WriteLine(forPrinting[i]);
            }
            Console.ReadLine();
        }

        

        
    }
}
