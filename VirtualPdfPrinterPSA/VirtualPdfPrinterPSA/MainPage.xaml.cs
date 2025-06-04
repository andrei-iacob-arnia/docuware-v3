using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VirtualPdfPrinterPSA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a <see cref="Frame">.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            //System.IO.File.AppendAllText(@"C:\Work\DocuWare\docuware-v3\Logs\psa-logs.txt", $"MainPage.xaml -> MainPage method hit at {DateTime.Now}\r\n");
            var folder = Windows.Storage.KnownFolders.DocumentsLibrary;
            var file = folder.CreateFileAsync("psa-logs.txt", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
            Windows.Storage.FileIO.AppendTextAsync(file, $"MainPage.xaml -> MainPage method hit at {DateTime.Now}\r\n");

            PathTextBox.Text = $"Logs path:\n{file.Path}";
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(PathTextBox.Text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
