using System;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Content Management System");

            Console.WriteLine("Choose a site to manage");

            List<string> sites = new List<string>
            {
                "Site 1"
                , "Site 2"
                , "Site 3"
            };
            int count = 0;
            sites.ForEach(s => Console.WriteLine($"{++count}. {s}"));
            
            var selection = Console.ReadLine();



            if (!string.IsNullOrEmpty(selection))
            {
                Console.WriteLine(selection);

                var match = sites.FirstOrDefault(s => s.Equals(selection, StringComparison.InvariantCultureIgnoreCase));

                if (match != null)
                {
                    Console.WriteLine($"MATCHED: {match}");
                }
                else
                {
                    Console.WriteLine("No match found");
                }
            }
        }
    }
}