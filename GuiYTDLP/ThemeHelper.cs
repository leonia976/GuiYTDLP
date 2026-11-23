using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GuiYTDLP;

public static class ThemeHelper
{
    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string? pszSubIdList);

    public static void ApplyDarkTheme(
        Form form,
        GroupBox[] groups,
        TextBox[] textBoxes,
        ComboBox comboBox,
        ListView listView,
        Button[] buttons,
        ContextMenuStrip contextMenu,
        TextBox txtUrls,
        string urlPlaceholder)
    {
        Color bgMain = Color.FromArgb(30, 30, 30);
        Color bgControl = Color.FromArgb(42, 42, 42);
        Color bgButton = Color.FromArgb(52, 52, 52);
        Color fgText = Color.FromArgb(235, 235, 235);
        Color accentBlue = Color.FromArgb(86, 156, 214);

        _ = SetWindowTheme(form.Handle, "DarkMode_Explorer", null);
        form.BackColor = bgMain;
        form.ForeColor = fgText;

        foreach (var grp in groups)
            grp.ForeColor = accentBlue;

        _ = SetWindowTheme(listView.Handle, "", "");
        listView.GridLines = false;
        listView.BorderStyle = BorderStyle.None;
        listView.BackColor = bgControl;
        listView.ForeColor = fgText;

        foreach (var txt in textBoxes)
        {
            _ = SetWindowTheme(txt.Handle, "DarkMode_Explorer", null);
            txt.BackColor = bgControl;
            txt.ForeColor = (txt == txtUrls && txt.Text == urlPlaceholder) ? Color.Gray : fgText;
        }

        comboBox.BackColor = bgControl;
        comboBox.ForeColor = fgText;

        foreach (Button btn in buttons)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
            btn.BackColor = bgButton;
            btn.ForeColor = fgText;
        }

        contextMenu.ShowImageMargin = false;
        contextMenu.ShowCheckMargin = false;
        contextMenu.Renderer = new ToolStripProfessionalRenderer(new DarkMenuColorTable());
        contextMenu.ForeColor = fgText;
        foreach (ToolStripItem item in contextMenu.Items)
            item.ForeColor = fgText;
    }
}

internal class DarkMenuColorTable : ProfessionalColorTable
{
    public override Color MenuItemSelected => Color.FromArgb(62, 62, 66);
    public override Color MenuItemBorder => Color.FromArgb(80, 80, 85);
    public override Color MenuBorder => Color.FromArgb(65, 65, 65);
    public override Color ToolStripDropDownBackground => Color.FromArgb(37, 37, 38);
    public override Color ImageMarginGradientBegin => Color.FromArgb(37, 37, 38);
    public override Color ImageMarginGradientMiddle => Color.FromArgb(37, 37, 38);
    public override Color ImageMarginGradientEnd => Color.FromArgb(37, 37, 38);
}
