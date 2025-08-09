param (
    [Parameter(Mandatory=$false)]
    [string]$Token
)

if ($Token) { 
    Write-Host "Creating Kubernetes secret 'discogs-token'..."
    kubectl create secret generic discogs-token --from-literal=token="$Token" --dry-run=client -o yaml | kubectl apply -f -
}
else
{
    Write-Host "Token not provided - skipping Kubernetes secret creation."
}

Write-Host "Running BuildAndDeployClient.ps1..."
& "$PSScriptRoot\BuildAndDeployClient.ps1"

Write-Host "Running BuildAndDeployServer.ps1..."
& "$PSScriptRoot\BuildAndDeployServer.ps1"

Write-Host "Deployment complete."