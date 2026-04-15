using ReflectieveDuck.Analyse.Application.DTOs;

namespace ReflectieveDuck.Analyse.Domain.Services;

public interface IStacktraceAnalyzer
{
    StacktraceAnalysisDto Analyze(string stacktrace);
}
