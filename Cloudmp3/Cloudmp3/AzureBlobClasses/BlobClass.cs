using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;

namespace Cloudmp3.AzureBlobClasses
{
    public class BlobClass
    {
        private const string azureBlobConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=cloudmp3;AccountKey=Ve511Euew+MS6w8SkJct3CZTMGaKOTacGLdUSYbnnxklNw4ec3vUDKMnkw4Gg26wr3cJTp1IaTqyBRFEQR5auQ==";

        public BlobClass()
        {

        }

        private CloudBlockBlob getBlob(string filePath)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("mp3testblob");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(Path.GetFileName(filePath));

            return blockBlob;
        }

        public void uploadSong(string filePath)
        {
            try
            {
                CloudBlockBlob blockBlob = getBlob(filePath);
                blockBlob.Properties.ContentType = "audio/mpeg3";

                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    Console.WriteLine("Uploading " + Path.GetFileName(filePath));
                    blockBlob.UploadFromStream(fileStream);
                    Console.WriteLine("UploadComplete");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }   
        }

        public void downloadSong(string filePath)
        {
            try
            {
                CloudBlockBlob blockBlob = getBlob(filePath);

                using (var fileStream = System.IO.File.OpenWrite("C:/Users/Steven Ulibarri/Desktop/CloudMp3/TestMp3Dir/" + Path.GetFileName(filePath)))
                {
                    Console.WriteLine("DownLoading " + Path.GetFileName(filePath));
                    blockBlob.DownloadToStream(fileStream);
                    Console.WriteLine("DownLoad Complete");
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ObservableCollection<string> getCloudSongs()
        {
            ObservableCollection<string> blobPaths = new ObservableCollection<string>();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("mp3testblob");

            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                blobPaths.Add(blob.Uri.ToString());
            }

            return blobPaths;
        }
    }
}
