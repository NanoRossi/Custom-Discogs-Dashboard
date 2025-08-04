$startDir = Get-Location

Write-Host "Starting Server deployment..."

Set-Location ..\server
Write-Host "Building Docker image..."
docker build -t discogs-server -f Dockerfile .

Set-Location ..\Deployment\Charts
Write-Host "Uninstalling Helm release..."
helm uninstall discogs-server

# give helm time to clean up components like pvc etc
Start-Sleep -Seconds 3

Write-Host "Installing Helm release..."
helm install discogs-server discogs-server

Set-Location $startDir
Write-Host "Server Deployment complete."