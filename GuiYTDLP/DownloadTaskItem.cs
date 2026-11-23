using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
namespace GuiYTDLP;

public class DownloadTaskItem
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Preset { get; set; } = "best";
    public string ProfileName { get; set; } = string.Empty;
    public string Size { get; set; } = "N/A";
    public int Progress { get; set; } = 0;
    public string Status { get; set; } = "Queued";
    public string Speed { get; set; } = "N/A";
    public string Eta { get; set; } = "N/A";
    public string OutputFolder { get; set; } = string.Empty;
    public string OutputTemplate { get; set; } = "%(title)s.%(ext)s";
    public Image? Favicon { get; set; } //
    public ListViewItem ListViewItem { get; set; }
    public Process? ActiveProcess { get; set; }

    public DownloadTaskItem(string url, string preset, string outputFolder)
    {
        Url = url;
        Title = url;
        Preset = preset;
        OutputFolder = outputFolder;

        ListViewItem = new ListViewItem(Title) { Tag = this };
        ListViewItem.SubItems.Add(Preset);
        ListViewItem.SubItems.Add(Size);
        ListViewItem.SubItems.Add(Progress.ToString());
        ListViewItem.SubItems.Add(Status);
        ListViewItem.SubItems.Add(Speed);
        ListViewItem.SubItems.Add(Eta);

    }

    public void RefreshRow()
    {
        var items = ListViewItem.SubItems.Cast<ListViewItem.ListViewSubItem>().ToList();
        if (items.Count < 7) return;

        items.ElementAt(0).Text = Title;
        items.ElementAt(1).Text = Preset;
        items.ElementAt(2).Text = Size;
        items.ElementAt(3).Text = Progress.ToString();
        items.ElementAt(4).Text = Status;
        items.ElementAt(5).Text = Speed;
        items.ElementAt(6).Text = Eta;
    }
    public record ProfiloDownload(string Nome, string EtichettaBreve, string Argomenti)
    {
        public override string ToString() => Nome;
    }
}
