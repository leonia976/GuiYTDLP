########################### by AL
$ErrorActionPreference = "Stop"

$BinDir = ".\Bin"
$TempDir = "$BinDir\temp_bin"

Write-Host "1. Cleaning the Bin folder..." -ForegroundColor Cyan
if (Test-Path $BinDir) {
    Remove-Item -Path "$BinDir\*" -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $TempDir | Out-Null

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Write-Host "2. Downloading aria2c..." -ForegroundColor Cyan
Invoke-WebRequest "https://github.com/aria2/aria2/releases/download/release-1.37.0/aria2-1.37.0-win-32bit-build1.zip" -OutFile "$TempDir\aria2.zip"
Expand-Archive -Path "$TempDir\aria2.zip" -DestinationPath $TempDir -Force
Move-Item -Path "$TempDir\aria2-1.37.0-win-32bit-build1\aria2c.exe" -Destination $BinDir -Force

Write-Host "3. Downloading packages with aria2c..." -ForegroundColor Cyan
& "$BinDir\aria2c.exe" -x 16 -s 16 --allow-overwrite=true "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip" -d $TempDir -o "ffmpeg.zip"
& "$BinDir\aria2c.exe" -x 16 -s 16 --allow-overwrite=true "https://github.com/denoland/deno/releases/latest/download/deno-x86_64-pc-windows-msvc.zip" -d $TempDir -o "deno.zip"
& "$BinDir\aria2c.exe" -x 16 -s 16 --allow-overwrite=true "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyWindows.zip" -d $TempDir -o "AtomicParsley.zip"
& "$BinDir\aria2c.exe" -x 16 -s 16 --allow-overwrite=true "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe" -d $BinDir -o "yt-dlp.exe"

Write-Host "4. Extracting archives..." -ForegroundColor Cyan
Expand-Archive -Path "$TempDir\ffmpeg.zip" -DestinationPath $TempDir -Force
Expand-Archive -Path "$TempDir\deno.zip" -DestinationPath $BinDir -Force
Expand-Archive -Path "$TempDir\AtomicParsley.zip" -DestinationPath $BinDir -Force
Move-Item -Path "$TempDir\ffmpeg-master-latest-win64-gpl\bin\*" -Destination $BinDir -Force

Write-Host "5. Final cleaning..." -ForegroundColor Cyan
Remove-Item -Path $TempDir -Recurse -Force

Write-Host "Update completed successfully!" -ForegroundColor Green


