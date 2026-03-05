using System;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using System.Linq;
using System.IO;

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
            var userDriveId = drive.Id;

            string projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.FullName;
            string localFilePath = Path.Combine(AppContext.BaseDirectory, "fileForUpload.txt");
            string downloadFilePath = Path.Combine(AppContext.BaseDirectory, "DownloadedFile.txt");



            DriveItem folder = await FileManagementService.CreateOrGetFolder(graphClient, userDriveId, "Myname");

            if (folder != null)
            {
                await FileManagementService.UploadFile(graphClient, userDriveId, folder.Id!, localFilePath);

            }
            else
            {
                Console.WriteLine("The folder is null");
                return;

            }

            var downloadFileRes = await FileManagementService.DownloadFile(graphClient, userDriveId, folder.Id, "fileForUpload.txt", downloadFilePath);


            var sha1 = FileManagementService.ComputeSHA256(localFilePath);
            var sha2 = FileManagementService.ComputeSHA256(downloadFilePath);

            if (sha1.SequenceEqual(sha2))
            {
                Console.WriteLine("The hash codes are the same");
            }
            else
            {
                Console.WriteLine("The hash codes are NOT the same");
            }


        }
    }
}
