using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Graph.Models.ODataErrors;
using System.Linq;
using System.Collections.Generic;
namespace FileUploadTask
{
    class FileManagementService
    {
        public static async Task<DriveItem?> CreateOrGetFolder(GraphServiceClient graphClient, string folderName)
        {
            //SCENARIO 1: the folder already exists and we just return it
            var drive = await graphClient.Me.Drive.GetAsync();
            var userDriveId = drive.Id;

            var children = await graphClient.Drives[userDriveId].Items["root"].Children.GetAsync();

            var existingFolder = children.Value.FirstOrDefault(i => i.Name == folderName);

            if (existingFolder != null)
            {
                Console.WriteLine($"{existingFolder.Name} already exists");
                return existingFolder;
            }



            // SCENARIO 2: the folder doesn't exist, so we must create it
            var requestBody = new DriveItem
            {
                Name = folderName,
                Folder = new Folder(),
                AdditionalData = new Dictionary<string, object>
                {
                    { "@microsoft.graph.conflictBehavior", "fail" }
                }
            };

            try
            {
                var result = await graphClient.Drives[userDriveId].Items["root"].Children.PostAsync(requestBody);
                Console.WriteLine($"Folder created: {result.Name}");
                return result;
            }
            catch (ODataError odataError)
            {
                Console.WriteLine($"OData Error Message: {odataError.Error?.Message}");
                Console.WriteLine($"OData Error Code: {odataError.Error?.Code}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return null;
            }
        }
    }
}