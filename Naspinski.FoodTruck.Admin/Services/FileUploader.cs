using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Naspinski.FoodTruck.AdminWeb.Services
{
    public class FileUploader
    {
        // https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/uploading-files
        public static async Task<Uri> ProcessFormFile(string azureStorageAccount, string azureStoragePassword, IFormFile file, string containerName, ModelStateDictionary modelState, string fileName = "", bool overwrite = false)
        {
            if (file.Length == 0)
            {
                modelState.AddModelError(file.Name, $"The file is empty.");
            }
            else
            {
                try
                {
                    CloudStorageAccount storageAccount = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(azureStorageAccount, azureStoragePassword), true);

                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                    await container.CreateIfNotExistsAsync();

                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                    fileName = string.IsNullOrWhiteSpace(fileName) ? file.FileName : Path.GetFileNameWithoutExtension(fileName);

                    using (var reader = new StreamReader(file.OpenReadStream(), true))
                    {
                        if (!overwrite)
                        {
                            var count = 1;
                            while (await blockBlob.ExistsAsync())
                            {
                                fileName = $"{Path.GetFileNameWithoutExtension(fileName)}[{count++}]{Path.GetExtension(file.FileName)}";
                            }
                        }
                        else
                            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}{Path.GetExtension(file.FileName)}";

                        blockBlob = container.GetBlockBlobReference(fileName);
                        await blockBlob.UploadFromStreamAsync(reader.BaseStream);
                    }

                    return blockBlob.Uri;
                }
                catch (IOException ex)
                {
                    modelState.AddModelError(file.Name, $"The file upload failed. Please contact the Help Desk for support.");
                    // Log the exception
                }
            }

            return new Uri("error");
        }
    }
}
