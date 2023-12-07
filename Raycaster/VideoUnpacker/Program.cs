using System;
using System.Diagnostics;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class Program
{
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

        try
        {
            foreach (string inputFile in mp4Files)
            {
                Console.WriteLine("Started conversion: " + inputFile);
                ConvertVideo(inputFile);
                Console.WriteLine("Completed conversion: " + inputFile);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("All conversions completed successfully.");

        Console.ForegroundColor= ConsoleColor.Green;
        Console.WriteLine("\nPress Enter to exit.");
        Console.ReadLine();
    }

    static void ConvertVideo(string inputPath)
    {
        string currentDirectory = Path.GetDirectoryName(inputPath);
        string outputBaseName = Path.GetFileNameWithoutExtension(inputPath);
        string framesFile = Path.Combine(currentDirectory, $"{outputBaseName}");
        string audioFile = Path.Combine(currentDirectory, $"{outputBaseName}.mp3");

        // Use FFmpeg to extract frames
        string extractFramesArgs = $"-i \"{inputPath}\" -vf \"fps=16,scale=320:-1:flags=lanczos\" -q:v 2 -c:v png \"{framesFile}_%04d-temp.png\"";

        RunFFmpegCommand(extractFramesArgs);


        // Wait for the frame extraction to complete
        Console.WriteLine("Waiting for frame extraction to complete...");
        // Wait for FFmpeg to complete
        WaitForFFmpegCompletion();

        // Use FFmpeg to extract audio
        Console.WriteLine("Extracting audio...");
        string extractAudioArgs = $"-i \"{inputPath}\" -vn -acodec libmp3lame -ar 44100 -ac 2 -b:a 192k \"{audioFile}\"";
        RunFFmpegCommand(extractAudioArgs);

        // Combine frames into a texture sheet
        Console.WriteLine("Combining frames...");
        CombineFrames(currentDirectory, $"{outputBaseName}.png");

        // Delete individual frame files
        Console.WriteLine("Removing temporary files...");
        string[] frameFiles = Directory.GetFiles(currentDirectory, "*-temp.png");
        foreach (var frame in frameFiles)
        {
            File.Delete(frame);
        }
    }

    static void CombineFrames(string directory, string outputFile)
    {
        string[] frameFiles = Directory.GetFiles(directory, "*-temp.png");
        int frameWidth = 320;
        int frameHeight = 180;

        // Assuming you want to arrange the frames in a grid with 100 columns
        int columns = 100;
        int rows = (int)Math.Ceiling((double)frameFiles.Length / columns);

        Console.WriteLine("Enter the size of the black bar at the top and bottom (in pixels):");
        int blackBarSize = int.Parse(Console.ReadLine());

        // Gamma correction factor to control the contrast
        double gamma = 1;

        Console.WriteLine("Started making the file...");

        using (Image<Rgba32> combinedImage = new Image<Rgba32>(frameWidth * columns, frameHeight * rows))
        {
            for (int i = 0; i < frameFiles.Length; i++)
            {
                using (Image<Rgba32> frameImage = Image.Load<Rgba32>(frameFiles[i]))
                {
                    int row = i / columns;
                    int col = i % columns;

                    // Calculate min and max brightness of the frame (ignoring black borders)
                    int minBrightness = int.MaxValue;
                    int maxBrightness = int.MinValue;

                    // Create a single instance of Random outside of the loop
                    Random random = new Random();

                    // Adjust colors based on min and max brightness with gamma correction
                    /*for (int x = 0; x < frameImage.Width; x++)
                    {
                        for (int y = 0; y < frameImage.Height; y++)
                        {
                            Rgba32 pixelColor = frameImage[x, y];

                            int br = (int)(0.21 * pixelColor.R + 0.72 * pixelColor.G + 0.07 * pixelColor.B);
                            br = Math.Max(0, Math.Min(255, br));

                            // Determine the color based on a random chance
                            bool isWhite = random.NextDouble() < (double)br / 255;

                            Rgba32 adjustedColor = isWhite ? new Rgba32(255, 255, 255) : new Rgba32(0, 0, 0);

                            combinedImage[x + col * frameImage.Width, y + row * frameImage.Height] = adjustedColor;
                        }
                    }*/

                    for (int x = 0; x < frameImage.Width; x++)
                    {
                        for (int y = blackBarSize; y < frameImage.Height - blackBarSize; y++)
                        {
                            Rgba32 pixelColor = frameImage[x, y];

                            int br = (int)(0.21 * pixelColor.R + 0.72 * pixelColor.G + 0.07 * pixelColor.B);
                            br = Math.Max(0, Math.Min(255, br));

                            if (br < minBrightness)
                            {
                                minBrightness = br;
                            }

                            if (br > maxBrightness)
                            {
                                maxBrightness = br;
                            }
                        }
                    }

                    // Adjust colors based on min and max brightness with gamma correction
                    for (int x = 0; x < frameImage.Width; x++)
                    {
                        for (int y = 0; y < frameImage.Height; y++)
                        {
                            Rgba32 pixelColor = frameImage[x, y];

                            int br = (int)(0.21 * pixelColor.R + 0.72 * pixelColor.G + 0.07 * pixelColor.B);
                            br = Math.Max(0, Math.Min(255, br));

                            // Adjust color based on min and max brightness with gamma correction
                            float normalizedBrightness = (float)(br - minBrightness) / (maxBrightness - minBrightness);
                            float adjustedBrightness = (float)Math.Pow(normalizedBrightness, gamma);

                            Rgba32 adjustedColor = new Rgba32(adjustedBrightness, adjustedBrightness, adjustedBrightness);

                            combinedImage[x + col * frameImage.Width, y + row * frameImage.Height] = adjustedColor;
                        }
                    }

                    // Display the progress percentage without creating a new line
                    Console.Write($"\rProgress: {(i + 1) * 100 / frameFiles.Length}%");
                }
            }

            Console.WriteLine(); // Move to the next line after the progress is complete

            Console.WriteLine("Saving file...");

            // Save the combined image as a PNG file
            combinedImage.Save(outputFile);
        }
    }




    static void RunFFmpegCommand(string arguments)
    {
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };
        Console.WriteLine("Started generating frames: " + startInfo.Arguments);

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();
        }
    }

    static void WaitForFFmpegCompletion()
    {
        Process[] processes = Process.GetProcessesByName("ffmpeg");

        while (processes.Length > 0)
        {
            Console.WriteLine("Waiting for processes: " + processes.Length);
            // Sleep for a short duration before checking again
            System.Threading.Thread.Sleep(1000);
            processes = Process.GetProcessesByName("ffmpeg");
        }
    }
}
