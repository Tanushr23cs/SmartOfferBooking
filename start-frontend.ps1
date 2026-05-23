# Starts Vite dev server (proxies API to http://localhost:5000).
$ErrorActionPreference = "Stop"
$root = Join-Path $PSScriptRoot "frontend"

if (-not (Test-Path (Join-Path $root "node_modules"))) {
    Write-Host "Installing npm packages..." -ForegroundColor Cyan
    Set-Location $root
    npm install
}

Write-Host "Starting frontend at http://localhost:5173" -ForegroundColor Cyan
Write-Host "Ensure the API is running (run start-backend.ps1 in another terminal)." -ForegroundColor Yellow

Set-Location $root
npm run dev
