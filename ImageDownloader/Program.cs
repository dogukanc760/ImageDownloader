using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            InputData inputData = ReadInputFromConsole();

            Console.WriteLine($"Downloading {inputData.Count} images ({inputData.Parallelism} parallel downloads at most)");
            Console.WriteLine();

            ImageDownloader downloader = new ImageDownloader(inputData);
            downloader.ProgressChanged += Downloader_ProgressChanged;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine();
                Console.WriteLine("Cancelling download...");
                cancellationTokenSource.Cancel();
                downloader.CleanUp();
                Console.WriteLine("Download cancelled. Press any key to exit.");
            };
            try
            {
                downloader.DownloadImages(cancellationTokenSource.Token);
                Console.WriteLine();
                Console.WriteLine("    Download completed. Press any key to exit.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("Download cancelled. Cleaning up...");
                downloader.CleanUp();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.ReadKey();
        }

        static InputData ReadInputFromConsole()
        {
            Console.Write("Enter the number of images to download: ");
            int count = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter the maximum parallel download limit: ");
            int parallelism = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter the save path (default: ./outputs): ");
            string savePath = Console.ReadLine();

            if (string.IsNullOrEmpty(savePath))
                savePath = "./outputs";

            return new InputData(count, parallelism, savePath);
        }

        static void Downloader_ProgressChanged(object sender, ProgressEventArgs e)
        {
            Console.CursorTop = Console.CursorTop - 1;
            Console.WriteLine($"Progress: {e.CurrentProgress}/{e.TotalCount}");
        }
    }

 
}
