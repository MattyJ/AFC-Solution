$username = "Matthew.Jordan@FujitsuUKI.onmicrosoft.com"
$password = "##PASSWORD##!"
$siteAdminURL = "https://fujitsuuki-admin.sharepoint.com/"
Import-Module Microsoft.Online.SharePoint.PowerShell –DisableNameChecking
$securePassword = ConvertTo-SecureString $password –AsPlainText –force
$O365Credential = New-Object System.Management.Automation.PsCredential($username, $securePassword)
Connect-SPOService -Url $siteAdminUrl -Credential $O365Credential
 
Get-SPODeletedSite | foreach {
Write-host "Deleting " $_.Url
Remove-SPODeletedSite –Identity $_.Url –Confirm:$false
}
Disconnect-SPOService