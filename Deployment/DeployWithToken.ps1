param (
    [Parameter(Mandatory=$true)]
    [string]$Token
)

Write-Host "Creating Kubernetes secret 'discogs-token'..."
kubectl create secret generic discogs-token --from-literal=token="$Token" --dry-run=client -o yaml | kubectl apply -f -

Write-Host "Running BuildAndDeployClient.ps1..."
& "$PSScriptRoot\BuildAndDeployClient.ps1"

Write-Host "Running BuildAndDeployServer.ps1..."
& "$PSScriptRoot\BuildAndDeployServer.ps1"

Write-Host "Deployment complete."