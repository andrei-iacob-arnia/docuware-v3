# ----------------- CONFIG -----------------
$pfxPath = "C:\Work\DocuWare\docuware-v3\Certificate\VirtualPdfPrinterPSA.pfx"
$pfxPassword = ConvertTo-SecureString "MySecurePassword123!" -AsPlainText -Force
# -----------------------------------------

# Load the PFX certificate from file
$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$cert.Import($pfxPath, $pfxPassword, 'Exportable,PersistKeySet')

# Add to Trusted Root Certification Authorities
$storeRoot = New-Object System.Security.Cryptography.X509Certificates.X509Store "Root", "LocalMachine"
$storeRoot.Open("ReadWrite")
$storeRoot.Add($cert)
$storeRoot.Close()

# Add to Trusted Publishers
$storeTrusted = New-Object System.Security.Cryptography.X509Certificates.X509Store "TrustedPublisher", "LocalMachine"
$storeTrusted.Open("ReadWrite")
$storeTrusted.Add($cert)
$storeTrusted.Close()

Write-Host "✅ Certificate successfully imported into Root and TrustedPublisher."