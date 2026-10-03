$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
Set-Location -LiteralPath $projectRoot
gh auth status
if ($LASTEXITCODE -ne 0) { throw 'ACCIÓN MANUAL PENDIENTE: autenticar GitHub CLI con gh auth login' }
$owner = gh api user --jq .login
if ($LASTEXITCODE -ne 0) { throw 'No se pudo consultar la cuenta autenticada.' }
$remote = "https://github.com/$owner/last-key-game.git"
gh repo view "$owner/last-key-game" --json name 2>$null
if ($LASTEXITCODE -ne 0) {
    gh repo create "$owner/last-key-game" --private --description 'LAST KEY: competencia de mecanografía por rondas, Unity, Quaternius CC0'
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo crear last-key-game.' }
}
$origin = git remote get-url origin 2>$null
if ($LASTEXITCODE -eq 0 -and $origin -ne $remote) { throw 'Origin apunta a otro repositorio; se evita modificarlo.' }
if (-not $origin) { git remote add origin $remote }
git lfs install --local
git push -u origin main
if ($LASTEXITCODE -ne 0) { throw 'Push incompleto; revisar autenticación/permisos.' }
