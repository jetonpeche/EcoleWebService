namespace back.Services.Protections;

public interface IProtectionService
{
    string? XSS(string? _texte);
}
