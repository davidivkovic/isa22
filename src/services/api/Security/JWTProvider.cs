namespace API.Security;

using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using API.Core.Model;

public class JWTProvider
{
    public static string Issuer { get; } = "api";
    public static string Audience { get; } = "spa";
    public static SymmetricSecurityKey Key { get; } = new(
        Encoding.UTF8.GetBytes("9e6cd2eca1d9426781f516085ae0ce18")
    );

    private static readonly JwtSecurityTokenHandler _tokenHandler = new();
    private static readonly SigningCredentials _signingCredentials = new(
        Key,
        SecurityAlgorithms.HmacSha256Signature
    );

    public static JwtSecurityToken ParseJwtRefreshToken(string token)
    {
        try
        {
            _tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = Key,
                ValidIssuer = Issuer,
                ValidAudience = "refresh",
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }
        catch
        {
            return null;
        }
    }

    public static string GetAccessToken(User user)
    {
        var claims = user.Roles.Select(r => 
            new Claim(ClaimTypes.Role, r.ToString())
        ).ToList();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(500),
            SigningCredentials = _signingCredentials,
            Issuer = Issuer,
            Audience = Audience
        };

        SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
}