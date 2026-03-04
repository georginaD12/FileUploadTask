using Microsoft.Graph;

using System;

using System.Threading.Tasks;


namespace Console_DeviceCodeFlow
{    internal class Program
    {

        private static async Task Main(string[] args)
        {
            // Sign-in user using MSAL and obtain an access token for MS Graph
            GraphServiceClient graphClient = await AuthenticationService.Auth();

            // Call the /me endpoint of MS Graphs
            await CallMSGraph(graphClient);
        }

        /// <summary>
        /// Call MS Graph and print results
        /// </summary>
        /// <param name="graphClient"></param>
        /// <returns></returns>
        private static async Task CallMSGraph(GraphServiceClient graphClient)
        {
            var me = await graphClient.Me.Request().GetAsync();

            // Printing the results
            Console.Write(Environment.NewLine);
            Console.WriteLine("-------- Data from call to MS Graph --------");
            Console.Write(Environment.NewLine);
            Console.WriteLine($"Id: {me.Id}");
            Console.WriteLine($"Display Name: {me.DisplayName}");
            Console.WriteLine($"Email: {me.Mail}");
        }
    }
}
