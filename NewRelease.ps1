param (
    [Parameter()]
    [string]
    $RepositoryName = 'Wiki-Up',

    [Parameter()]
    [string]
    $Branch = 'master',

    [Parameter()]
    [string]
    $Owner = 'Aspallar'
)
$ErrorActionPreference = 'stop'

Import-Module PowerShellForGitHub

. ($PSScriptRoot + '\common.ps1')

function main {
    Write-Host "Creating release $version"
    $release = create_new_release
    $assets = upload_assets $release.Id
    display_assets $assets
}

function create_new_release {
    $version = read_application_version_from_assemblyinfo
    $newreleaseDetails = @{
        Tag            = "v$version"
        Committish     = $Branch
        Name           = "Version $version"
        Body           = 'TODO: release description'
        Draft          = $true
        OwnerName      = $Owner
        RepositoryName = $RepositoryName
    }
    $release = New-GitHubRelease @newreleaseDetails
    $release | Format-Table | Out-Host
    $release
}

function display_assets {
    param ([Parameter(Mandatory)] [PSCustomObject[]] $assets)
    
    Write-Host "`n"
    $MiB = @{Name = 'MiB'; Expression = { [math]::Round($_.Size / 1mb, 2) } }
    $assets | Select-Object ID, Name, Size, $Mib, Content_Type | Format-Table | Out-Host

    $totalSize = ($assets | Measure-Object Size -Sum).Sum
    $totalSize = [math]::Round($totalSize / 1mb, 3)

    Write-Host "Total Asset Size = $totalSize MiB`n"
}

function upload_assets {
    param ([Parameter(Mandatory)] [long] $releaseId)
    $newAssetDetails = @{
        Release        = $releaseId
        OwnerName      = $Owner
        RepositoryName = $RepositoryName
    }
    $progressChar = [char]0x25B6
    Write-Host "Uploading Assets $progressChar" -NoNewline

    $assets = @()
    Get-ChildItem .\Dist | ForEach-Object {
        Write-Host $progressChar -NoNewline -ForegroundColor 'Cyan'
        $assets += (New-GitHubReleaseAsset @newAssetDetails -Path $_.FullName)
    }
    $assets
}

main
