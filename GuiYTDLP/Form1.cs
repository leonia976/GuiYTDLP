using GuiYTDLP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GuiYTDLP.DownloadTaskItem;

namespace GuiYTDLP;

public partial class Form1 : Form
{
    private readonly List<DownloadTaskItem> _tasks = new();
    private readonly SemaphoreSlim _downloadSemaphore = new(10, 10);
    private bool _isUpdating = false;
    private readonly List<ProfiloDownload> _profili = new()
    {
        new("MP4 1080p (max res)", "ツ",
            "-f \"bv*[height<=1080][vcodec^=avc1]+ba[ext=m4a]/bv*[height<=1080]+ba/b[height<=1080]/b\" --merge-output-format mp4 --embed-thumbnail --convert-thumbnails jpg --embed-metadata"),
        new("MP4 1080p (compatible)", "ツ",
            "-f \"bv*[height<=1080][fps<=30][vcodec^=avc1]+ba[ext=m4a]/b[height<=1080][fps<=30][vcodec^=avc1]/bv*[height<=1080]+ba/b[height<=1080]\" --recode-video mp4 --postprocessor-args \"ffmpeg:-c:v libx264 -profile:v high -level 4.1 -pix_fmt yuv420p -r 30 -c:a aac -b:a 192k\" --restrict-filenames"),
        new("MP4 (max res)", "ツ",
            "-f \"bv*[vcodec^=avc1]+ba[ext=m4a]/bv*+ba/b\" --merge-output-format mp4 --embed-thumbnail --convert-thumbnails jpg --embed-metadata"),
        new("MKV (max res)", "ツ",
            "-f \"bv*+ba/b\" --merge-output-format mkv --embed-thumbnail --convert-thumbnails jpg --embed-metadata"),
        new("MKV 1080p (max res)", "ツ",
            "-f \"bv*[height<=1080]+ba/b[height<=1080]\" --merge-output-format mkv --embed-thumbnail --convert-thumbnails jpg --embed-metadata"),
        new("MP3 (192k)", "ツ",
            "-x --audio-format mp3 --audio-quality 192K --embed-thumbnail --convert-thumbnails jpg --embed-metadata"),
        new("MP3 (best)", "ツ",
            "-x --audio-format mp3 --audio-quality 0 --embed-thumbnail --convert-thumbnails jpg --embed-metadata"),
        new("Only Audio (M4A best)", "ツ",
            "-f \"ba[ext=m4a]/ba/b\" --embed-metadata"),
        new("Only Video (MP4 best)", "ツ",
            "-f \"bv*[ext=mp4]/bv*\" --remux-video mp4 --embed-metadata"),
        new("MP4 (lowest res)", "ツ",
            "-f \"wv*+wa/w\" --remux-video mp4 --embed-metadata"),
        new("MP4 (lowest res - compatible)", "ツ",
            "-f \"wv*[vcodec^=avc1]+wa[acodec^=mp4a]/w[ext=mp4]/w\" --merge-output-format mp4 --embed-metadata"),
        new("MP4 (lowest res - no audio)", "ツ",
            "-f \"wv*[ext=mp4]/wv*\" --remux-video mp4 --embed-metadata")
    };

    private const string UrlPlaceholder =
        "https://www.youtube.com/watch?v=cvnh8MDxNYA\r\n" +
        "https://www.youtube.com/watch?v=0A4XxOYOgi8\r\n" +
        "https://www.youtube.com/watch?v=qC3L0VCeCEU";

    public Form1()
    {
        InitializeComponent();

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        string verStr = version != null ? $"v{version.Major}.{version.Minor}.{version.Build}" : "v1.0.0";
        this.Text = $"Gui YTDLP {verStr}";

        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.SizeGripStyle = SizeGripStyle.Show;

        lvDownloads.SmallImageList = new ImageList { ImageSize = new Size(16, 24) };
        typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
            .SetValue(lvDownloads, true, null);

        string defaultFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Download");
        txtSavePath.Text = defaultFolder.Replace("\\", "/");

        cmbPreset.Items.Clear();
        cmbPreset.Items.AddRange(_profili.ToArray());
        cmbPreset.SelectedIndex = 0;

        SetupUrlPlaceholder();

        ThemeHelper.ApplyDarkTheme(
            this,
            new[] { grpParameters, grpDownloads },
            new[] { txtUrls, txtSavePath },
            cmbPreset,
            lvDownloads,
            new[] { btnBrowse, btnOpenFolder, btnAdd, BtnPaste, btnClearLink, btnUpdateBin, btnClearAll, btnStartDownload },
            contextMenu,
            txtUrls,
            UrlPlaceholder);

        AppStorage.LoadWindowState(this);
        this.FormClosing += (s, e) => AppStorage.SaveWindowState(this);
        this.Shown += (s, e) =>
        {
            this.ActiveControl = btnStartDownload;
            AdjustListViewColumns();
        };
        lvDownloads.SizeChanged += (s, e) => AdjustListViewColumns();
    }

    private void SetupUrlPlaceholder()
    {
        txtUrls.Text = UrlPlaceholder;
        txtUrls.ForeColor = Color.Gray;

        txtUrls.Enter += (s, e) =>
        {
            if (txtUrls.Text == UrlPlaceholder)
            {
                txtUrls.Text = string.Empty;
                txtUrls.ForeColor = Color.FromArgb(235, 235, 235);
            }
        };

        txtUrls.Leave += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtUrls.Text))
            {
                txtUrls.Text = UrlPlaceholder;
                txtUrls.ForeColor = Color.Gray;
            }
        };

        string savedLinks = AppStorage.LoadLinks();
        if (!string.IsNullOrWhiteSpace(savedLinks))
        {
            txtUrls.Text = savedLinks;
            txtUrls.ForeColor = Color.FromArgb(235, 235, 235);
        }
    }

    // --- EVENTI UI ---

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        using FolderBrowserDialog dialog = new();
        if (dialog.ShowDialog() == DialogResult.OK)
            txtSavePath.Text = dialog.SelectedPath.Replace("\\", "/");
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (txtUrls.Text == UrlPlaceholder || string.IsNullOrWhiteSpace(txtUrls.Text))
            return;

        string[] lines = txtUrls.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        AppStorage.SaveLinks(lines, UrlPlaceholder);

        string folder = txtSavePath.Text.Trim();
        bool allowPlaylist = chkPlaylist.Checked;

        var profilo = cmbPreset.SelectedItem as ProfiloDownload;
        string etichettaTabella = profilo?.EtichettaBreve ?? "Best";
        string nomeProfilo = profilo?.Nome ?? string.Empty;

        txtUrls.Clear();
        btnAdd.Focus();

        foreach (string rawLine in lines)
        {
            string url = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(url)) continue;

            if (allowPlaylist && (url.Contains("list=", StringComparison.OrdinalIgnoreCase) || url.Contains("/playlist", StringComparison.OrdinalIgnoreCase)))
                await ExpandAndAddPlaylistAsync(url, etichettaTabella, nomeProfilo, folder);
            else
                AddSingleTaskToList(url, url, etichettaTabella, nomeProfilo, folder, "%(title)s.%(ext)s");
        }
    }

    private void AddSingleTaskToList(string url, string initialTitle, string etichettaTabella, string nomeProfilo, string folder, string outputTemplate)
    {
        var task = new DownloadTaskItem(url, etichettaTabella, folder)
        {
            Title = initialTitle,
            ProfileName = nomeProfilo,
            OutputTemplate = outputTemplate
        };

        _tasks.Add(task);
        lvDownloads.Items.Add(task.ListViewItem);

        _ = FaviconService.GetFaviconAsync(url).ContinueWith(t =>
        {
            task.Favicon = t.Result;
            UpdateRowSafe(task);
        });
    }

    private async Task ExpandAndAddPlaylistAsync(string playlistUrl, string etichettaTabella, string nomeProfilo, string folder)
    {
        string ytDlpPath = Path.Combine(AppContext.BaseDirectory, "bin", "yt-dlp.exe");
        if (!File.Exists(ytDlpPath))
        {
            AddSingleTaskToList(playlistUrl, playlistUrl, etichettaTabella, nomeProfilo, folder, "%(playlist_index)02d - %(title)s.%(ext)s");
            return;
        }

        try
        {
            string jsonOutput = await Task.Run(async () =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                psi.ArgumentList.Add("--flat-playlist");
                psi.ArgumentList.Add("--dump-single-json");
                psi.ArgumentList.Add("--no-warnings");
                psi.ArgumentList.Add(playlistUrl);

                using var proc = Process.Start(psi);
                if (proc == null) return string.Empty;

                string output = await proc.StandardOutput.ReadToEndAsync();
                await proc.WaitForExitAsync();
                return output;
            });

            if (!string.IsNullOrWhiteSpace(jsonOutput))
            {
                using JsonDocument doc = JsonDocument.Parse(jsonOutput);
                if (doc.RootElement.TryGetProperty("entries", out JsonElement entries) && entries.ValueKind == JsonValueKind.Array)
                {
                    int index = 1;
                    foreach (JsonElement entry in entries.EnumerateArray())
                    {
                        string? videoUrl = entry.TryGetProperty("url", out var u) ? u.GetString() : null;
                        if (string.IsNullOrWhiteSpace(videoUrl) && entry.TryGetProperty("webpage_url", out var wu))
                            videoUrl = wu.GetString();

                        string title = entry.TryGetProperty("title", out var t) ? (t.GetString() ?? $"Video {index}") : $"Video {index}";

                        if (!string.IsNullOrWhiteSpace(videoUrl))
                        {
                            if (!videoUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                                videoUrl = $"https://youtube.com{videoUrl}";

                
                            AddSingleTaskToList(videoUrl, $"{index:D2} - {title}", etichettaTabella, nomeProfilo, folder, $"{index:D2} - %(title)s.%(ext)s");
                            index++;
                        }
                    }
                    return;
                }
            }
        }
        catch { }
        AddSingleTaskToList(playlistUrl, playlistUrl, etichettaTabella, nomeProfilo, folder, "%(title)s.%(ext)s");
    }
    private void BtnStartDownload_Click(object? sender, EventArgs e)
    {
        foreach (var task in _tasks)
        {
            if (task.Status == "Queued" || task.Status == "Error")
            {
                task.Status = "Queued";
                UpdateRowSafe(task);
                _ = Task.Run(() => RunYtDlpWorkerAsync(task));
            }
        }
    }
    private void BtnClearAll_Click(object? sender, EventArgs e)
    {
        for (int i = _tasks.Count - 1; i >= 0; i--)
        {
            if (_tasks[i].Status != "Downloading" && _tasks[i].Status != "Converting...")
            {
                lvDownloads.Items.Remove(_tasks[i].ListViewItem);
                _tasks.RemoveAt(i);
            }
        }
    }
    private async void btnUpdateBin_Click(object sender, EventArgs e)
    {
        if (_isUpdating) return;

        if (_tasks.Exists(t => t.Status == "Downloading" || t.Status == "Converting..."))
        {
            MessageBox.Show("Wait for ongoing downloads to complete before updating the files.",
                "Active downloads", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _isUpdating = true;
        btnStartDownload.Enabled = false;

        try
        {
            await BinUpdater.UpdateAllAsync(UpdateButtonText);
            MessageBox.Show("All components in the 'bin' folder have been download-updated!",
                "Update-download complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while downloading the update:\n{ex.Message}",
                "Update/download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _isUpdating = false;
            btnUpdateBin.Text = "🔄 Download Binary";
            btnStartDownload.Enabled = true;
        }
    }

    private void UpdateButtonText(string text)
    {
        if (IsDisposed) return;
        if (InvokeRequired) BeginInvoke(() => btnUpdateBin.Text = text);
        else btnUpdateBin.Text = text;
    }
    private void BtnPaste_Click(object? sender, EventArgs e)
    {
        if (!Clipboard.ContainsText()) return;
        string clipText = Clipboard.GetText().Trim();
        if (string.IsNullOrWhiteSpace(clipText)) return;
        if (txtUrls.Text == UrlPlaceholder)
        {
            txtUrls.Clear();
            txtUrls.ForeColor = Color.FromArgb(235, 235, 235);
        }
        if (!string.IsNullOrWhiteSpace(txtUrls.Text))
            txtUrls.AppendText(Environment.NewLine + clipText);
        else
            txtUrls.Text = clipText;
    }
    private void btnClearLink_Click(object sender, EventArgs e)
    {
        txtUrls.Clear();
        btnClearLink.Focus();
    }
    private void btnOpenFolder_Click(object sender, EventArgs e) => MenuOpenFolder_Click(sender, e);
    // --- MENU CONTESTUALE ---
    private DownloadTaskItem? GetSelectedTask()
    {
        foreach (ListViewItem item in lvDownloads.SelectedItems)
            return item.Tag as DownloadTaskItem;
        return null;
    }
    private void MenuDelete_Click(object? sender, EventArgs e)
    {
        var task = GetSelectedTask();
        if (task == null) return;
        try { if (task.ActiveProcess is { HasExited: false }) task.ActiveProcess.Kill(true); } catch { }
        lvDownloads.Items.Remove(task.ListViewItem);
        _tasks.Remove(task);
    }
    private void MenuCopyUrl_Click(object? sender, EventArgs e)
    {
        var task = GetSelectedTask();
        if (task != null) Clipboard.SetText(task.Url);
    }
    private void MenuOpenFolder_Click(object? sender, EventArgs e)
    {
        string folder = GetSelectedTask()?.OutputFolder ?? txtSavePath.Text;
        if (Directory.Exists(folder))
            Process.Start("explorer.exe", folder.Replace("/", "\\"));
    }

    // --- WORKER DOWNLOAD ---
    private async Task RunYtDlpWorkerAsync(DownloadTaskItem task)
    {
        await _downloadSemaphore.WaitAsync();
        try
        {
            if (!_tasks.Contains(task)) return;
            task.Status = "Starting...";
            UpdateRowSafe(task);
            Directory.CreateDirectory(task.OutputFolder);
            string binFolder = Path.Combine(AppContext.BaseDirectory, "bin");
            string ytDlpPath = Path.Combine(binFolder, "yt-dlp.exe");
            if (!File.Exists(ytDlpPath))
            {
                task.Status = "Error";
                UpdateRowSafe(task);
                BeginInvoke(() => MessageBox.Show(
                $"File non trovato:\n{ytDlpPath}\n\nPremi il pulsante 'Update' per scaricare automaticamente yt-dlp.exe e FFmpeg nella cartella 'bin'.",
                "Eseguibile mancante", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                return;
            }
            string profileArgs = _profili.Find(p => p.Nome == task.ProfileName || p.EtichettaBreve == task.Preset)?.Argomenti ?? "-f \"bv*+ba/b\"";

            var psi = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"{profileArgs} --no-playlist --ffmpeg-location \"{binFolder}\" --newline --progress --no-simulate --print \"TITLE:%(title)s\" -o \"{task.OutputTemplate}\" -P \"{task.OutputFolder}\" \"{task.Url}\"",

                UseShellExecute = false,
RedirectStandardOutput = true,
RedirectStandardError = true,
CreateNoWindow = true
        };
        psi.EnvironmentVariables["PYTHONUNBUFFERED"] = "1";
        using var process = new Process { StartInfo = psi };
        task.ActiveProcess = process;
        process.Start();
        task.Status = "Downloading";
        UpdateRowSafe(task);
        async Task ReadStreamAsync(StreamReader reader)
        {
            while (true)
            {
                string? line = await reader.ReadLineAsync();
                if (line == null) break;
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("TITLE:"))
                {
                    task.Title = line.Substring(6).Trim();
                    UpdateRowSafe(task);
                    continue;
                }
                if (line.Contains("[download]") && line.Contains("%"))
                {
                    ParseStandardProgress(line, task);
                    UpdateRowSafe(task);
                }
                else if (line.Contains("[Merger]") || line.Contains("[ExtractAudio]") || line.Contains("[VideoConvertor]"))
                {
                    task.Status = "Converting...";
                    UpdateRowSafe(task);
                }
            }
        }
        await Task.WhenAll(ReadStreamAsync(process.StandardOutput), ReadStreamAsync(process.StandardError));
        await process.WaitForExitAsync();
        if (process.ExitCode == 0)
        {
            task.Progress = 100;
            task.Status = "Finished";
            task.Eta = "NA";
        }
        else
        {
            task.Status = "Error";
        }
    }
catch (Exception ex)
{
task.Status = "Error";
BeginInvoke(() => MessageBox.Show($"Errore yt-dlp.exe: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error));
}
finally
{
task.ActiveProcess = null;
_downloadSemaphore.Release();
UpdateRowSafe(task);
}
}
private static void ParseStandardProgress(string line, DownloadTaskItem task)
{
    int pctIdx = line.IndexOf('%');
    if (pctIdx > 0)
    {
        string beforePct = line.Substring(0, pctIdx).Trim();
        int lastSpace = beforePct.LastIndexOf(' ');
        string numStr = lastSpace >= 0 ? beforePct.Substring(lastSpace + 1) : beforePct;
        if (double.TryParse(numStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double pct))
            task.Progress = (int)Math.Round(pct);
    }
    int ofIdx = line.IndexOf(" of ");
    if (ofIdx >= 0)
    {
        string afterOf = line.Substring(ofIdx + 4).Trim().TrimStart('~').Trim();
        int spaceAfterSize = afterOf.IndexOf(' ');
        if (spaceAfterSize > 0) task.Size = afterOf.Substring(0, spaceAfterSize).Trim();
    }
    int atIdx = line.IndexOf(" at ");
    if (atIdx >= 0)
    {
        string afterAt = line.Substring(atIdx + 4).Trim();
        int spaceAfterSpeed = afterAt.IndexOf(' ');
        if (spaceAfterSpeed > 0) task.Speed = afterAt.Substring(0, spaceAfterSpeed).Trim();
    }
    int etaIdx = line.IndexOf(" ETA ");
    if (etaIdx >= 0) task.Eta = line.Substring(etaIdx + 5).Trim();
}
private void UpdateRowSafe(DownloadTaskItem task)
{
    if (IsDisposed) return;
    void Apply() { task.RefreshRow(); lvDownloads.Invalidate(); }
    if (InvokeRequired) BeginInvoke(Apply); else Apply();
}
// --- DISEGNO LISTVIEW ---
private void LvDownloads_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
{
    e.DrawDefault = false;
    using (var bgBrush = new SolidBrush(Color.FromArgb(34, 34, 34)))
    using (var borderPen = new Pen(Color.FromArgb(62, 62, 62)))
    {
        e.Graphics.FillRectangle(bgBrush, e.Bounds.X, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height);
        e.Graphics.DrawLine(borderPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
    }
    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
    (e.ColumnIndex == 0 ? TextFormatFlags.Left : TextFormatFlags.HorizontalCenter);
    Rectangle textBounds = e.ColumnIndex == 0
    ? new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height)
    : e.Bounds;
    TextRenderer.DrawText(e.Graphics, e.Header?.Text ?? string.Empty, lvDownloads.Font, textBounds, Color.FromArgb(200, 200, 200), flags);
}
private void LvDownloads_DrawItem(object? sender, DrawListViewItemEventArgs e) => e.DrawDefault = false;
private void LvDownloads_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
{
    if (e.Item == null || e.SubItem == null) return;
    Color rowBg = e.Item.Selected ? Color.FromArgb(55, 80, 115) : Color.FromArgb(42, 42, 42);
    Color textColor = Color.FromArgb(235, 235, 235);
    using (var bgBrush = new SolidBrush(rowBg))
        e.Graphics.FillRectangle(bgBrush, e.Bounds);
    if (e.ColumnIndex == 0)
    {
        Rectangle textBounds = e.Bounds;
        var task = e.Item.Tag as DownloadTaskItem;
        if (task?.Favicon != null)
        {
            int iconSize = 16;
            e.Graphics.DrawImage(task.Favicon, e.Bounds.X + 6, e.Bounds.Y + (e.Bounds.Height - iconSize) / 2, iconSize, iconSize);
            textBounds = new Rectangle(e.Bounds.X + 26, e.Bounds.Y, Math.Max(0, e.Bounds.Width - 26), e.Bounds.Height);
        }
        TextRenderer.DrawText(e.Graphics, e.SubItem.Text, lvDownloads.Font, textBounds, textColor,
        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }
    else if (e.ColumnIndex == 3 && int.TryParse(e.SubItem.Text, out int progress))
    {
        int barHeight = 10, marginX = 12;
        int barWidth = e.Bounds.Width - (marginX * 2);
        int barY = e.Bounds.Y + (e.Bounds.Height - barHeight) / 2;
        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(75, 75, 75)), e.Bounds.X + marginX, barY, barWidth, barHeight);
        int fillWidth = (int)(barWidth * (Math.Clamp(progress, 0, 100) / 100.0));
        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 140, 255)), e.Bounds.X + marginX, barY, fillWidth, barHeight);
    }
    else
    {
        TextRenderer.DrawText(e.Graphics, e.SubItem.Text, lvDownloads.Font, e.Bounds, textColor,
        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
private void AdjustListViewColumns()
{
    if (lvDownloads.Columns.Count < 7) return;
    int totalWidth = lvDownloads.ClientSize.Width - 4;
    if (this.WindowState == FormWindowState.Maximized || totalWidth > 1050)
    {
        colTitle.Width = 480;
        colPreset.Width = 140;
        colSize.Width = 100;
        colStatus.Width = 130;
        colSpeed.Width = 115;
        colEta.Width = 95;
        int used = colTitle.Width + colPreset.Width + colSize.Width + colStatus.Width + colSpeed.Width + colEta.Width;
        colProgress.Width = Math.Clamp(totalWidth - used, 150, 320);
    }
    else
    {
        colPreset.Width = 110;
        colSize.Width = 80;
        colProgress.Width = 135;
        colStatus.Width = 105;
        colSpeed.Width = 95;
        colEta.Width = 75;
        int fixedWidth = colPreset.Width + colSize.Width + colProgress.Width + colStatus.Width + colSpeed.Width + colEta.Width;
        colTitle.Width = Math.Clamp(totalWidth - fixedWidth, 240, 420);
    }
}
private void Form1_Load(object sender, EventArgs e) { }
private void chkPlaylist_CheckedChanged(object sender, EventArgs e) { }
}