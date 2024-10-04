using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WSUSCommander.Windows
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private bool _isCancelled = false;
        public bool IsCancelled => _isCancelled;

        public ProgressWindow()
        {
            InitializeComponent();
        }

        // Method to update the title of the window
        public void SetWindowTitle(string title)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Title = title;
            });
        }

        // Method to update the progress bar value
        public void UpdateProgress(double percentage)
        {
            this.Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = percentage;
            });
        }

        // Method to update the "doing X of Y" text
        public void UpdateIterationText(string text)
        {
            this.Dispatcher.Invoke(() =>
            {
                IterationTextBlock.Text = text;
            });
        }

        // Event handler for cancel button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isCancelled = true;
            this.Close();
        }

        // Override OnClosing to prevent immediate closing without setting cancellation
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_isCancelled)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Cancel button to exit the progress window.");
            }
        }
    }
}
