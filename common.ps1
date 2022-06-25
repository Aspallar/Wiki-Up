function get_supported_languages {
    # Supported languages, The default (english) must
    # be at index 0. The others can be in any order
    @(
        @{ folder = 'en-US'; name = 'English' }
        @{ folder = 'de-DE'; name = 'Deutsch' }
        @{ folder = 'et-EE'; name = 'Eesti' }
        @{ folder = 'fr-FR'; name = 'Francais' }
    )
}

function extract_assembly_version {
    param ( [Parameter(Mandatory = $true)] [string] $assemblyInfoPath )
    $version = Select-String -Path $assemblyInfoPath `
        -Pattern '^\[assembly: AssemblyVersion.+?([0-9]+\.[0-9]+\.[0-9]+)'
    if ($version.Length -eq 0 ) {
        throw "Unable to determine version from $assemblyInfoPath"
    }
    $version.Matches[0].Groups[1]
}

function is_project_root {
    return ((Test-Path .\.git -PathType Container) -and `
        (Test-Path .\src -PathType Container))
}

function read_application_version_from_assemblyinfo {
    extract_assembly_version  '.\src\WikiUpload\Properties\AssemblyInfo.cs'
}
