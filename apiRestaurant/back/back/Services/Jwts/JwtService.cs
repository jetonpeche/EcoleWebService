using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace back.Services.Jwts
{
    public sealed class JwtService: IJwtService
    {
        private RSA Rsa { get; init; }
        private string Issuer { get; init; }

        public JwtService(RSA _rsa, string _issuer)
        {
            Rsa = _rsa;
            Issuer = _issuer;
        }

        public string Generer(Claim[] _tabClaim)
        {
            var gestionnaireJwt = new JsonWebTokenHandler();

            // permet de signer le JWT
            var cle = new RsaSecurityKey(Rsa);

            // creation du JWT
            // par defaut dure 1 heure
            var jwt = gestionnaireJwt.CreateToken(new SecurityTokenDescriptor
            {
                // informations ajouter dans le JWT
                Subject = new ClaimsIdentity(_tabClaim),

                // OBLIGATOIRE => qui est l'émeteur
                // en général mettre URL
                Issuer = Issuer,

                SigningCredentials = new SigningCredentials(cle, SecurityAlgorithms.RsaSha256)
            });

            return jwt;
        }

        public string GenererRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
