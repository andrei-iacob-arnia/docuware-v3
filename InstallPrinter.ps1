# ------------------ CONFIG ------------------
$printerName = "VirtualPDFPrinterQueueName"
$driverName  = "Microsoft XPS Document Writer v4"
$portName    = "VirtualPDFPrinterQueueNamePort:"
# -------------------------------------------

Write-Host "[INFO] Removing existing printer (if it exists)..."

if (Get-Printer -Name $printerName -ErrorAction SilentlyContinue) {
    Remove-Printer -Name $printerName
    Write-Host "[INFO] Removed existing printer: $printerName"
}

# Create the custom port (if not already created)
if (-not (Get-PrinterPort -Name $portName -ErrorAction SilentlyContinue)) {
    Write-Host "[INFO] Creating custom port: $portName"
    Add-PrinterPort -Name $portName
}

# Add the printer using the built-in MXDW driver and custom port
Write-Host "[INFO] Adding printer using built-in driver and custom port..."
Add-Printer -Name $printerName -DriverName $driverName -PortName $portName

Write-Host "[OK] Printer '$printerName' created successfully on port '$portName'."
Write-Host "[INFO] PSA should now trigger when this printer is used."
