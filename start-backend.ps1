# Starts the .NET API on http://localhost:5000 (required for the Vite frontend proxy).
$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host "Starting Smart Offer Booking API on http://localhost:5000 ..." -ForegroundColor Cyan
Write-Host "Keep this window open. In another terminal run: cd frontend; npm run dev" -ForegroundColor Yellow
Write-Host ""

Set-Location $root
& dotnet run --project "src\SmartOfferBooking.API\SmartOfferBooking.API.csproj" --launch-profile http
