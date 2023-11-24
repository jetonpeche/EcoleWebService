using System.Security.Claims;

namespace back.Services.Jwts;

public interface IJwtService
{
    /// <summary>
    /// Generer un JWT
    /// </summary>
    /// <param name="_tabClaim">Infos contenu dans le JWT</param>
    /// <returns>Renvoie le JWT</returns>
    string Generer(Claim[] _tabClaim);

    /// <summary>
    /// Generer un refresh token valable 2 jours
    /// </summary>
    /// <returns>Renvoie le refresh token</returns>
    string GenererRefreshToken();
}
