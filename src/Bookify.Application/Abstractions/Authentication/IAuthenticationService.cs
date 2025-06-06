﻿using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.Utilities;

namespace Bookify.Application.Abstractions.Authentication;
public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The identifier of the newly registered user</returns>
    
    // Task<string> RegisterAsync(
    //     User user,
    //     string password,
    //     CancellationToken cancellationToken = default);
    
    Task<Result<string>> RegisterAsync(User user, string password, CancellationToken cancellationToken = default);
}
