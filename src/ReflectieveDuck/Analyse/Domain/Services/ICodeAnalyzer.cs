using ReflectieveDuck.Analyse.Application.DTOs;

namespace ReflectieveDuck.Analyse.Domain.Services;

public interface ICodeAnalyzer
{
    CodeAnalysisDto Analyze(string code, string? taal = null);
}
