param (
    # Clean intermediate files before building
    [Parameter()]
    [switch]
    $Clean,

    # Build the Dist folder which contains zip and installers.
    [Parameter()]
    [switch]
    $Distribution
)

$ErrorActionPreference = 'stop'

. ($PSScriptRoot + '\common.ps1')

function main {
    $languages = get_supported_languages
    check_prerequisites
    ensure_msbuild
    if ($Clean.IsPresent) { clean_build }
    $version = read_application_version_from_assemblyinfo
    build_application $Distribution.IsPresent
    create_deploy_folder $languages
    copy_application_files_to_deploy_folder $languages
    if ($Distribution.IsPresent) { build_distribution $version $languages }
}

function check_prerequisites {
    if (-not (is_project_root)) {
        throw 'Script must be run from project root'
    }
}

function ensure_msbuild {
    try {
        Get-Command msbuild.exe | Out-Null
    }
    catch {
        try {
            Start_VisualStudio_Environment
        }
        catch {
            throw 'Microsoft Build (msbuild.exe) must be in the path to run this script.'
        }
    }

}

function clean_build {
    if ((delete_obj_and_bin_folders)) {
        Write-Host 'Project cleaned - continuing with build.'
    }
    else {
        throw ' - Build cancelled'
    }
}

function build_application {
    param ( [Parameter(Mandatory)] [bool] $buildInstallers )
    if ($buildInstallers) { $config = 'Install' } else { $config = 'Release' }
    Push-Location -Path .\src
    try {
        msbuild /m /v:m /p:Configuration=$config /nologo
    }
    finally {
        Pop-Location
    }
}

function create_deploy_folder {
    param ( [Parameter(Mandatory)] [hashtable[]] $languages )
    New-Item .\Deploy -ItemType Directory -Force | Out-Null
    get_language_folders $languages | ForEach-Object {
        New-Item -Path ".\Deploy\$($_)" -ItemType Directory -Force | Out-Null
    }
    # Delete any files in the deploy folders
    Get-ChildItem .\Deploy\* -Recurse -File | ForEach-Object {
        Remove-Item -Path $_.FullName -Force
    }
}

function copy_application_files_to_deploy_folder {
    param ( [Parameter(Mandatory)] [hashtable[]] $languages )
    $src = '.\src\WikiUpload\bin\release'
    $applicationFiles = '*.dll', 'wikiup.exe', 'wikiup.exe.config'
    $excludedFiles = 'nunit*'
    Copy-Item -path $src\* `
        -Include $applicationFiles `
        -Exclude $excludedFiles `
        -Destination .\Deploy

    get_language_folders $languages | ForEach-Object {
        Copy-Item -Path "$src\$_\WikiUp.resources.dll" -Destination ".\Deploy\$_" 
    }
}

function get_language_folders {
    param ( [Parameter(Mandatory)] [hashtable[]] $languages )
    $languages | Select-Object -Skip 1 -ExpandProperty folder
}

function build_distribution {
    param (
        [Parameter(Mandatory)] [string] $version,
        [Parameter(Mandatory)] [hashtable[]] $languages
    )
    create_dist_folder
    create_zip_from_deploy_folder $version
    copy_installers_to_dist_folder $version $languages
}


function delete_obj_and_bin_folders {
    $continue = $true
    $cleanFolderNames = 'obj', 'bin'
    $folders = Get-ChildItem -Path .\src -Recurse -Directory | Where-Object {
        $cleanFolderNames -contains $_.Name
    }
    if ($folders.Count -gt 0) {
        Write-Host "`nGoing to remove the following folders" -ForegroundColor Cyan
        $folders.ForEach({ Write-Host $_.FullName })
        Write-Host "`nPress enter to continue any other key to cancel:"
        $ch = [Console]::ReadKey()
        if ($ch.Key -eq 'Enter') {
            $folders.ForEach({ Remove-Item -Path $_.FullName -Recurse })
        }
        else {
            $continue = $false
        }
    }
    return $continue
}

function create_zip_from_deploy_folder {
    param ( [Parameter(Mandatory)] [string] $version )
    Compress-Archive -Path .\deploy\* `
        -DestinationPath ".\Dist\Wiki-Up-$version.zip"
}


function create_dist_folder {
    New-Item .\Dist -ItemType Directory -Force | Out-Null
    Remove-Item -Path .\Dist\* -Force
}

function copy_installers_to_dist_folder {
    param (
        [Parameter(Mandatory)] [string] $version,
        [Parameter(Mandatory)] [hashtable[]] $languages
    )
    $installerSrc = '.\src\WikiUpInstaller\bin\Release'
    foreach ($lang in $languages) {
        Copy-Item -Path "$installerSrc\$($lang.folder)\WikiUpInstaller.msi" `
            -Destination ".\Dist\WikiUpInstaller-$($lang.name)-$version.msi" 
    }
}

main
