# Reflectieve Duck MCP Server

Welzijns-MCP server voor neurodivergente developers. Gebouwd in C# .NET 10 met OpenIddict OAuth, gedeployed op Fly.io.

**Live:** [reflectieve-duck.fly.dev](https://reflectieve-duck.fly.dev)

## Wat is dit?

Een [Model Context Protocol](https://modelcontextprotocol.io) server die AI-assistenten (Claude, ChatGPT) uitrust met tools voor:

- **Stoplichtplan** (groen/oranje/rood/blauw) — energiemonitoring en kleur-specifieke strategieën
- **Feedback & Reflectie** — ervaringen vastleggen, patronen herkennen, reflectievragen
- **Analyse** — code-analyse, stacktrace-uitleg, sociale situatie-analyse
- **Productiviteit** — focussessies, energielogging, 90-minuten regel
- **Modules** — Theory of Mind, Executieve Functies, Kernkwadranten, IJsberg-analyse, Reflectieve Duck debug-modus
- **Agents** — Autisme Coach, Wellbeing Coach, Debug Duck, Werk Coach, Prikkel Adviseur, Feedback Coach, en meer

## Cijfers

| | Aantal |
|---|---|
| MCP Tools | 45 |
| MCP Prompts/Agents | 15 |
| YAML data bestanden | 9 |
| Bounded contexts | 7 |

## Installatie (lokaal)

```powershell
# Vereist: .NET 10 SDK
.\install-claude.ps1
```

Of handmatig:

```json
{
  "mcpServers": {
    "reflectieve-duck": {
      "command": "dotnet",
      "args": ["run", "--project", "src/ReflectieveDuck.McpServer"]
    }
  }
}
```

## Installatie (remote)

Gebruik de Fly.io deployment:

```json
{
  "mcpServers": {
    "reflectieve-duck": {
      "url": "https://reflectieve-duck.fly.dev/mcp"
    }
  }
}
```

OAuth discovery: `https://reflectieve-duck.fly.dev/.well-known/openid-configuration`

## Architectuur

```
ReflectieveDuck.slnx
├── src/ReflectieveDuck/                    (Class Library — domain logic)
│   ├── Stoplichtplan/                      4 tools
│   ├── Feedback/                           5 tools
│   ├── Reflectie/                          1 tool
│   ├── Analyse/                            2 tools
│   ├── Context/                            4 tools (incl. dashboard)
│   ├── Resources/                          8 tools
│   └── Productiviteit/                     4 tools
│
├── src/ReflectieveDuck.McpServer/          (Web App — MCP server)
│   ├── Tools/                              9 tool klassen (45 tools)
│   ├── Prompts/                            2 prompt klassen (15 prompts)
│   ├── Auth/                               OAuth controllers (DCR + authorize)
│   └── Data/*.yaml                         9 YAML data bestanden
│
├── Dockerfile                              Multi-stage build
├── fly.toml                                Fly.io deployment config
└── .github/workflows/deploy.yml            CI/CD
```

## Data bronnen

| Bestand | Inhoud |
|---------|--------|
| `stoplichtplan.yaml` | 4-kleuren systeem met per-kleur strategieën, SCARF, Leary Roos |
| `ass_wijzer.yaml` | Ingevulde ASS-wijzer psycho-educatie |
| `modules.yaml` | Reflectie-modules (ToM, EF, Kernkwadranten, IJsberg, Duck-modus) |
| `reflectie_vragen.yaml` | 66 reflectievragen per stoplichtkleur |
| `addendum.yaml` | Werkstrategieën, wettelijk kader (WGBH/CZ), rolverdeling |
| `ijsbergmetafoor.yaml` | IJsberg-metafoor: zichtbaar vs. onzichtbaar gedrag |
| `reflectie_assistent.yaml` | AI Reflectie-Assistent handleiding en modules |
| `lifemap.yaml` | Levenstijdlijn 1989-2025 |
| `strengths_profile.yaml` | VIA Character Strengths profiel |

## OAuth

Self-hosted OpenIddict authorization server (in-process):
- Authorization Code + PKCE
- Dynamic Client Registration (DCR) voor Claude.ai en ChatGPT
- Compatibel met Claude Code, Claude Desktop, Claude.ai, ChatGPT MCP en GPT Actions

## Licentie

MIT

## Bron

Stoplichtplan en psycho-educatie gebaseerd op: [tuinstra.family/autisme-stoplicht-werk](https://tuinstra.family/autisme-stoplicht-werk)
