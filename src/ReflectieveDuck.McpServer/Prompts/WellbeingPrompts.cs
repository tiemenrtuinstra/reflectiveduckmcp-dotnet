using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace ReflectieveDuck.McpServer.Prompts;

/// <summary>
/// MCP Prompts voor wellbeing sessies — conversation starters die de AI-client
/// kan aanbieden aan de gebruiker als snelkoppelingen.
/// </summary>
[McpServerPromptType]
public sealed class WellbeingPrompts
{
    [McpServerPrompt, Description("Start een complete emmer-check: hoe vol is je emmer, welke triggers spelen, en welke strategieën helpen nu.")]
    public static IEnumerable<ChatMessage> EmmerCheck()
    {
        yield return new ChatMessage(ChatRole.User, """
            Doe een complete emmer-check met mij. Gebruik de beschikbare tools:

            1. Haal mijn huidige stoplicht-status op (stoplicht_status)
            2. Bekijk mijn recente feedback (feedback_lijst, limit: 5)
            3. Analyseer patronen (context_patronen)
            4. Op basis van mijn kleur: haal de relevante tips op (stoplicht_tips)
            5. Toon de transitie-codewoorden (stoplicht_codewoord)

            Geef een samenvatting van hoe het gaat en concrete aanbevelingen.
            Houd het rustig, gestructureerd en in korte alinea's.
            Als ik op oranje of rood sta, bied dan direct de mindfulness tips aan.
            """);
    }

    [McpServerPrompt, Description("Start een reflectiesessie afgestemd op je huidige stoplichtkleur met gerichte vragen en tips.")]
    public static IEnumerable<ChatMessage> ReflectieSessie(
        [Description("Stoplichtkleur: groen, oranje, rood of blauw (optioneel)")] string? kleur = null)
    {
        var kleurFilter = kleur is not null ? $"Mijn kleur is {kleur}." : "Haal eerst mijn huidige kleur op via stoplicht_status.";

        yield return new ChatMessage(ChatRole.User, $"""
            Begeleid mij door een reflectiesessie. {kleurFilter}

            1. Genereer reflectievragen passend bij mijn kleur (reflectie_vragen)
            2. Haal de zelfondersteuning-tips op voor mijn kleur (stoplicht_tips)
            3. Stel mij de vragen één voor één, wacht op mijn antwoord
            4. Na elke 2-3 vragen: geef een korte observatie
            5. Sluit af met een samenvatting en één concrete actie

            Communiceer rustig, in korte zinnen, zonder overprikkeling.
            Gebruik het format: korte samenvatting → blokken → reflectievraag.
            """);
    }

    [McpServerPrompt, Description("Start een dagelijkse check-in: energie loggen, status bijwerken, en kort reflecteren.")]
    public static IEnumerable<ChatMessage> DagelijkseCheckIn()
    {
        yield return new ChatMessage(ChatRole.User, """
            Doe een dagelijkse check-in met mij:

            1. Vraag naar mijn energielevel (0-100) en log het (energie_log)
            2. Bepaal de bijbehorende stoplichtkleur en werk de status bij (stoplicht_update)
            3. Geef 2-3 reflectievragen passend bij mijn kleur (reflectie_vragen)
            4. Laat mij kort antwoorden
            5. Sla mijn antwoorden op als feedback (feedback_toevoegen) met relevante tags

            Houd het kort — maximaal 5 minuten. Dit is een check-in, geen therapiesessie.
            Gebruik het stoplicht-codewoord als ik op oranje of rood sta.
            """);
    }

    [McpServerPrompt, Description("Analyseer een stressvolle werksituatie met het SCARF-model en de Leary Roos.")]
    public static IEnumerable<ChatMessage> ScrafAnalyse(
        [Description("Beschrijf kort de werksituatie")] string situatie = "")
    {
        yield return new ChatMessage(ChatRole.User, $"""
            Analyseer deze werksituatie met het SCARF-model en de Leary Roos:

            Situatie: {(string.IsNullOrWhiteSpace(situatie) ? "[Ik beschrijf hem zodadelijk]" : situatie)}

            Gebruik de tools:
            1. Haal het addendum op (resource_addendum) voor de SCARF en Leary definities
            2. Haal mijn huidige stoplichtkleur op (stoplicht_status)

            Analyseer per SCARF-domein:
            - Status: wordt mijn eigenwaarde bedreigd?
            - Certainty: is er onzekerheid of onvoorspelbaarheid?
            - Autonomy: verlies ik controle over mijn werk?
            - Relatedness: voel ik me verbonden of buitengesloten?
            - Fairness: is de situatie eerlijk?

            Geef op basis van mijn stoplichtkleur aan welke domeinen het meest kwetsbaar zijn.
            Toon de Leary Roos positie en wat anderen kunnen doen.
            Sluit af met concrete stappen.
            """);
    }

    [McpServerPrompt, Description("Genereer een compleet weekoverzicht: productiviteit, energie, patronen en aanbevelingen.")]
    public static IEnumerable<ChatMessage> Weekoverzicht()
    {
        yield return new ChatMessage(ChatRole.User, """
            Maak een weekoverzicht voor mij:

            1. Haal de productiviteitsanalyse op (productiviteit_analyse, 7 dagen)
            2. Bekijk de stoplichtgeschiedenis (stoplicht_geschiedenis, 14)
            3. Analyseer patronen (context_patronen)
            4. Haal inzichten op (context_inzichten)
            5. Bekijk de feedback statistieken (feedback_statistieken)

            Stel een overzicht samen met:
            - Energie-trend over de week
            - Meest productieve momenten
            - Terugkerende thema's of patronen
            - Kernkwadrant-observatie (waar schoot perfectionnisme door?)
            - 3 concrete aanbevelingen voor volgende week

            Houd het positief en constructief. Begin met wat goed ging.
            """);
    }

    [McpServerPrompt, Description("Analyseer een situatie met zowel de IJsberg-metafoor als het Kernkwadrantenmodel. Combineert zichtbaar/onzichtbaar gedrag met kwaliteit/valkuil/uitdaging/allergie.")]
    public static IEnumerable<ChatMessage> DiepeAnalyse(
        [Description("Beschrijf kort de situatie")] string situatie = "")
    {
        yield return new ChatMessage(ChatRole.User, $"""
            Doe een diepe analyse van deze situatie met twee modellen:

            Situatie: {(string.IsNullOrWhiteSpace(situatie) ? "[Ik beschrijf hem zodadelijk]" : situatie)}

            Gebruik de tools in deze volgorde:

            1. Check mijn stoplichtkleur (stoplicht_status)
            2. Doe een IJsberg-analyse (module_ijsberg_analyse) — wat was zichtbaar vs. onzichtbaar
            3. Doe een Kernkwadrant-analyse (module_kernkwadrant_analyse) — welke kwaliteit/valkuil speelde
            4. Haal mijn triggers op (emmer_triggers) om te checken of bekende prikkels meespeelden
            5. Check het SCARF-model via het addendum (resource_addendum)

            Combineer de analyses tot:
            - Wat collega's zagen (boven water) en hoe ze het interpreteerden
            - Wat er werkelijk speelde (onder water) per laag
            - Welke kernkwaliteit actief was en waar die doorsloeg
            - Welke SCARF-domeinen geraakt werden
            - Eén concreet leerpunt
            - Eén concrete actie voor de volgende keer

            Sla de analyse op als feedback met tags "ijsberg,kernkwadrant,analyse".
            """);
    }
}
