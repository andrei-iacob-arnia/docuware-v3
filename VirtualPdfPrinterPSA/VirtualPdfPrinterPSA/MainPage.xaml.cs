using System;
using Windows.ApplicationModel.DataTransfer;
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
            System.IO.File.AppendAllText(System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "psa-logs.txt"), $"MainPage.xaml -> MainPage method hit at {DateTime.Now}\r\n");

            PathTextBox.Text = $"Logs path:\n{System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "psa-logs.txt")}";
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(PathTextBox.Text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
