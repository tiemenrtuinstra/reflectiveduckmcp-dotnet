using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Resources.Infrastructure;

namespace ReflectieveDuck.McpServer.Tools;

/// <summary>
/// Gerichte wellbeing tools die de rijke YAML content ontsluiten.
/// Geeft concrete, contextgevoelige tips i.p.v. hele YAML documenten te dumpen.
/// </summary>
[McpServerToolType, Authorize]
public class WellbeingTools
{
    private readonly ResourceProvider _provider;

    public WellbeingTools(ResourceProvider provider) => _provider = provider;

    [McpServerTool(Name = "stoplicht_tips"),
     Description("Haal concrete tips op voor een stoplichtkleur: zelfondersteuning, collegaondersteuning, mindfulness, Scrum/Agile aanpassingen. Gebruik dit om te weten wat je kunt doen bij je huidige kleur.")]
    public string GetStoplichtTips(
        [Description("Kleur: groen, oranje, rood of blauw")] string kleur)
        => _provider.GetStoplichtTips(kleur);

    [McpServerTool(Name = "stoplicht_codewoord"),
     Description("Geeft het transitie-codewoord voor een stoplichtkleur. Dit zijn de afgesproken signaalwoorden om je stoplichtkleur te communiceren naar collega's.")]
    public string GetCodewoord(
        [Description("Kleur: groen, oranje, rood of blauw")] string kleur)
        => _provider.GetCodewoord(kleur);

    [McpServerTool(Name = "kernkwadrant"),
     Description("Bekijk het Kernkwadrant van Ofman: kernkwaliteit (consciëntieusheid), valkuil (perfectionisme), uitdaging (hulp vragen) en allergie (slordigheid). Nuttig voor feedback-momenten.")]
    public string GetKernkwadrant()
        => _provider.GetKernkwadranten();

    [McpServerTool(Name = "emmer_strategieen"),
     Description("Concrete strategieën om de emmer te legen (ASS-wijzer). Kies een categorie: prikkels, communicatie, veranderingen of alles.")]
    public string GetEmmerStrategieen(
        [Description("Categorie: prikkels, communicatie, veranderingen (optioneel, zonder = alles)")] string? categorie = null)
        => _provider.GetAssWijzer(categorie is "prikkels" or "communicatie" or "veranderingen"
            ? categorie
            : "zelfhulp");

    [McpServerTool(Name = "emmer_anderen"),
     Description("Wat kunnen anderen (collega's, manager, HR) doen om te helpen? Concrete tips uit de ASS-wijzer voor je ondersteuningsnetwerk.")]
    public string GetWatAnderenKunnenDoen()
        => _provider.GetAssWijzer("anderen");

    [McpServerTool(Name = "emmer_triggers"),
     Description("Bekijk je persoonlijke triggers — waar loopt de emmer van vol? Kies: prikkels, communicatie of veranderingen.")]
    public string GetEmmerTriggers(
        [Description("Categorie: prikkels, communicatie, veranderingen (optioneel, zonder = alles)")] string? categorie = null)
        => _provider.GetAssWijzer(categorie ?? "prikkels");

    [McpServerTool(Name = "emmer_reactie"),
     Description("Wat gebeurt er als de emmer overstroomt? Herken je eigen overbelastingsreacties (piekeren, terugtrekken, inflexibiliteit, stress-eten, etc.).")]
    public string GetOverbelastingsReacties()
        => _provider.GetAssWijzer("reactie");

    [McpServerTool(Name = "stoplicht_rolverdeling"),
     Description("Wie doet wat per stoplichtkleur? Bekijk de rolverdeling: wat collega's, Scrum Master, Product Owner, manager, HR en vertrouwenspersoon moeten doen. Optioneel gefilterd op kleur.")]
    public string GetRolverdeling(
        [Description("Kleur: groen, oranje, rood, blauw (optioneel, zonder = algemene rolverdeling)")] string? kleur = null)
    {
        if (kleur is not null)
            return _provider.GetStoplichtTips(kleur);

        return _provider.GetStoplichtplan();
    }

    [McpServerTool(Name = "stoplicht_volledig"),
     Description("Haal het complete stoplichtplan op: alle kleuren met eigen beleving, wat anderen zien, triggers, tips, mindfulness, Scrum aanpassingen, SCARF, Leary Roos, codewoorden en rolverdeling.")]
    public string GetVolledigStoplichtplan()
        => _provider.GetStoplichtplan();
}
