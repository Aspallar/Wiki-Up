$ErrorActionPreference = 'stop'

Import-Module PowerShellForGitHub

. ((Split-Path $MyInvocation.MyCommand.Path -Parent) + '\common.ps1')

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
    $assets |
        Format-Table `
        @{Name = 'ID'; Expression = { $_.ID } }, 
        @{Name = 'Name'; Expression = { $_.Name } },
        @{Name = 'Size'; Expression = { $_.Size } },
        @{Name = 'Content Type'; Expression = { $_.Content_Type } }
    
    $totalSize = ($assets | Measure-Object Size -Sum).Sum
    $totalSize = [math]::Round($totalSize / 1mb, 3)

    Write-Host "Total Asset Size = $totalSize MiB`n"
}

main
