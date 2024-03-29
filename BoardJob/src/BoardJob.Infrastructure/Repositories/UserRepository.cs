﻿using BoardJob.Domain.Entities;
using BoardJob.Domain.Repositories.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoardJob.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
        {
            var result = await _signInManager.PasswordSignInAsync(
                email, password, false, false);
            return result.Succeeded;
        }

        public async Task<bool> SignUpAsync(User user, string password, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<User> GetByEmailAsync(string requestEmail, CancellationToken cancellationToken)
        {
            return await _userManager.Users.FirstOrDefaultAsync(user => user.Email == requestEmail, cancellationToken);
        }

        public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
