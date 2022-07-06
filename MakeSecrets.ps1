$ErrorActionPreference = 'stop'

function Main {
    $basePath = '.\src\WikiUpload\Services'
    create_secret "$basePath\Youtube\YoutubeSecrets.cs" `
        $youtubeContent `
        'Edit this file to provide your own google API key.'

    create_secret "$basePath\Passwords\Entropy.cs" `
        $entropyContent `
        'Edit this file to provide your own entropy data.'    
}

function create_secret {
    param ( $path, $content, $message )
    if (-not (Test-Path -Path $path)) {
        Set-Content -Path $path -Value $content
        Write-Host 'Created' -BackgroundColor DarkMagenta -ForegroundColor yellow -NoNewline
        Write-Host " $path" -ForegroundColor cyan
        Write-Host $message -ForegroundColor Gray
        Write-Host ''
    }
}

$youtubeContent = @'
namespace WikiUpload
{
    internal partial class Youtube
    {
        // The API key used to acccess google API (youtube)

        // TODO: Supply a google API key and remove compiler warning
        #warning Google (youtube) API key must be supplied        
        private const string key = "";
    }
}
'@

$entropyContent = @'
namespace WikiUpload
{
    internal partial class Encryption
    {
        // byte array used during password encryption

        // TODO: supply a random series of bytes (any length)
        // and remove warning
        #warning Entropy data nust be supplied 
        private static readonly byte[] entropy =
        {
        };
    }
}
'@

Main
