# ------------------ CONFIG ------------------
$infPath          = "C:\Work\DocuWare\docuware-v3\CustomDriver\VirtualPdfPrinter.inf"
$printerDriverDir = Split-Path $infPath
$catOutputPath    = Join-Path $printerDriverDir "VirtualPdfPrinter.cat"

# ✅ Correct path to the certificate
$pfxPath          = "C:\Work\DocuWare\docuware-v3\Certificate\VirtualPdfPrinterPSA.pfx"
$pfxPassword      = "MySecurePassword123!"  # Replace with actual password

# Tool paths (adjust version number if needed)
$inf2catPath      = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x86\inf2cat.exe"
$signtoolPath     = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x86\signtool.exe"
# -------------------------------------------

# 1. Generate .cat file using inf2cat
Write-Host "Generating .cat file..." -ForegroundColor Cyan
Push-Location $printerDriverDir
& $inf2catPath /driver:. /os:10_X64,10_X86 /uselocaltime
Pop-Location

if (!(Test-Path $catOutputPath)) {
    Write-Error "❌ Failed to generate CAT file."
    exit 1
}

# 2. Sign the .cat file using signtool
Write-Host "Signing .cat file..." -ForegroundColor Cyan
& $signtoolPath sign `
    /f $pfxPath `
    /p $pfxPassword `
    /tr http://timestamp.digicert.com `
    /td sha256 `
    /fd sha256 `
    $catOutputPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ signtool failed."
    exit 1
}
