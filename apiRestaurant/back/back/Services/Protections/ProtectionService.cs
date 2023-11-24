using System.Text.RegularExpressions;

namespace back.Services.Protections;

public sealed class ProtectionService: IProtectionService
{
    public string? XSS(string? _texte)
    {
        if (string.IsNullOrEmpty(_texte))
            return _texte;

        return Regex.Replace(_texte, "<[^>]*>", "");
    }
}
