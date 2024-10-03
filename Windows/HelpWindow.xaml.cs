using System.Windows;
using System.Windows.Documents;

namespace WSUSCommander.Windows
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            // Create a FlowDocument programmatically and add content
            var document = new FlowDocument();

            // Add a heading with controlled spacing
            var heading = new Paragraph(new Run("WSUS Commander"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)  // 10 pixels space below the heading
            };
            document.Blocks.Add(heading);

            // Add some normal text with controlled spacing
            var paragraphA01 = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 10)  // 10 pixels space below
            };
            paragraphA01.Inlines.Add(new Run("WSUSCommander") { FontWeight = FontWeights.Bold });
            paragraphA01.Inlines.Add(new Run(" is a powerful C# application designed to streamline the management of "));
            paragraphA01.Inlines.Add(new Run("Windows Server Update Services") {  FontWeight = FontWeights.Bold });
            paragraphA01.Inlines.Add(new Run(" "));
            paragraphA01.Inlines.Add(new Run("(WSUS)") { FontStyle = FontStyles.Italic });
            paragraphA01.Inlines.Add(new Run(" and related tasks across multiple remote Windows machines within " +
                "a network. This tool provides administrators with a suite of functionalities to efficiently monitor " +
                "and manage system updates, file operations, and service statuses, ensuring optimal performance and " +
                "compliance across the organization's IT infrastructure."));
            document.Blocks.Add(paragraphA01);

            // Prerequisities (B01)
            var subHeadingB01 = new Paragraph(new Run("Prerequisities"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 2)
            };
            document.Blocks.Add(subHeadingB01);

            var paragraphB01 = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            paragraphB01.Inlines.Add(new Run("Befor running WSUS Commander, ensure each machine has the following:"));
            document.Blocks.Add(paragraphB01);

            var bulletListB01 = new List
            {
                MarkerStyle = TextMarkerStyle.Disc,
                Margin = new Thickness(0, 0, 0, 10)
            };
            bulletListB01.ListItems.Add(new ListItem(new Paragraph(new Run("Administrative privileges on both the local and remote machines."))));
            bulletListB01.ListItems.Add(new ListItem(new Paragraph(new Run("Ensure that PowerShell Remoting is enabled on target servers."))));
            bulletListB01.ListItems.Add(new ListItem(new Paragraph(new Run("Firewall rules permitting remote management and file sharing should be configured."))));
            var paragraphB02 = new Paragraph();
            paragraphB02.Inlines.Add(new Run("PSWindowsUpdate") { FontWeight = FontWeights.Bold });
            paragraphB02.Inlines.Add(new Run(" PowerShell module install on remote machines."));
            bulletListB01.ListItems.Add(new ListItem(paragraphB02));
            document.Blocks.Add(bulletListB01);

            // servers.txt
            var subHeadingC01 = new Paragraph(new Run("servers.txt"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 2)
            };
            document.Blocks.Add(subHeadingC01);

            var paragraphC01 = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            paragraphC01.Inlines.Add(new Run("A file named "));
            paragraphC01.Inlines.Add(new Run("servers.txt") { FontWeight = FontWeights.Bold });
            paragraphC01.Inlines.Add(new Run(" should exist in the same directory as the executable. This file will contains server name blocks " +
                "that are named and surrounded by '[' and ']' characters. Each server name "));
            paragraphC01.Inlines.Add(new Run("(either FQDN or DNS name)") { FontStyle = FontStyles.Italic });
            paragraphC01.Inlines.Add(new Run(" would follow with a single server name per line. Any blank lines are ignored. Each server block " +
                "allows for specific servers to be targeted, and separated out.\n"));
            paragraphC01.Inlines.Add(new Run("\tExample:\n"));
            paragraphC01.Inlines.Add(new Run("\t\t[group1]\n" +
                "\t\tserver01\n" +
                "\t\tserver02\n" +
                "\t\tserverC\n" +
                "\n" +
                "\t\t[group2]\n" +
                "\t\tserverTY\n" +
                "\t\tserverTT"));
            document.Blocks.Add(paragraphC01);


            // services.txt
            var subHeadingC02 = new Paragraph(new Run("services.txt"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 2)
            };
            document.Blocks.Add(subHeadingC02);

            var paragraphC02 = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            paragraphC02.Inlines.Add(new Run("A file named "));
            paragraphC02.Inlines.Add(new Run("services.txt") { FontWeight = FontWeights.Bold });
            paragraphC02.Inlines.Add(new Run(" should exist in the same directory as the executable. This is the list of services that should " +
                "be stopped on each machine before any processes are started. This will not discrinimate against services per machine, but all " +
                "machines in each selected server group will be targeted.\n"));
            paragraphC02.Inlines.Add(new Run("\tExample:\n"));
            paragraphC02.Inlines.Add(new Run("\t\tservice1\n" +
                "\t\tservice2\n" +
                "\t\tservice3\n" +
                "\t\tservice4"));
            document.Blocks.Add(paragraphC02);


            // appsettings.json
            var subHeadingD01 = new Paragraph(new Run("appsettings.json"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 2)
            };
            document.Blocks.Add(subHeadingD01);

            var paragraphD01 = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            paragraphD01.Inlines.Add(new Run("A file named "));
            paragraphD01.Inlines.Add(new Run("appsettings.json") { FontWeight = FontWeights.Bold });
            paragraphD01.Inlines.Add(new Run(" should exist in the same directory as the executable. This file is formatted in JSON to contain all " +
                "application settings and configuration. Use the "));
            paragraphD01.Inlines.Add(new Run("README.html") { FontWeight = FontWeights.Bold });
            paragraphD01.Inlines.Add(new Run(" file to have a better understanding of all the configuration settings. The main sections " +
                "are:\n"));
            paragraphD01.Inlines.Add(new Run("WSUSSettings::BackupDir") { FontWeight = FontWeights.Bold });
            paragraphD01.Inlines.Add(new Run(" - \n"));
            paragraphD01.Inlines.Add(new Run("FileBackup::Name") { FontWeight = FontWeights.Bold });
            paragraphD01.Inlines.Add(new Run(" - \n"));
            paragraphD01.Inlines.Add(new Run("FileBackup::SourcePath") { FontWeight = FontWeights.Bold });
            paragraphD01.Inlines.Add(new Run(" - \n"));
            paragraphD01.Inlines.Add(new Run("FileBackup::BackupSubDir") { FontWeight = FontWeights.Bold });
            paragraphD01.Inlines.Add(new Run(" - \n"));

            var subHeading = new Paragraph(new Run("Sub-heading"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)  // 10 pixels space below
            };
            document.Blocks.Add(subHeading);

            // Add bullet points with controlled spacing
            var bulletList = new List
            {
                MarkerStyle = TextMarkerStyle.Disc,
                Margin = new Thickness(0, 0, 0, 10)  // 10 pixels space below the list
            };
            bulletList.ListItems.Add(new ListItem(new Paragraph(new Run("First bullet point"))));
            bulletList.ListItems.Add(new ListItem(new Paragraph(new Run("Second bullet point"))));
            document.Blocks.Add(bulletList);

            // Set the FlowDocument to the RichTextBox
            HelpRichTextBox.Document = document;
        }
    }
}
