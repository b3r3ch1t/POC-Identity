using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Poc.Infra.CrossCutting.Identity.Enums;
using Poc.Infra.CrossCutting.Identity.Models;
using Poc.Infra.CrossCutting.Identity.Models.AccountViewModels;

namespace Poc.Infra.CrossCutting.Identity.Repository
{
    public interface IUserIdentityRepository : IDisposable
    {
        IEnumerable<ApplicationUser> Search(Expression<Func<ApplicationUser, bool>> predicate);
        Task<ApplicationUser> GetUserByIdAsync(Guid id);
        IQueryable<ApplicationUser> GetAll();

        ApplicationUser Create(ApplicationUser user, string password);
        Task<IdentityResult> AddClaimAsync(ApplicationUser user, Claim claim);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string senhaAtual, string novaSenha);
        IList<ClaimViewModel> GetClaimsUser(ApplicationUser user);
        Task<ApplicationUser> FindByIdAsync(string id);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string password);
        Task<string> GeneratePasswordResetTokenNullAsync(string host);
        Task<IdentityResult> RemoveClaimUserAsync(Guid user, Claim claim);
        int SaveChanges();
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

        IEnumerable<ApplicationUser> PagedResult(int page, int pageSize, string type);

        int PageCount(int pageSize, string type);

        Task<IdentityResult> AddPasswordAsync(ApplicationUser user, string newPassword);
        Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int i);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string code);
        Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string modelPassword);
        IEnumerable<ApplicationUser> PagedResult(int page, int pageSize);
        Task<IEnumerable<ApplicationUser>> GetAllAsync(string type);
        ApplicationUser GetUserByUsername(string userName);
        void AddRefreshToken(ApplicationUser user, string refreshToken);
        string GetTokenByUserId(Guid userId);
        IQueryable<ApplicationUser> GetListProfile(TypeUserEnum typeUser);
    }
}