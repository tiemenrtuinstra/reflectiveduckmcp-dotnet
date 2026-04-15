using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace ReflectieveDuck.McpServer.Tools;

/// <summary>
/// Activeert specifieke reflectiemodules uit de Reflectie-Assistent.
/// Elke module geeft een gestructureerde reflectieopdracht terug die
/// de AI-client kan gebruiken om een sessie te begeleiden.
/// </summary>
[McpServerToolType, Authorize]
public class ReflectieModuleTools
{
    [McpServerTool(Name = "module_theory_of_mind"),
     Description("Activeer een Theory of Mind reflectie. Helpt om het perspectief van de ander te begrijpen bij een specifieke situatie. Geeft reflectievragen en aanpak.")]
    public string TheoryOfMind(
        [Description("Beschrijf de situatie waarin je het perspectief van de ander wilt begrijpen")] string situatie)
    {
        return $"""
            # Theory of Mind Reflectie

            ## Situatie
            {situatie}

            ## Reflectie-aanpak
            Theory of Mind helpt je te begrijpen dat anderen andere kennis, emoties en intenties hebben.
            Bij autisme is deze perspectiefneming vaak minder intuïtief — expliciete reflectie helpt.

            ## Stap 1: Observatie
            - Wat zag je de ander doen of zeggen?
            - Wat was de context (tijd, plaats, omstandigheden)?

            ## Stap 2: Interpretatie
            - Hoe denk je dat de ander zich voelde?
            - Wat zou de ander bedoeld kunnen hebben?
            - Welke informatie had de ander die jij niet had (of omgekeerd)?

            ## Stap 3: Eigen reactie
            - Wat voelde jij bij deze situatie?
            - Welke aanname maakte je over de intentie van de ander?
            - Is er een alternatieve verklaring mogelijk?

            ## Stap 4: Verbinding
            - Hoe zou je dit kunnen bespreken met de ander?
            - Wat zou je willen dat de ander begrijpt over jouw beleving?

            ## Reflectievragen
            - "Hoe denk je dat de ander dit bedoelde?"
            - "Welke informatie had jij niet die de ander wel had?"
            - "Als je dit vanuit hun ogen bekijkt, wat zie je dan?"
            """;
    }

    [McpServerTool(Name = "module_executieve_functies"),
     Description("Activeer een Executieve Functies hulpsessie. Helpt bij planning, taakorganisatie, prioritering en het schakelen tussen activiteiten. Geef een taak of probleem mee.")]
    public string ExecutieveFuncties(
        [Description("De taak of het probleem waarbij je hulp nodig hebt met planning/organisatie")] string taak)
    {
        return $"""
            # Executieve Functies Hulp

            ## Taak
            {taak}

            ## Aanpak
            EF-problemen bij autisme betekenen niet onvermogen, maar een andere informatie-verwerkingsstijl.
            Externe structuur helpt totdat interne planning weer stabiel is.

            ## Stap 1: Decompositie
            Breek de taak op in de kleinst mogelijke subtaken:
            - Wat is de allereerste concrete handeling?
            - Welke stappen komen daarna?
            - Waar zijn afhankelijkheden?

            ## Stap 2: Tijd- en energie-inschatting
            - Hoeveel minuten kost elke subtaak?
            - Welke subtaken kosten het meeste energie?
            - Welke kun je doen als je energie laag is?

            ## Stap 3: Volgorde bepalen
            - Begin met de taak die het minste beslisenergie kost
            - Plan energievretende taken in je groene momenten
            - Plan pauzes tussen schakelmomenten (min. 5 minuten)

            ## Stap 4: Externe geheugensteun
            - Schrijf elke stap op (checklist, Jira subtaken, post-it)
            - Gebruik een timer voor focusblokken (max 90 minuten)
            - Zet een alarm voor de eerste stap

            ## Reflectievragen
            - "Wat is het allerkleinste eerste stapje dat je kunt zetten?"
            - "Welke subtaak geeft het meeste voldoening als die af is?"
            - "Heb je hulp nodig bij een van de stappen? Van wie?"
            """;
    }

    [McpServerTool(Name = "module_sociale_coherentie"),
     Description("Activeer een Sociale Coherentie reflectie. Helpt bij het begrijpen van sociale situaties door patroon- en betekenisherkenning. Beschrijf de sociale situatie.")]
    public string SocialeCoherentie(
        [Description("Beschrijf de sociale situatie die je wilt begrijpen")] string situatie)
    {
        return $"""
            # Sociale Coherentie Reflectie

            ## Situatie
            {situatie}

            ## Aanpak
            Sociale coherentie gaat over het herkennen van patronen en betekenis in sociale contexten.
            We analyseren wat je zag, hoorde en voelde — en koppelen gedrag aan context.

            ## Stap 1: Observatie (feiten)
            - Wat zag je precies gebeuren?
            - Wat werd er gezegd (letterlijk)?
            - Welke non-verbale signalen merkte je op?

            ## Stap 2: Context
            - Waar en wanneer gebeurde dit?
            - Wat was de relatie tussen de betrokken personen?
            - Was er tijdsdruk, stress of andere invloeden?

            ## Stap 3: Betekenis
            - Welke verklaring geef jij aan het gedrag?
            - Zijn er alternatieve verklaringen mogelijk?
            - Welk patroon herken je (komt dit vaker voor)?

            ## Stap 4: Benoem mogelijkheden (geen fouten)
            - Wat had je anders kunnen doen?
            - Wat ging er goed in jouw reactie?
            - Wat zou je meenemen naar een vergelijkbare situatie?

            ## Voorbeeld
            Iemand is kortaf in een meeting → In plaats van "Hij is boos op mij":
            "Kan het zijn dat hij gehaast was door een deadline?"

            ## Reflectievragen
            - "Hoe zou de situatie eruitzien vanuit het standpunt van de ander?"
            - "Welk patroon herken je uit eerdere ervaringen?"
            - "Wat zou je aan een vertrouwenspersoon vertellen over deze situatie?"
            """;
    }

    [McpServerTool(Name = "module_dating"),
     Description("Activeer de Dating & Communicatie module. Analyseer een gesprek of interactie en krijg hulp bij communicatiepatronen, emotionele reacties en sociale intuïtie.")]
    public string DatingCommunicatie(
        [Description("Beschrijf de dating-interactie of het gesprek dat je wilt analyseren")] string interactie)
    {
        return $"""
            # Dating & Communicatie Analyse

            ## Interactie
            {interactie}

            ## Analyse-stappen

            ### 1. Observatie
            - Wat ging er goed in deze interactie?
            - Wat gaf spanning of onzekerheid?
            - Welke reactie had je verwacht vs. wat er werkelijk gebeurde?

            ### 2. Gevoel
            - Hoe voelde je je tijdens de interactie?
            - Hoe voelde je je achteraf?
            - Welk gevoel wil je benoemen?

            ### 3. Behoefte
            - Wat had je nodig op dat moment?
            - Wat wilde je dat de ander begreep?
            - Welke verbinding zocht je?

            ### 4. Communicatiestijl
            - Was je bericht/reactie te lang of te kort?
            - Was de toon duidelijk voor de ander?
            - Hoe kun je je boodschap vloeiender overbrengen?

            ### 5. Volgende stap
            - Wil je reageren? Zo ja, wat zou een rustige, open reactie zijn?
            - Is er iets dat je eerst wilt laten bezinken?
            - Kun je een vertrouwenspersoon vragen om mee te lezen?

            ## Reflectievragen
            - "Wat deed de reactie van de ander met jou?"
            - "Welke verwachting had je op dat moment?"
            - "Welke kwaliteit van jou kwam naar voren — en hoe kun je die doseren?"
            """;
    }

    [McpServerTool(Name = "module_reflectieve_duck"),
     Description("Activeer de Reflectieve Duck modus voor technische problemen. Helpt bij het structureren van probleemoplossing, doorbreekt tunnelvisie en kanalisert hyperfocus. Kies submodus: debug, projectstart of reflectie.")]
    public string ReflectieveDuckModus(
        [Description("Het technische probleem of de context")] string probleem,
        [Description("Submodus: debug, projectstart of reflectie (standaard: debug)")] string modus = "debug")
    {
        var modusContent = modus.ToLowerInvariant() switch
        {
            "projectstart" => """
                ## Submodus: Projectstart
                Structuur aanbrengen bij een nieuw project.

                ### Stap 1: Scope definiëren
                - Wat is het minimale werkende resultaat (MVP)?
                - Wat hoort er NIET bij voor nu?

                ### Stap 2: Architectuur schetsen
                - Welke componenten heb je nodig?
                - Welke bestaande code/patronen kun je hergebruiken?
                - Waar zitten de risico's?

                ### Stap 3: Takenlijst
                - Breek het project op in taken van max 90 minuten
                - Bepaal de volgorde (wat moet eerst?)
                - Markeer taken die hulp nodig hebben

                ### Stap 4: Eerste actie
                - Wat is het allereerste bestand dat je aanmaakt?
                - Zet een timer op 25 minuten en begin
                """,

            "reflectie" => """
                ## Submodus: Reflectie-toon (Theory of Mind verbreding)
                Combineer technische reflectie met sociale context.

                ### Technisch
                - Wat heb je geleerd van dit probleem?
                - Welke aanname bleek verkeerd?

                ### Sociaal
                - Hoe zou je dit uitleggen aan een collega?
                - Is er iemand die je om hulp zou kunnen vragen?
                - Hoe zou je teamgenoot dit probleem aanpakken?

                ### Groei
                - Waar ben je trots op in deze oplossing?
                - Wat zou je volgende keer anders doen?
                - Welke kennis wil je delen met het team?
                """,

            _ => """
                ## Submodus: Debug
                Tunnelvisie doorbreken tijdens foutopsporing.

                ### Stap 1: Stop en adem
                - Hoe lang ben je al bezig met dit probleem?
                - Heb je een pauze genomen in het laatste uur?

                ### Stap 2: Herformuleer
                - Beschrijf het probleem in één zin
                - Wat is het verwachte gedrag vs. het werkelijke gedrag?

                ### Stap 3: Aannames checken
                - Wat was je eerste aanname?
                - Welke aannames heb je NIET gecontroleerd?
                - Wat zou er anders zijn als je aanname niet klopt?

                ### Stap 4: Versimpel
                - Kun je het probleem reproduceren met minimale code?
                - Wat is de kleinste verandering die het probleem introduceert?
                - Heb je de error message werkelijk gelezen (niet gescand)?

                ### Stap 5: Rubber duck
                - Leg het probleem uit aan mij, stap voor stap
                - Waar aarzel je in je uitleg? Dat is waarschijnlijk de bug.
                """
        };

        return $"""
            # Reflectieve Duck Modus 🦆

            ## Probleem
            {probleem}

            ## Werkwijze
            - Eén vraag per keer — houdt het gesprek rustig
            - Korte, concrete zinnen
            - Focus op structuur, niet op emotie
            - Afsluiten met een samenvatting en volgende stap

            {modusContent}

            ## Reflectievragen
            - "Wat zie je nu dat je eerder niet zag?"
            - "Hoe zorg je dat je niet vastloopt in perfectie?"
            - "Waar ben je trots op in je aanpak tot nu toe?"
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
            → Benoem specifieke samenwerkingsmomenten

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

            ## Kernkwadrant Check
            - Kernkwaliteit: waar zagen we consciëntieusheid?
            - Valkuil: waar schoot perfectionisme door?
            - Uitdaging: hebben we hulp gevraagd waar nodig?
            - Allergie: was er slordigheid die spanning veroorzaakte?

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

        return $"""
            # Kernkwadrant Analyse — {contextLabel}

            ## Situatie
            {situatie}

            ## Stap 1: Welke kwaliteit speelde hier?
            Welke van jouw sterke kanten was actief in deze situatie?

            Bekende kwaliteiten (VIA Strengths + Ofman):
            | Kwaliteit | Valkuil (doorgeslagen) | Uitdaging (groei) | Allergie (trigger) |
            |-----------|----------------------|-------------------|-------------------|
            | Consciëntieusheid | Perfectionisme | Hulp vragen | Slordigheid |
            | Loyaliteit | Oververantwoordelijkheid | Loslaten, vertrouwen | Onverschilligheid |
            | Eerlijkheid | Botte directheid | Tact en timing | Hypocrisie |
            | Enthousiasme | Overintensiteit | Geduld, ruimte laten | Passiviteit |
            | Betrouwbaarheid | Zelfopoffering | Grenzen stellen | Onbetrouwbaarheid |
            | Nieuwsgierigheid | Verstrooiing/afdwaling | Focus houden | Desinteresse |
            | Rechtvaardigheid | Rigiditeit in regels | Flexibiliteit | Vriendjespolitiek |
            | Moed | Roekeloosheid | Reflectie voor actie | Lafheid |

            ## Stap 2: Sloeg de kwaliteit door?
            - Herken je de valkuil in deze situatie?
            - Wat was het effect op jezelf? Op de ander?
            - Was je je ervan bewust op het moment zelf?

            ## Stap 3: Wat is de groeirichting?
            - Welke uitdaging past bij de kwaliteit die doorsloeg?
            - Wat zou een klein, concreet eerste stapje zijn?
            - Wie zou je hierbij kunnen helpen?

            ## Stap 4: Wat triggerde je?
            - Was er gedrag van de ander dat je allergisch maakte?
            - Is dat gedrag het tegenovergestelde van jouw kernkwaliteit?
            - Kun je het gedrag van de ander loskoppelen van jouw reactie?

            ## Stap 5: Verbinding met je stoplichtplan
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

            ## Reflectievragen
            - "Welke kwaliteit van jou zat onder de frustratie?"
            - "Wat is de gezonde uitdaging die je nu kunt oefenen?"
            - "Hoe zou je dit bespreken met een vertrouwenspersoon?"
            """;
    }
}
