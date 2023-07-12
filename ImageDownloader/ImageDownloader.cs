using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDownloader
{

    class ImageDownloader
    {
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        private readonly InputData inputData;
        private int progress;

        public ImageDownloader(InputData inputData)
        {
            this.inputData = inputData;
        }

        public void DownloadImages(CancellationToken cancellationToken)
        {
            List<Task> downloadTasks = new List<Task>();
            object lockObject = new object();

            for (int i = 0; i < inputData.Count; i++)
            {
                string imageUrl = GetRandomImageUrl();
                int imageNumber = i + 1;

                Task downloadTask = Task.Run(() => DownloadImage(imageUrl, imageNumber, lockObject, cancellationToken));
                downloadTasks.Add(downloadTask);

                if (downloadTasks.Count == inputData.Parallelism || imageNumber == inputData.Count)
                {
                    Task.WaitAny(downloadTasks.ToArray());
                    downloadTasks.RemoveAll(t => t.IsCompleted || t.IsCanceled || t.IsFaulted);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        private string GetRandomImageUrl()
        {
            Random random = new Random();
            int width = random.Next(200, 1000);
            int height = random.Next(200, 1000);
            return $"https://picsum.photos/{width}/{height}";
        }

        private void DownloadImage(string imageUrl, int imageNumber, object lockObject, CancellationToken cancellationToken)
        {
            using (WebClient client = new WebClient())
            {
                byte[] imageData = client.DownloadData(imageUrl);
                string filePath = Path.Combine(inputData.SavePath, $"{imageNumber}.png");

                lock (lockObject)
                {
                    progress++;
                    OnProgressChanged(progress, inputData.Count);
                }

                EnsureDirectoryExists(inputData.SavePath);
                File.WriteAllBytes(filePath, imageData);
            }
        }

        private void OnProgressChanged(int currentProgress, int totalCount)
        {
            ProgressChanged?.Invoke(this, new ProgressEventArgs(currentProgress, totalCount));
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public void CleanUp()
        {
            for (int i = 1; i <= inputData.Count; i++)
            {
                string filePath = Path.Combine(inputData.SavePath, $"{i}.png");
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }

}
