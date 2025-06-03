# ------------------ CERTIFICATE CREATION ------------------

$certFolder = "C:\Work\DocuWare\docuware-v2\Certificate"
$certName = "VirtualPdfPrinterPSA"
$pfxPath = Join-Path $certFolder "$certName.pfx"
$cerPath = Join-Path $certFolder "$certName.cer"
$certPassword = ConvertTo-SecureString -String "MySecurePassword123!" -Force -AsPlainText

# Ensure certificate folder exists
if (-not (Test-Path $certFolder)) {
    New-Item -Path $certFolder -ItemType Directory | Out-Null
}

# Create a new self-signed code-signing certificate
Write-Host "Creating self-signed certificate..." -ForegroundColor Cyan
$cert = New-SelfSignedCertificate -Type CodeSigningCert `
    -Subject "CN=$certName" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -KeyExportPolicy Exportable `
    -KeySpec Signature `
    -NotAfter (Get-Date).AddYears(5)

# Export .pfx for signing
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $certPassword

# Export .cer for trust installation
Export-Certificate -Cert $cert -FilePath $cerPath

# Trust the certificate by importing to LocalMachine\Root
Write-Host "Importing certificate to Trusted Root..." -ForegroundColor Cyan
Import-Certificate -FilePath $cerPath -CertStoreLocation "Cert:\LocalMachine\Root"

Write-Host ""
Write-Host "Certificate created and trusted successfully." -ForegroundColor Green
Write-Host "PFX path: $pfxPath"
Write-Host "CER path: $cerPath"
