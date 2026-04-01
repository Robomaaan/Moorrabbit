#!/usr/bin/env pwsh
# Setup script for Moorhase Game Images

Write-Host "🐰 Moorhase Game - Image Setup Wizard" -ForegroundColor Magenta
Write-Host ""

$assetsPath = ".\Moorhase\Assets"

# Create Assets folder
if (-not (Test-Path $assetsPath)) {
    Write-Host "📁 Erstelle Assets-Ordner..." -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $assetsPath -Force | Out-Null
    Write-Host "✅ Assets-Ordner erstellt: $assetsPath" -ForegroundColor Green
} else {
    Write-Host "✅ Assets-Ordner existiert bereits: $assetsPath" -ForegroundColor Green
}

# Check for images
$grassPath = Join-Path $assetsPath "Grass.png"
$hasenPath = Join-Path $assetsPath "Hase.png"

Write-Host ""
Write-Host "🖼️  Bilder-Status:" -ForegroundColor Cyan

if (Test-Path $grassPath) {
    Write-Host "✅ Grass.png vorhanden" -ForegroundColor Green
} else {
    Write-Host "❌ Grass.png fehlt - Bitte kopieren Sie es in: $grassPath" -ForegroundColor Yellow
}

if (Test-Path $hasenPath) {
    Write-Host "✅ Hase.png vorhanden" -ForegroundColor Green
} else {
    Write-Host "❌ Hase.png fehlt - Bitte kopieren Sie es in: $hasenPath" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🔧 Nächste Schritte:" -ForegroundColor Cyan
Write-Host "1. Kopieren Sie Ihre PNG-Dateien in: $assetsPath" -ForegroundColor White
Write-Host "2. Führe aus: dotnet build" -ForegroundColor White
Write-Host "3. Führe aus: dotnet run" -ForegroundColor White

Write-Host ""
Write-Host "📝 Hinweis: Das Spiel funktioniert auch ohne Bilder (mit Fallback-Farben)" -ForegroundColor Yellow
