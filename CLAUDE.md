# CLAUDE.md — ReflectieveDuck MCP Server

## Wat is dit project?
Een MCP (Model Context Protocol) server voor neurodivergent welzijn, gebouwd in C# .NET 10.
Het biedt tools, prompts en agent-rollen voor zelfreflectie, energiebeheer en werkondersteuning.

## Architectuur
- **Solution**: `ReflectieveDuck.slnx` met 2 projecten
- **ReflectieveDuck** (class library): Domain logic, 7 bounded contexts, EF Core + SQLite
- **ReflectieveDuck.McpServer** (web app): MCP tools, prompts, OAuth, HTTP endpoints
- **Patroon**: DDD met CQRS-lite (`IQueryHandler<TQuery, TResult>`)

## Bounded contexts
Stoplichtplan, Feedback, Reflectie, Analyse, Context, Resources, Productiviteit

## Data
Alle content staat in YAML bestanden in `src/ReflectieveDuck.McpServer/Data/*.yaml` (9 bestanden).
Deze worden als EmbeddedResource ingebakken in de DLL. Geen hardcoded strings in C#.

## OAuth
OpenIddict in-process: Authorization Code + PKCE + DCR. Keys via Data Protection + ephemeral JWKS.
DCR endpoint beveiligd met `DCR_TOKEN` environment variable.

## Deployment
- **Fly.io**: `reflectieve-duck.fly.dev` (Amsterdam, 1 CPU, 1GB RAM, 1GB SQLite volume)
- **CI/CD**: `.github/workflows/deploy.yml` (push to main → fly deploy)
- **Lokaal**: `dotnet run --project src/ReflectieveDuck.McpServer` (stdio transport)

## Conventies
- Nederlands in tool-namen en beschrijvingen
- YAML voor alle content (geen hardcoded strings)
- `[Authorize]` op alle tool klassen (behalve health/landing page)
- `[McpServerToolType]` + `[McpServerTool(Name = "...")]` voor tools
- `[McpServerPromptType]` + `[McpServerPrompt]` voor prompts
- Elke bounded context heeft een `*ServiceExtensions.cs` voor DI registratie

## Testen
Geen unit tests (nog). Build check: `dotnet build`. Runtime test: `dotnet run` + MCP client.

## Belangrijke bestanden
- `Program.cs` — composition root, OAuth setup, HTTP endpoints
- `ResourceProvider.cs` — leest embedded YAML, YAML sectie-extractie
- `VraagGenerator.cs` — laadt reflectie_vragen.yaml via YamlDotNet
- `modules.yaml` — alle reflectie-module templates (ToM, EF, Duck, Kernkwadrant, IJsberg)
- `stoplichtplan.yaml` — het volledige stoplichtplan per kleur
- `ass_wijzer.yaml` — de ingevulde psycho-educatie ASS-wijzer
