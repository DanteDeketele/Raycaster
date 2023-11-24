using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    private const int ProgressBarLength = 50;

    static void Main(string[] args)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string[] mp4Files = Directory.GetFiles(currentDirectory, "*.mp4");

        if (mp4Files.Length == 0)
        {
            Console.WriteLine("No MP4 files found in the current directory.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Important! Be sure to install ffmpeg!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("MP4 files found in the current directory:");

        Console.ForegroundColor = ConsoleColor.Green;
        foreach (string file in mp4Files)
        {
            Console.WriteLine($"- {Path.GetFileName(file)}");
        }
        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine("\nDo you want to convert these files? (Y/N)");
        string response = Console.ReadLine();

        if (response.Trim().ToUpper() != "Y")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Conversion canceled.");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        foreach (string inputFile in mp4Files)
        {
            string outputFolder = Path.Combine(currentDirectory, Path.GetFileNameWithoutExtension(inputFile) + "-data");

            Console.WriteLine($"Converting {Path.GetFileName(inputFile)}...");

            ConvertVideo(inputFile, outputFolder);

            Console.WriteLine(); // Move to the next line after the progress bar
        }

        Console.WriteLine("All conversions completed successfully.");
    }

    static void ConvertVideo(string inputPath, string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);

        // Use FFmpeg to extract frames and audio
        string ffmpegArgs = $"-i \"{inputPath}\" -vf scale=320x180 \"{outputFolder}\\%04d.jpg\" -vn -acodec libmp3lame -ar 44100 -ac 2 -b:a 192k \"{outputFolder}\\audio.mp3\"";
        RunFFmpegCommandWithProgressBar(ffmpegArgs);
    }

    static void RunFFmpegCommandWithProgressBar(string arguments)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            while (!process.StandardError.EndOfStream)
            {
                string line = process.StandardError.ReadLine();
                UpdateProgressBar(line);
            }

            process.WaitForExit();
        }
    }

    static void UpdateProgressBar(string line)
    {
        if (line.Contains("time="))
        {
            // Extract the time information from the FFmpeg output
            string[] parts = line.Split(new[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 4 && TimeSpan.TryParse(parts[3], out TimeSpan currentTime) && TimeSpan.TryParse(parts[5], out TimeSpan duration))
            {
                // Calculate the progress percentage
                double progress = currentTime.TotalSeconds / duration.TotalSeconds;
                int progressBarLength = (int)(progress * ProgressBarLength);

                // Draw the progress bar
                Console.Write("\r[");
                Console.Write(new string('#', progressBarLength).PadRight(ProgressBarLength));
                Console.Write($"] {progress:P0}");
            }
        }
    }
}
