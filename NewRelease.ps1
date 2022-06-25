$ErrorActionPreference = 'stop'

Import-Module PowerShellForGitHub

. ($PSScriptRoot + '\common.ps1')

function main {
    $repoName = 'Wiki-Up'
    $repoCommittish = 'master'
    $version = read_application_version_from_assemblyinfo
    $newreleaseDetails = @{
        Tag            = "v$version";
        Committish     = $repoCommittish;
        Name           = "Version $version";
        Body           = 'TODO: release description';
        Draft          = $true;
        OwnerName      = 'Aspallar';
        RepositoryName = $repoName;
    }
    Write-Host "Creating release $version"
    $release = New-GitHubRelease @newreleaseDetails
    $release | Format-Table

    $newAssetDetails = @{
        Release        = $release.ID;
        OwnerName      = 'Aspallar';
        RepositoryName = $repoName;
    }

    $progressChar = [char]0x25B6
    Write-Host "Uploading Assets $progressChar" -NoNewline
    $assets = @()
    Get-ChildItem .\Dist | ForEach-Object {
        Write-Host $progressChar -NoNewline -ForegroundColor 'Cyan'
        $assets += (New-GitHubReleaseAsset @newAssetDetails -Path $_.FullName)
    }
    
    Write-Host "`n"
    $MiB = @{Name = 'MiB'; Expression = { [math]::Round($_.Size / 1mb, 2) } }
    $assets | Select-Object ID, Name, Size, $Mib, Content_Type | Format-Table

    $totalSize = ($assets | Measure-Object Size -Sum).Sum
    $totalSize = [math]::Round($totalSize / 1mb, 3)

    Write-Host "Total Asset Size = $totalSize MiB`n"
}

main
