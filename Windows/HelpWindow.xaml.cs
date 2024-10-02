using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace WSUSCommander.Windows
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            // Correctly formatted RTF content
            string rtfText = @"{\rtf1\ansi
                                {\b Main Heading}\par 
                                This is some normal text.\par
                                {\b Sub-heading}\par
                                \pard\li360\bullet First bullet point\par
                                \bullet Second bullet point\par}";

            // Load the RTF content into the RichTextBox
            LoadRtfContent(rtfText);
        }

        private void LoadRtfContent(string rtf)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(rtf);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    var range = new TextRange(HelpRichTextBox.Document.ContentStart, HelpRichTextBox.Document.ContentEnd);
                    range.Load(stream, DataFormats.Rtf);
                }
            }
        }
    }
}
