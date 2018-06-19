using ListingApp.BusinessContracts.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace ListingApp.BusinessComponents.Services
{
	public class AzureUploadService : IAzureUploadService
	{
		public async Task<string> Upload(string connectionString, string name, byte[] data)
		{
			if(CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
			{
				var blobClient = storageAccount.CreateCloudBlobClient();
				var container = blobClient.GetContainerReference("realescort-container");
				if (!await container.ExistsAsync())
				{
					await container.CreateAsync();
				}

				var permissions = new BlobContainerPermissions
				{
					PublicAccess = BlobContainerPublicAccessType.Blob
				};

				await container.SetPermissionsAsync(permissions);

				var cloudBlockBlob = container.GetBlockBlobReference(name);
				await cloudBlockBlob.DeleteIfExistsAsync();
				await cloudBlockBlob.UploadFromByteArrayAsync(data, 0, data.Length);

				return cloudBlockBlob.StorageUri.PrimaryUri.AbsoluteUri;
			}

			throw new ArgumentException("Wrong connection string.");
		}
	}
}
