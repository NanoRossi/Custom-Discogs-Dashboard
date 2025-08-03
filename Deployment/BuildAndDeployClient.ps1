$startDir = Get-Location

Write-Host "Starting client deployment..."

Set-Location ..\client
Write-Host "Building Docker image..."
docker build -t discogs-client -f Dockerfile .

Set-Location ..\Deployment\Charts
Write-Host "Uninstalling Helm release..."
helm uninstall discogs-client

# give helm time to clean up components like pvc etc
Start-Sleep -Seconds 3

Write-Host "Installing Helm release..."
helm install discogs-client discogs-client

Set-Location $startDir
Write-Host "Deployment complete."