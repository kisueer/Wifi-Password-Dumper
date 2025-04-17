using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WifiPasswordRetriever
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wi-Fi Password Retriever");
            Console.WriteLine("------------------------");
            Console.WriteLine("Retrieving saved Wi-Fi profiles...\n");

            // Get all saved Wi-Fi profiles
            List<string> profiles = GetWifiProfiles();

            if (profiles.Count == 0)
            {
                Console.WriteLine("No saved Wi-Fi profiles found.");
                return;
            }

            Console.WriteLine($"Found {profiles.Count} saved Wi-Fi profiles.\n");

            // Retrieve passwords for each profile
            foreach (string profile in profiles)
            {
                string password = GetWifiPassword(profile);
                Console.WriteLine($"Network: {profile}");
                Console.WriteLine($"Password: {password}\n");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static List<string> GetWifiProfiles()
        {
            List<string> profiles = new List<string>();

            // Create process to run netsh command
            Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = "wlan show profiles";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Parse profiles from the output
            Regex regex = new Regex(@"All User Profile\s+:\s(.+)");
            MatchCollection matches = regex.Matches(output);

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count > 1)
                {
                    profiles.Add(match.Groups[1].Value.Trim());
                }
            }

            return profiles;
        }

        static string GetWifiPassword(string profileName)
        {
            // Create process to run netsh command for specific profile
            Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = $"wlan show profile name=\"{profileName}\" key=clear";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Parse password from the output
            Regex regex = new Regex(@"Key Content\s+:\s(.+)");
            Match match = regex.Match(output);

            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value.Trim();
            }
            else
            {
                return "Password not found or not available.";
            }
        }
    }
}