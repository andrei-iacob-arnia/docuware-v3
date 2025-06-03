using System;
using Windows.ApplicationModel.Background;
using Windows.Graphics.Printing.Workflow;

namespace VirtualPdfPrinterPSA
{
    public sealed class VirtualPrinterBackgroundTask : IBackgroundTask
    {
        BackgroundTaskDeferral taskDeferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            System.IO.File.AppendAllText(@"C:\Work\DocuWare\docuware-v3\psa-logs.txt", $"VirtualPrinterBackgroundTask -> Run method hit at {DateTime.Now}\r\n");

            // Take Task Deferral            
            taskDeferral = taskInstance.GetDeferral();

            var jobTriggerDetails = taskInstance.TriggerDetails as PrintWorkflowJobTriggerDetails;

            var workflowBackgroundSession = jobTriggerDetails.PrintWorkflowJobSession as PrintWorkflowJobBackgroundSession;
            // Register for events
            workflowBackgroundSession.JobStarting += this.OnJobStarting;
            workflowBackgroundSession.PdlModificationRequested += this.OnPdlModificationRequested;
            // Start Firing events
            workflowBackgroundSession.Start();
        }

        private void OnJobStarting(PrintWorkflowJobBackgroundSession session, PrintWorkflowJobStartingEventArgs args)
        {
            System.IO.File.AppendAllText(@"C:\Work\DocuWare\docuware-v3\psa-logs.txt", $"VirtualPrinterBackgroundTask -> OnJobStarting method hit at {DateTime.Now}\r\n");

            using (args.GetDeferral())
            {
                // Call SetSkipSystemRendering to skip conversion for XPS to PDL, so that PSA can directly manipulate the XPS file.
                args.SetSkipSystemRendering();
            }
        }


        private async void OnPdlModificationRequested(PrintWorkflowJobBackgroundSession session, PrintWorkflowPdlModificationRequestedEventArgs args)
        {
            System.IO.File.AppendAllText(@"C:\Work\DocuWare\docuware-v3\psa-logs.txt", $"VirtualPrinterBackgroundTask -> OnPdlModificationRequested method hit at {DateTime.Now}\r\n");

            using (args.GetDeferral())
            {
                if (String.Equals(args.SourceContent.ContentType, "application/oxps", StringComparison.OrdinalIgnoreCase))
                {
                    var xpsContent = args.SourceContent.GetInputStream();

                    var printTicket = args.PrinterJob.GetJobPrintTicket();
                    PrintWorkflowPdlTargetStream streamTarget = args.CreateJobOnPrinter("application/pdf");

                    // Modify XPS stream here to make the needed changes 
                    // for example adding a watermark

                    PrintWorkflowPdlConverter pdlConverter = args.GetPdlConverter(PrintWorkflowPdlConversionType.XpsToPdf);
                    await pdlConverter.ConvertPdlAsync(printTicket, xpsContent, streamTarget.GetOutputStream());

                    streamTarget.CompleteStreamSubmission(PrintWorkflowSubmittedStatus.Succeeded);
                }
                else
                {
                    // We except source content to be XPS in this case, abort the session if it is not XPS.
                    args.Configuration.AbortPrintFlow(PrintWorkflowJobAbortReason.JobFailed);
                }
            }
            this.taskDeferral.Complete();

        }
    }
}
