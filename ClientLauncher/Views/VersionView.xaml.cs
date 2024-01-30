using System.Windows.Controls;
using Version = UAM.Core.Models.Version;

namespace ClientLauncher.Views;

public partial class VersionView : UserControl
{
    public readonly Version CurrentVersion;
    public VersionView(Version version)
    {
        CurrentVersion = version;
        InitializeComponent();

        BuildTextBlock.Text = version.Build;
        TimestampTextBlock.Text = version.Timestamp.ToString("d MMMM yyyy");
        DescriptionTextBlock.Text = version.Description;
        TypeTextBlock.Text = version.Type.ToString();
    }
}