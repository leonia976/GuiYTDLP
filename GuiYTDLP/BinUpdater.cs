using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace GuiYTDLP;

public static class BinUpdater
{
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(30) };

    public static async Task UpdateAllAsync(Action<string> onStatusChanged)
    {
        await Task.Run(async () =>
        {
            string binDir = Path.Combine(AppContext.BaseDirectory, "bin");
            string tempDir = Path.Combine(binDir, "temp_bin");

            onStatusChanged("↻ Cleaning...");
            if (Directory.Exists(binDir))
                Directory.Delete(binDir, true);

            Directory.CreateDirectory(tempDir);

            onStatusChanged("↻ Get aria2...");
            string aria2Zip = Path.Combine(tempDir, "aria2.zip");
            byte[] aria2Bytes = await _httpClient.GetByteArrayAsync(
                "https://github.com/aria2/aria2/releases/download/release-1.37.0/aria2-1.37.0-win-32bit-build1.zip");
            await File.WriteAllBytesAsync(aria2Zip, aria2Bytes);

            ZipFile.ExtractToDirectory(aria2Zip, tempDir, true);
            string extractedAria = Path.Combine(tempDir, "aria2-1.37.0-win-32bit-build1", "aria2c.exe");
            string aria2Exe = Path.Combine(binDir, "aria2c.exe");
            File.Move(extractedAria, aria2Exe, true);

            onStatusChanged("↻ Downloading...");
            await RunAria2DownloadAsync(aria2Exe, "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe", binDir, "yt-dlp.exe");
            await RunAria2DownloadAsync(aria2Exe, "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip", tempDir, "ffmpeg.zip");
            await RunAria2DownloadAsync(aria2Exe, "https://github.com/denoland/deno/releases/latest/download/deno-x86_64-pc-windows-msvc.zip", tempDir, "deno.zip");
            await RunAria2DownloadAsync(aria2Exe, "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyWindows.zip", tempDir, "AtomicParsley.zip");

            onStatusChanged("↻ Extracting...");
            ZipFile.ExtractToDirectory(Path.Combine(tempDir, "ffmpeg.zip"), tempDir, true);
            ZipFile.ExtractToDirectory(Path.Combine(tempDir, "deno.zip"), binDir, true);
            ZipFile.ExtractToDirectory(Path.Combine(tempDir, "AtomicParsley.zip"), binDir, true);

            string ffmpegBinDir = Path.Combine(tempDir, "ffmpeg-master-latest-win64-gpl", "bin");
            if (Directory.Exists(ffmpegBinDir))
            {
                foreach (string file in Directory.GetFiles(ffmpegBinDir))
                {
                    string destFile = Path.Combine(binDir, Path.GetFileName(file));
                    File.Move(file, destFile, true);
                }
            }

            onStatusChanged("↻ Finishing...");
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        });
    }

    private static async Task RunAria2DownloadAsync(string aria2Path, string url, string outDir, string outFileName)
    {
        var psi = new ProcessStartInfo
        {
            FileName = aria2Path,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add("-x");
        psi.ArgumentList.Add("8");
        psi.ArgumentList.Add("-s");
        psi.ArgumentList.Add("8");
        psi.ArgumentList.Add("--allow-overwrite=true");
        psi.ArgumentList.Add("-d");
        psi.ArgumentList.Add(outDir);
        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add(outFileName);
        psi.ArgumentList.Add(url);

        using var proc = Process.Start(psi);
        if (proc != null)
        {
            await proc.WaitForExitAsync();
            if (proc.ExitCode != 0)
                throw new Exception($"aria2c returned code {proc.ExitCode} while downloading {outFileName}");
        }
    }
}
