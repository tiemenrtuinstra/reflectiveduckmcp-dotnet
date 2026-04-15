using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Resources.Infrastructure;

namespace ReflectieveDuck.McpServer.Tools;

/// <summary>
/// Activeert specifieke reflectiemodules uit modules.yaml.
/// Elke module combineert de YAML-data met de situatie-input van de gebruiker.
/// </summary>
[McpServerToolType, Authorize]
public class ReflectieModuleTools
{
    private readonly ResourceProvider _provider;

    public ReflectieModuleTools(ResourceProvider provider) => _provider = provider;

    [McpServerTool(Name = "module_theory_of_mind"),
     Description("Activeer een Theory of Mind reflectie. Helpt om het perspectief van de ander te begrijpen bij een specifieke situatie. Geeft reflectievragen en aanpak.")]
    public string TheoryOfMind(
        [Description("Beschrijf de situatie waarin je het perspectief van de ander wilt begrijpen")] string situatie)
    {
        var module = _provider.GetModule("theoryOfMind");
        return $"""
            # Theory of Mind Reflectie

            ## Situatie
            {situatie}

            ## Module-inhoud
            {module}
            """;
    }

    [McpServerTool(Name = "module_executieve_functies"),
     Description("Activeer een Executieve Functies hulpsessie. Helpt bij planning, taakorganisatie, prioritering en het schakelen tussen activiteiten. Geef een taak of probleem mee.")]
    public string ExecutieveFuncties(
        [Description("De taak of het probleem waarbij je hulp nodig hebt met planning/organisatie")] string taak)
    {
        var module = _provider.GetModule("executieveFuncties");
        return $"""
            # Executieve Functies Hulp

            ## Taak
            {taak}

            ## Module-inhoud
            {module}
            """;
    }

    [McpServerTool(Name = "module_sociale_coherentie"),
     Description("Activeer een Sociale Coherentie reflectie. Helpt bij het begrijpen van sociale situaties door patroon- en betekenisherkenning. Beschrijf de sociale situatie.")]
    public string SocialeCoherentie(
        [Description("Beschrijf de sociale situatie die je wilt begrijpen")] string situatie)
    {
        var module = _provider.GetModule("socialeCoherentie");
        return $"""
            # Sociale Coherentie Reflectie

            ## Situatie
            {situatie}

            ## Module-inhoud
            {module}
            """;
    }

    [McpServerTool(Name = "module_dating"),
     Description("Activeer de Dating & Communicatie module. Analyseer een gesprek of interactie en krijg hulp bij communicatiepatronen, emotionele reacties en sociale intuïtie.")]
    public string DatingCommunicatie(
        [Description("Beschrijf de dating-interactie of het gesprek dat je wilt analyseren")] string interactie)
    {
        var module = _provider.GetModule("dating");
        return $"""
            # Dating & Communicatie Analyse

            ## Interactie
            {interactie}

            ## Module-inhoud
            {module}
            """;
    }

    [McpServerTool(Name = "module_reflectieve_duck"),
     Description("Activeer de Reflectieve Duck modus voor technische problemen. Helpt bij het structureren van probleemoplossing, doorbreekt tunnelvisie en kanalisert hyperfocus. Kies submodus: debug, projectstart of reflectie.")]
    public string ReflectieveDuckModus(
        [Description("Het technische probleem of de context")] string probleem,
        [Description("Submodus: debug, projectstart of reflectie (standaard: debug)")] string modus = "debug")
    {
        var duckModule = _provider.GetModule("reflectieveDuck");
        var submodus = _provider.GetModule($"reflectieveDuck");

        return $"""
            # Reflectieve Duck Modus

            ## Probleem
            {probleem}

            ## Gekozen submodus: {modus}

            ## Module-inhoud
            {duckModule}
            """;
    }

    [McpServerTool(Name = "module_retrospective"),
     Description("Genereer een werkretrospective template. Output in Posh British English (zoals afgesproken in de Reflectie-Assistent). Geef de sprintperiode of context mee.")]
    public string Retrospective(
        [Description("Sprint of periode (bijv. 'Sprint 42' of 'afgelopen week')")] string periode = "afgelopen sprint")
    {
        return $"""
            # Sprint Retrospective — {periode}

            *In refined British English, as per the Reflective Duck protocol.*

            ## Team Compliments
            "Splendid teamwork this sprint, particularly on..."

            ## Continue Doing
            - What practices served us well?
            - Which rituals provided stability and predictability?

            ## Start Doing
            - What could improve our workflow?
            - Are there accommodations we should formalise?

            ## Stop Doing
            - What caused unnecessary friction or overload?
            - Which interruptions could we prevent?

            ## Other Observations
            - Sensory environment: were there issues?
            - Communication clarity: where could we be more explicit?
            - Energy management: did we respect the 90-minute focus blocks?

            ## Kernkwadrant Check (uit modules.yaml)
            {_provider.GetKernkwadranten()}

            ## Actie-items
            1. ...
            2. ...
            3. ...
            """;
    }

    [McpServerTool(Name = "module_kernkwadrant_analyse"),
     Description("Analyseer een specifieke situatie met het Kernkwadrantenmodel van Ofman. Ontdek welke kernkwaliteit speelde, waar die doorsloeg (valkuil), wat de groeirichting is (uitdaging) en wat je triggerde (allergie). Werkt voor werk-, sociale en persoonlijke situaties.")]
    public string KernkwadrantAnalyse(
        [Description("Beschrijf de situatie die je wilt analyseren")] string situatie,
        [Description("Context: werk, sociaal, persoonlijk (optioneel)")] string? context = null)
    {
        var contextLabel = context?.ToLowerInvariant() switch
        {
            "werk" => "Werkcontext",
            "sociaal" => "Sociale context",
            "persoonlijk" => "Persoonlijke context",
            _ => "Situatie"
        };

        var module = _provider.GetKernkwadranten();

        return $"""
            # Kernkwadrant Analyse — {contextLabel}

            ## Situatie
            {situatie}

            ## Kernkwadranten-data (uit modules.yaml)
            {module}

            ## Verbinding met je stoplichtplan
            - Welke kleur had je toen dit gebeurde?
            - Had je emmerniveau invloed op hoe heftig je reageerde?
            - Zou je anders gereageerd hebben in het groen?

            ## Samenvatting invullen
            ```
            Kernkwaliteit:  [...]
            Valkuil:        [...] (waar sloeg het door?)
            Uitdaging:      [...] (wat is de groeirichting?)
            Allergie:       [...] (wat triggerde je bij de ander?)
            Stoplichtkleur: [...] (had dit invloed?)
            Leerpunt:       [...] (wat neem je mee?)
            ```
            """;
    }

    [McpServerTool(Name = "module_ijsberg_analyse"),
     Description("Analyseer een situatie met de IJsberg-metafoor. Ontdek wat zichtbaar was (gedrag) vs. wat onzichtbaar speelde (prikkels, energie, emoties, overtuigingen, sociale dynamiek). Helpt om misinterpretatie te voorkomen.")]
    public string IjsbergAnalyse(
        [Description("Beschrijf de situatie die je wilt analyseren")] string situatie,
        [Description("Stoplichtkleur op dat moment: groen/oranje/rood/blauw (optioneel)")] string? kleur = null)
    {
        var kleurContext = kleur?.ToLowerInvariant() switch
        {
            "groen" => "Op groen: actieve deelname, vriendelijke reacties, georganiseerde werkplek.",
            "oranje" => "Op oranje: zuchten, korte antwoorden, minder lachen, vaker weg van bureau.",
            "rood" => "Op rood: terugtrekking, Teams offline, korte opmerkingen, wegloop uit meetings.",
            "blauw" => "Op blauw: trager tempo, stiller in meetings, vaker inchecken bij collega's.",
            _ => "Kleur onbekend — probeer te bepalen welke kleur je had op dat moment."
        };

        var module = _provider.GetModule("ijsbergAnalyse");

        return $"""
            # IJsberg-Analyse

            ## Situatie
            {situatie}

            ## Jouw stoplichtkleur
            {kleurContext}

            ## Analyse-structuur (uit modules.yaml)
            {module}
            """;
    }
}
