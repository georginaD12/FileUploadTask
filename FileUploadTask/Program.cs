using Microsoft.Graph;

using System;

using System.Threading.Tasks;
using Microsoft.Graph.Models;
using System.Collections.Generic;


namespace FileUploadTask
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var graphClient = AuthenticationService.GetGraphServiceClient();

            var me = await graphClient.Me.GetAsync();
            Console.WriteLine($"Signed in as: {me!.DisplayName}");

            DriveItem folder = await FileManagementService.CreateOrGetFolder(graphClient, "Myname");

         }

    }
}
