using System;
using System.Collections.Generic;
using System.Security.Claims;
using MdNotesServer.Infrastructure.Entities;


namespace MdNotesServer.Infrastructure.Jwt
{
    public interface IJwtAuthManager<T>
    {
        Task<bool> RemoveExpiredTokens(T user);

        Task<JwtAuthResult> GenerateTokens(T user, IEnumerable<Claim> claims, DateTime now);
    }
}
