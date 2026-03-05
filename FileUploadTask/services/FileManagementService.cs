using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Graph.Models.ODataErrors;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace FileUploadTask
{
    class FileManagementService
    {
        public static async Task<DriveItem> CreateOrGetFolder(GraphServiceClient graphClient, string folderName)
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

        public static async Task<DriveItem> UploadFile(GraphServiceClient graphClient, string userDriveId, string folderId, string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            using var fileStream = File.OpenRead(filePath);

            try
            {
                //try to upload the file, if it already exists, override it
                var uploadedFile = await graphClient.Drives[userDriveId].Items[folderId].ItemWithPath(fileName).Content.PutAsync(fileStream);
                Console.WriteLine($"File uploaded: {uploadedFile.Name}");
                return uploadedFile;
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


        public static async Task<bool> DownloadFile(GraphServiceClient graphClient, string userDriveId, string folderId, string fileName, string downloadPath)
        {
            try
            {
                var fileContent = await graphClient.Drives[userDriveId].Items[folderId].ItemWithPath(fileName).Content.GetAsync();

                using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
                {
                    await fileContent.CopyToAsync(fileStream);
                }

                Console.WriteLine($"File downloaded to: {downloadPath}");
                return true;
            }
            catch (ODataError odataError)
            {
                Console.WriteLine($"OData Error Message: {odataError.Error?.Message}");
                Console.WriteLine($"OData Error Code: {odataError.Error?.Code}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return false;
            }
        }

        public static byte[] ComputeSHA256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var res = sha256.ComputeHash(stream);
            return res;
        }

    }
}