using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GuiYTDLP;

public static class FaviconService
{
    private static readonly Dictionary<string, Image> _cache = new();
    private static readonly HttpClient _httpClient = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        return client;
    }

    public static async Task<Image?> GetFaviconAsync(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)) return null;

        string domain = GetBaseDomain(uri.Host);
        if (_cache.TryGetValue(domain, out Image? cached)) return cached;

        Image? img = await TryDownloadAsync($"https://duckduckgo.com{domain}.ico")
                  ?? await TryDownloadAsync($"https://google.com{Uri.EscapeDataString(domain)}&sz=32")
                  ?? await TryDownloadAsync($"{uri.Scheme}://{uri.Host}/favicon.ico")
                  ?? CreateFallbackIcon();

        _cache[domain] = img;
        return img;
    }

    private static async Task<Image?> TryDownloadAsync(string iconUrl)
    {
        try
        {
            byte[] bytes = await _httpClient.GetByteArrayAsync(iconUrl);
            if (bytes.Length == 0) return null;

            using var ms = new MemoryStream(bytes);
            try
            {
                using var raw = Image.FromStream(ms);
                return new Bitmap(raw, new Size(16, 16));
            }
            catch
            {
                ms.Position = 0;
                using var ico = new Icon(ms, 16, 16);
                return ico.ToBitmap();
            }
        }
        catch
        {
            return null;
        }
    }

    private static Bitmap CreateFallbackIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using Graphics g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.FillEllipse(new SolidBrush(Color.FromArgb(100, 116, 139)), new Rectangle(1, 1, 14, 14));
        g.FillPolygon(Brushes.White, new Point[] { new(6, 4), new(6, 12), new(12, 8) });
        return bmp;
    }

    private static string GetBaseDomain(string host)
    {
        if (host.Equals("youtu.be", StringComparison.OrdinalIgnoreCase)) return "youtube.com";
        string[] parts = host.Split('.');
        return parts.Length >= 2 ? $"{parts[^2]}.{parts[^1]}" : host;
    }
}
