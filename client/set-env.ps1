Write-Output "Generating env-config.js from template..."

# Using -Raw to preserve formatting
(Get-Content -Raw public/env-config.js.template) -replace '\$\{REACT_APP_API_BASE_URL\}', $env:REACT_APP_API_BASE_URL `
                                             -replace '\$\{REACT_APP_USERNAME\}', $env:REACT_APP_USERNAME | 
    Set-Content public/env-config.js

Write-Output "Done."