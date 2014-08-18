using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Globalization;

namespace Cloudmp3.AzureBlobClasses
{
    public class AzureAccess
    {
        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=cloudmp3;AccountKey=gHwhRUYX9xNAJIEjcWAG8RayTX//ir8NPNWWAK/BxUQNa85JQizZQ6Emn9ucoxez+M0pa/2q499t4SGQ+ksX+Q==";
        private const string containerName = "test";
        private const string blobStorageUri = "https://cloudmp3.blob.core.windows.net/";

        private string localMp3Directory = "C:/Users/Public/Music/CloudMp3/";


        private CloudStorageAccount _account;
        private CloudBlobClient _client;
        private CloudBlobContainer _container;
        private StorageCredentials _accountCredentials;
        private StorageCredentials _permissions;

        public AzureAccess()
        {
            _accountCredentials = new StorageCredentials("cloudmp3", "gHwhRUYX9xNAJIEjcWAG8RayTX//ir8NPNWWAK/BxUQNa85JQizZQ6Emn9ucoxez+M0pa/2q499t4SGQ+ksX+Q==");
            _account = new CloudStorageAccount(_accountCredentials, true);
            _client = _account.CreateCloudBlobClient();
            _container = _client.GetContainerReference(containerName);
            _container.CreateIfNotExists();

            setPermissions(_container);

            _permissions = new StorageCredentials(_container.GetSharedAccessSignature(new SharedAccessBlobPolicy(), "policy"));
        }

        private void setPermissions(CloudBlobContainer container)
        {
            container.GetPermissions().SharedAccessPolicies.Clear();
            BlobContainerPermissions blobPermissions = new BlobContainerPermissions();
            blobPermissions.SharedAccessPolicies.Add("policy", new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-10),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write |
                SharedAccessBlobPermissions.List
            });

            blobPermissions.PublicAccess = BlobContainerPublicAccessType.Off;
            container.SetPermissions(blobPermissions);
        }

        public void UploadSong(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(fileName);

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        public void DownloadSong(string filePath)
        {
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(filePath);
            string writePath = String.Format("{0}{1}", localMp3Directory, filePath);

            using (var fileStream = System.IO.File.OpenWrite(writePath))
            {
                
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public ObservableCollection<string> GetCloudSongs()
        {
            ObservableCollection<string> cloudSongs = new ObservableCollection<string>();

            foreach (IListBlobItem blob in _container.ListBlobs(null, false))
            {
                cloudSongs.Add(blob.Uri.ToString());
            }

            return cloudSongs;
        }
    }
}
