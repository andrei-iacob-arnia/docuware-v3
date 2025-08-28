# List any print-related channels
wevtutil el | findstr /i print

# Appx deployment logs (these always exist—use them now)
Get-WinEvent -LogName Microsoft-Windows-AppxDeploymentServer/Operational -MaxEvents 60 |
  Select TimeCreated, Id, LevelDisplayName, Message | Format-List

# If PrintWorkflow channel is missing, try to locate its manifest:
Get-ChildItem "$env:SystemRoot\System32\winevt\Manifests\*PrintWorkflow*.man" -ErrorAction SilentlyContinue

# If none found, search WinSxS for it:
$wf = Get-ChildItem "$env:SystemRoot\WinSxS" -Filter *PrintWorkflow*.man -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
$wf.FullName

# If you found one, register it (elevated):
if ($wf) { wevtutil im "$($wf.FullName)" }

# Enable useful channels
wevtutil sl Microsoft-Windows-AppxDeploymentServer/Operational /e:true
wevtutil sl Microsoft-Windows-PrintService/Operational /e:true
# (Once PrintWorkflow exists)
# wevtutil sl Microsoft-Windows-PrintWorkflow/Operational /e:true
