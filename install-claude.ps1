#Requires -Version 5.1
<#
.SYNOPSIS
    Installeert de ReflectieveDuck MCP server in Claude Desktop en/of Claude Code.

.DESCRIPTION
    Dit script:
    1. Controleert of .NET 10 SDK aanwezig is
    2. Bouwt de MCP server (Release)
    3. Genereert .mcp.json in de projectmap
    4. Voegt de server toe aan claude_desktop_config.json (Claude Desktop)
    5. Voegt de server toe aan ~/.claude/settings.json (Claude Code globaal)

.PARAMETER Target
    Doel: ClaudeDesktop, ClaudeCode, of Both (standaard: Both)

.EXAMPLE
    .\install-claude.ps1

.EXAMPLE
    .\install-claude.ps1 -Target ClaudeCode
#>

param(
    [ValidateSet("ClaudeDesktop", "ClaudeCode", "Both")]
    [string]$Target = "Both"
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "  ReflectieveDuck MCP Server - Installatie"          -ForegroundColor Cyan
Write-Host "  Doel: $Target"                                     -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host ""

# --- Stap 1: .NET 10 controleren -------------------------------------------
Write-Host "Stap 1: .NET SDK controleren..." -ForegroundColor Yellow

try {
    $dotnetVersion = & dotnet --version 2>&1
    if ($LASTEXITCODE -ne 0) { throw "dotnet niet gevonden" }

    $major = [int]($dotnetVersion -split '\.')[0]
    if ($major -lt 10) {
        Write-Host "  WAARSCHUWING: .NET $dotnetVersion gevonden, maar .NET 10+ is vereist." -ForegroundColor Yellow
        Write-Host "  Download: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Yellow
        $antwoord = Read-Host "  Toch doorgaan? j=ja, n=nee"
        if ($antwoord -ne 'j') { exit 1 }
    } else {
        Write-Host "  OK: .NET $dotnetVersion" -ForegroundColor Green
    }
} catch {
    Write-Host "  FOUT: .NET SDK niet gevonden. Installeer .NET 10:" -ForegroundColor Red
    Write-Host "  https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Red
    exit 1
}

# --- Stap 2: Project locatie bepalen ----------------------------------------
Write-Host ""
Write-Host "Stap 2: Project locatie..." -ForegroundColor Yellow

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $scriptDir "src\ReflectieveDuck.McpServer"

if (-not (Test-Path (Join-Path $projectPath "ReflectieveDuck.McpServer.csproj"))) {
    Write-Host "  FOUT: Project niet gevonden op: $projectPath" -ForegroundColor Red
    Write-Host "  Voer dit script uit vanuit de project-root." -ForegroundColor Red
    exit 1
}

Write-Host "  OK: $projectPath" -ForegroundColor Green

# --- Stap 3: Bouwen (Release) -----------------------------------------------
Write-Host ""
Write-Host "Stap 3: Project bouwen (dit kan even duren)..." -ForegroundColor Yellow

Push-Location $projectPath
try {
    & dotnet build --configuration Release --nologo -v quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  FOUT: Build mislukt. Controleer de foutmeldingen hierboven." -ForegroundColor Red
        exit 1
    }
    Write-Host "  OK: Build geslaagd" -ForegroundColor Green
} finally {
    Pop-Location
}

# Hulpfunctie: schrijf JSON zonder BOM
function Write-JsonFile {
    param([string]$Path, [object]$Content)
    $json = $Content | ConvertTo-Json -Depth 10
    [System.IO.File]::WriteAllText($Path, $json, [System.Text.UTF8Encoding]::new($false))
}

# Bouw MCP server entry
$projectPathForward = $projectPath -replace '\\', '/'
$dllPath = "$projectPathForward/bin/Release/net10.0/ReflectieveDuck.McpServer.dll"

function New-McpEntry {
    return [PSCustomObject]@{
        command = "dotnet"
        args    = @("exec", $dllPath)
    }
}

$mcpEntry = New-McpEntry

# --- Stap 4: .mcp.json genereren (project-level) ---------------------------
Write-Host ""
Write-Host "Stap 4: .mcp.json genereren in projectmap..." -ForegroundColor Yellow

$mcpJsonPath = Join-Path $scriptDir ".mcp.json"

$mcpJsonEntry = [PSCustomObject]@{
    command = "dotnet"
    args    = @("run", "--project", "src/ReflectieveDuck.McpServer", "--no-build", "--configuration", "Release")
}

$mcpJsonConfig = [PSCustomObject]@{
    mcpServers = [PSCustomObject]@{
        "reflectieve-duck" = $mcpJsonEntry
    }
}

Write-JsonFile -Path $mcpJsonPath -Content $mcpJsonConfig
Write-Host "  OK: $mcpJsonPath" -ForegroundColor Green

# --- Stap 5: Claude Desktop configuratie ------------------------------------
if ($Target -eq "ClaudeDesktop" -or $Target -eq "Both") {
    Write-Host ""
    Write-Host "Stap 5: Claude Desktop configureren..." -ForegroundColor Yellow

    $desktopDir  = Join-Path $env:APPDATA "Claude"
    $desktopFile = Join-Path $desktopDir "claude_desktop_config.json"

    if (-not (Test-Path $desktopDir)) {
        New-Item -ItemType Directory -Path $desktopDir -Force | Out-Null
    }

    if (Test-Path $desktopFile) {
        $backup = $desktopFile + ".backup-" + (Get-Date -Format "yyyyMMdd-HHmmss")
        Copy-Item $desktopFile $backup
        Write-Host "  Backup: $backup" -ForegroundColor Gray
        $config = Get-Content $desktopFile -Raw | ConvertFrom-Json
    } else {
        $config = [PSCustomObject]@{}
    }

    if (-not ($config | Get-Member -Name "mcpServers" -MemberType NoteProperty)) {
        $config | Add-Member -MemberType NoteProperty -Name "mcpServers" -Value ([PSCustomObject]@{})
    }

    if ($config.mcpServers | Get-Member -Name "reflectieve-duck" -MemberType NoteProperty) {
        $config.mcpServers."reflectieve-duck" = $mcpEntry
    } else {
        $config.mcpServers | Add-Member -MemberType NoteProperty -Name "reflectieve-duck" -Value $mcpEntry
    }

    Write-JsonFile -Path $desktopFile -Content $config
    Write-Host "  OK: $desktopFile" -ForegroundColor Green
}

# --- Stap 6: Claude Code configuratie ----------------------------------------
if ($Target -eq "ClaudeCode" -or $Target -eq "Both") {
    Write-Host ""
    Write-Host "Stap 6: Claude Code configureren..." -ForegroundColor Yellow

    $codeDir  = Join-Path $env:USERPROFILE ".claude"
    $codeFile = Join-Path $codeDir "settings.json"

    if (-not (Test-Path $codeDir)) {
        New-Item -ItemType Directory -Path $codeDir -Force | Out-Null
    }

    if (Test-Path $codeFile) {
        $backup = $codeFile + ".backup-" + (Get-Date -Format "yyyyMMdd-HHmmss")
        Copy-Item $codeFile $backup
        Write-Host "  Backup: $backup" -ForegroundColor Gray
        $ccConfig = Get-Content $codeFile -Raw | ConvertFrom-Json
    } else {
        $ccConfig = [PSCustomObject]@{}
    }

    if (-not ($ccConfig | Get-Member -Name "mcpServers" -MemberType NoteProperty)) {
        $ccConfig | Add-Member -MemberType NoteProperty -Name "mcpServers" -Value ([PSCustomObject]@{})
    }

    if ($ccConfig.mcpServers | Get-Member -Name "reflectieve-duck" -MemberType NoteProperty) {
        $ccConfig.mcpServers."reflectieve-duck" = $mcpEntry
    } else {
        $ccConfig.mcpServers | Add-Member -MemberType NoteProperty -Name "reflectieve-duck" -Value $mcpEntry
    }

    Write-JsonFile -Path $codeFile -Content $ccConfig
    Write-Host "  OK: $codeFile" -ForegroundColor Green
}

# --- Klaar -------------------------------------------------------------------
Write-Host ""
Write-Host "====================================================" -ForegroundColor Green
Write-Host "  Installatie voltooid!"                             -ForegroundColor Green
Write-Host "====================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Beschikbare: 45 tools + 15 prompts/agents" -ForegroundColor Cyan
Write-Host "  Stoplicht:       status, geschiedenis, update, vergelijk, tips, codewoord, rolverdeling, volledig"
Write-Host "  Feedback:        toevoegen, lijst, zoeken, statistieken, tags"
Write-Host "  Reflectie:       reflectie_vragen"
Write-Host "  Analyse:         code_analyse, stacktrace_analyse"
Write-Host "  Context:         patronen, inzichten, volledig, dashboard"
Write-Host "  Resources:       ijsberg, addendum, asswijzer, reflectie_assistent, lifemap, sterktes, health, config"
Write-Host "  Productiviteit:  focus_start, focus_stop, energie_log, productiviteit_analyse"
Write-Host "  Wellbeing:       kernkwadrant, emmer_strategieen, emmer_anderen, emmer_triggers, emmer_reactie"
Write-Host "  Modules:         theory_of_mind, executieve_functies, sociale_coherentie, dating, reflectieve_duck,"
Write-Host "                   retrospective, kernkwadrant_analyse"
Write-Host "  Agents:          AutismeCoach, WellbeingCoach, DebugDuck, RetrospectiveFacilitator,"
Write-Host "                   SocialeNavigator, EnergieManager, FeedbackCoach, WerkCoach, PrikkelAdviseur"
Write-Host ""
Write-Host "Database: %LOCALAPPDATA%\ReflectieveDuck\local.db" -ForegroundColor Gray
Write-Host "Project:  $projectPath" -ForegroundColor Gray
Write-Host ""

Write-Host "Volgende stappen:" -ForegroundColor Cyan
if ($Target -eq "ClaudeDesktop" -or $Target -eq "Both") {
    Write-Host "  Claude Desktop:"
    Write-Host "    1. Herstart Claude Desktop"
    Write-Host "    2. Klik op het hamer-icoon (MCP)"
    Write-Host "    3. 'reflectieve-duck' is nu zichtbaar"
}
if ($Target -eq "ClaudeCode" -or $Target -eq "Both") {
    Write-Host "  Claude Code:"
    Write-Host "    1. Start Claude Code opnieuw op"
    Write-Host "    2. Controleer met: /mcp"
}
Write-Host ""

$claudeDesktop = Get-Process -Name "Claude" -ErrorAction SilentlyContinue
if ($claudeDesktop) {
    Write-Host "OPMERKING: Claude Desktop draait - herstart om de MCP server te activeren." -ForegroundColor Yellow
}
