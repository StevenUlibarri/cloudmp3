using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudmp3.AzureBlobClasses
{
    public class BlobClass
    {
        private const string azureBlobConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=cloudmp3;AccountKey=Ve511Euew+MS6w8SkJct3CZTMGaKOTacGLdUSYbnnxklNw4ec3vUDKMnkw4Gg26wr3cJTp1IaTqyBRFEQR5auQ==";

        public BlobClass()
        {

        }

        public void uploadSong(string filePath)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference("mp3testblob");

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(Path.GetFileName(filePath));

                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    blockBlob.UploadFromStream(fileStream);
                    Console.WriteLine("UploadComplete");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }   
        }

        //public 

        public void testBlobDownLoad(string filePath = "")
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference("mp3testblob");

                CloudBlockBlob blockBlob = container.GetBlockBlobReference("test");

                using (var fileStream = System.IO.File.OpenWrite("C:/Users/Steven Ulibarri/Desktop/CloudMp3/TestMp3Dir/testDown.mp3"))
                {
                    blockBlob.DownloadToStream(fileStream);
                    Console.WriteLine("DownLoadCompelte");
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
