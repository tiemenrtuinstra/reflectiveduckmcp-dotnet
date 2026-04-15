namespace ReflectieveDuck.Reflectie.Domain.Entities;

public record ReflectieVraag(
    string Vraag,
    string Categorie,
    string? Context = null);
