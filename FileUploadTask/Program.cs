using System;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using System.Linq;

namespace FileUploadTask
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var graphClient = AuthenticationService.GetGraphServiceClient();

            var me = await graphClient.Me.GetAsync();
            Console.WriteLine($"Signed in as: {me!.DisplayName}");


            var drive = await graphClient.Me.Drive.GetAsync();
            var userDriveId = drive!.Id!;

            DriveItem folder = await FileManagementService.CreateOrGetFolder(graphClient, userDriveId, "Myname");

            if (folder != null)
            {
                await FileManagementService.UploadFile(graphClient, userDriveId, folder.Id!, "/Users/georgina/Desktop/FileUploadTask/FileUploadTask/fileForUpload.txt");
            }

            var downloadFileRes = await FileManagementService.DownloadFile(graphClient, userDriveId, folder.Id, "fileForUpload.txt", "/Users/georgina/Downloads/fileForUpload.txt");

            var sha1 = FileManagementService.ComputeSHA256("/Users/georgina/Desktop/FileUploadTask/FileUploadTask/fileForUpload.txt");
            var sha2 = FileManagementService.ComputeSHA256("/Users/georgina/Downloads/fileForUpload.txt");

            if (sha1.SequenceEqual(sha2))
            {
                Console.WriteLine("The hash codes are the same");
            }
        }
    }
}
