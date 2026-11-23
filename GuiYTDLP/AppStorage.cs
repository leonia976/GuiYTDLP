using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace GuiYTDLP;

public static class AppStorage
{
    private record WindowStateData(int Width, int Height, int X, int Y, bool IsMaximized);

    private static readonly string LinksFilePath = Path.Combine(AppContext.BaseDirectory, "links.txt");
    private static readonly string WindowStateFilePath = Path.Combine(AppContext.BaseDirectory, "window_state.json");

    public static void SaveLinks(string[] lines, string placeholder)
    {
        try
        {
            var validUrls = new List<string>();
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed) && trimmed != placeholder)
                    validUrls.Add(trimmed);
            }

            if (validUrls.Count > 0)
                File.WriteAllLines(LinksFilePath, validUrls);
        }
        catch { }
    }

    public static string LoadLinks()
    {
        try
        {
            return File.Exists(LinksFilePath) ? File.ReadAllText(LinksFilePath).Trim() : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static void SaveWindowState(Form form)
    {
        try
        {
            bool isMax = form.WindowState == FormWindowState.Maximized;
            Rectangle bounds = form.WindowState == FormWindowState.Normal ? form.Bounds : form.RestoreBounds;

            var state = new WindowStateData(bounds.Width, bounds.Height, bounds.X, bounds.Y, isMax);
            File.WriteAllText(WindowStateFilePath, JsonSerializer.Serialize(state));
        }
        catch { }
    }

    public static void LoadWindowState(Form form)
    {
        try
        {
            if (!File.Exists(WindowStateFilePath)) return;

            var state = JsonSerializer.Deserialize<WindowStateData>(File.ReadAllText(WindowStateFilePath));
            if (state == null) return;

            Rectangle savedRect = new Rectangle(
                state.X,
                state.Y,
                Math.Max(form.MinimumSize.Width, state.Width),
                Math.Max(form.MinimumSize.Height, state.Height));

            bool isOnScreen = Array.Exists(Screen.AllScreens, s => s.WorkingArea.IntersectsWith(savedRect));

            if (isOnScreen)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Bounds = savedRect;
            }
            else
            {
                form.Size = savedRect.Size;
            }

            if (state.IsMaximized)
                form.WindowState = FormWindowState.Maximized;
        }
        catch { }
    }
}
