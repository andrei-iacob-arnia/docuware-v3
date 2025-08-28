using System;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.Devices.Printers;
using Windows.Graphics.Printing.Workflow;
using Windows.Storage;
using Windows.Storage.Streams;

namespace VirtualPdfPrinterPSA.Tasks
{
    public partial class VirtualPrinterBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral taskDeferral;
        private IppPrintDevice printDevice;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //System.IO.File.AppendAllText(@"C:\Work\DocuWare\docuware-v3\Logs\psa-logs.txt", $"App.xaml -> OnLaunched method hit at {DateTime.Now}\r\n");
            var folder = Windows.Storage.KnownFolders.DocumentsLibrary;
            var file = folder.CreateFileAsync("psa-logs.txt", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
            Windows.Storage.FileIO.AppendTextAsync(file, $"VirtualPrinterBackgroundTask -> Run method hit at {DateTime.Now}\r\n");

            var virtualPrinterDetails = taskInstance.TriggerDetails as PrintWorkflowVirtualPrinterTriggerDetails;
            taskDeferral = taskInstance.GetDeferral();

            PrintWorkflowVirtualPrinterSession session = virtualPrinterDetails.VirtualPrinterSession;
            session.VirtualPrinterDataAvailable += VirtualPrinterDataAvailable;
            // Get print device for the session
            printDevice = session.Printer;

            // Make sure to register all the event handlers before PrintWorkflowVirtualPrinterSession.Start is called.
            session.Start();
        }

        private async void VirtualPrinterDataAvailable(PrintWorkflowVirtualPrinterSession sender, PrintWorkflowVirtualPrinterDataAvailableEventArgs args)
        {
            //System.IO.File.AppendAllText(@"C:\Work\DocuWare\docuware-v3\Logs\psa-logs.txt", $"App.xaml -> OnLaunched method hit at {DateTime.Now}\r\n");
            var folder = Windows.Storage.KnownFolders.DocumentsLibrary;
            var file = folder.CreateFileAsync("psa-logs.txt", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
            Windows.Storage.FileIO.AppendTextAsync(file, $"VirtualPrinterBackgroundTask -> VirtualPrinterDataAvailable method hit at {DateTime.Now}\r\n");

            PrintWorkflowSubmittedStatus jobStatus = PrintWorkflowSubmittedStatus.Failed;
            try
            {
                PrintWorkflowPdlSourceContent sourceContent = args.SourceContent;
                // This Software Print Support App always expect input as OXPS and to have target file to be present.
                if (sourceContent.ContentType != "application/oxps")
                {
                    throw new InvalidDataException();
                }


                if (this.printDevice.PrinterUri.AbsolutePath == "print-to-pdf")
                {
                    // For print-to-pdf endpoint converts OXPS to PDF and write the pdf contents to target file.

                    // Get target file output stream.
                    StorageFile targetFile = await args.GetTargetFileAsync();
                    IRandomAccessStream outputStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite);
                    // Get XPS to PDF PDL converter.
                    PrintWorkflowPdlConverter converter = args.GetPdlConverter(PrintWorkflowPdlConversionType.XpsToPdf);
                    // Convert XPS to PDF and write contents to outputStream.
                    await converter.ConvertPdlAsync(args.GetJobPrintTicket(), sourceContent.GetInputStream(), outputStream.GetOutputStreamAt(0));
                    // Complete Endpoint print job.
                    jobStatus = PrintWorkflowSubmittedStatus.Succeeded;
                }
                else if (this.printDevice.PrinterUri.AbsolutePath == "print-to-xps")
                {
                    // For print-to-xps endpoint directly write input XPS file contents to target file.

                    // Get target file output stream.
                    StorageFile targetFile = await args.GetTargetFileAsync();
                    IRandomAccessStream outputStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite);
                    // Copy XPS input stream to target file output stream.
                    await RandomAccessStream.CopyAndCloseAsync(sourceContent.GetInputStream(), outputStream.GetOutputStreamAt(0));
                    // Complete Endpoint print job.
                    jobStatus = PrintWorkflowSubmittedStatus.Succeeded;

                }
                else
                {
                    // This Print Support App only has print-to-pdf and print-to-xps printers, 
                    throw new InvalidDataException();
                }
            }
            finally
            {
                args.CompleteJob(jobStatus);
                taskDeferral.Complete();
            }
        }
    }
}
