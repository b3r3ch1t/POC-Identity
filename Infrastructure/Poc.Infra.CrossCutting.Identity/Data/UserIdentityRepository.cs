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
using Poc.Infra.CrossCutting.Identity.Repository;

namespace Poc.Infra.CrossCutting.Identity.Data
{
    public class UserIdentityRepository : IUserIdentityRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDataContext _context;

        public UserIdentityRepository(UserManager<ApplicationUser> userManager,
            ApplicationDataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public void Dispose()
        {
            _userManager?.Dispose();
            _context?.Dispose();
        }

        public IEnumerable<ApplicationUser> Search(Expression<Func<ApplicationUser, bool>> predicate)
        {
            var result = _context.Users.Where(predicate);
            return result;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(Guid id)
        {
            var result = (GetAll()).FirstOrDefault(x => x.Id == id);
            return result;
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            var result = _context.Users.Where(x => x.Valid);
            return result;
        }

        public ApplicationUser Create(ApplicationUser user, string password)
        {
            var passwordHasher = _userManager.PasswordHasher.HashPassword(user, password);

            user.PasswordHash = passwordHasher;

            _context.Users.Add(user);

            try
            {
                _context.SaveChanges();

                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            return null;
        }

        public async Task<IdentityResult> AddClaimAsync(ApplicationUser user, Claim claim)
        {
            var result = await _userManager.AddClaimAsync(user, claim).ConfigureAwait(true);
            return result;
        }

        public Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
        {
            var result = _userManager.GetUserAsync(user);
            return result;
        }

        public Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string senhaAtual, string novaSenha)
        {
            var result = _userManager.ChangePasswordAsync(user, senhaAtual, novaSenha);
            return result;
        }

        public IList<ClaimViewModel> GetClaimsUser(ApplicationUser user)
        {
            //var result = _userManager.GetClaimsAsync(user).Result;

            var result = _context.UserClaims.Where(x => x.UserId == user.Id).Select(x => new ClaimViewModel()
            {
                Id = x.Id,
                UserId = x.UserId,
                ClaimType = x.ClaimType,
                ClaimValue = x.ClaimValue
            });


            return result.ToList();
        }

        public Task<ApplicationUser> FindByIdAsync(string id)
        {
            var result = _userManager.FindByIdAsync(id);
            return result;
        }

        public Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var result = _userManager.GeneratePasswordResetTokenAsync(user);
            return result;
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var result = _userManager.FindByEmailAsync(email);
            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string password)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, password).ConfigureAwait(false);
            return result;
        }

        public Task<string> GeneratePasswordResetTokenNullAsync(string host)
        {
            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                Email = Guid.NewGuid() + "@" + host,
                UserName = Guid.NewGuid().ToString(),
                NormalizedEmail = Guid.NewGuid() + "@" + host,
                EmailConfirmed = true,
                Valid = true,
                PasswordHash = Guid.NewGuid().ToString()
            };
            var result = _userManager.GeneratePasswordResetTokenAsync(user);
            return result;
        }


        public async Task<IdentityResult> RemoveClaimUserAsync(Guid userId, Claim claim)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(true);
            var result = await _userManager.RemoveClaimAsync(user, claim).ConfigureAwait(true);
            return result;
        }

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return 0;
            }

        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                _context.Users.Update(user);
                var result = await _context.SaveChangesAsync().ConfigureAwait(false);

                return IdentityResult.Success;
            }

            catch
            {
                return IdentityResult.Failed();
            }
        }

        public IEnumerable<ApplicationUser> PagedResult(int page, int pageSize, string type)
        {
            var result = from cl in _context.UserClaims
                    .Where(x => string.Equals(x.ClaimType.ToUpper(), type.ToUpper(), StringComparison.Ordinal))
                         join user in _context.Users.Where(x => x.Valid) on cl.UserId equals user.Id
                         select user;

            var skip = (page - 1) * pageSize;

            return result.Skip(skip).Take(pageSize);
        }

        public int PageCount(int pageSize, string type)
        {
            var rowCount = (from cl in _context.UserClaims
                    .Where(x => string.Equals(x.ClaimType.ToUpper(), type.ToUpper(), StringComparison.Ordinal))
                            join user in _context.Users.Where(x => x.Valid) on cl.UserId equals user.Id
                            select user).Count();

            var pageCount = (int)Math.Ceiling((decimal)rowCount / pageSize);

            return pageCount;

        }



        public Task<IdentityResult> AddPasswordAsync(ApplicationUser user, string password)
        {
            var result = _userManager.AddPasswordAsync(user, password);
            return result;
        }

        public Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int number)
        {
            var result = _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, number);
            return result;
        }

        public Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string code)
        {
            var result = _userManager.ConfirmEmailAsync(user, code);

            return result;
        }

        public Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            var result = _userManager.IsEmailConfirmedAsync(user);
            return result;
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var result = _userManager.GenerateEmailConfirmationTokenAsync(user);
            return result;
        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var result = _userManager.GetClaimsAsync(user);
            return result;
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            var result = _userManager.CreateAsync(user, password);

            return result;
        }

        public IEnumerable<ApplicationUser> PagedResult(int page, int pageSize)
        {
            var result = _userManager.Users;

            var skip = (page - 1) * pageSize;

            return result.Skip(skip).Take(pageSize);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(string type)
        {
            var result = from cl in _context.UserClaims
                         join user in _context.Users.Where(x => x.Valid) on cl.UserId equals user.Id
                         where string.Equals(cl.ClaimValue, type, StringComparison.CurrentCultureIgnoreCase)
                         select user;

            return result;
        }

        public ApplicationUser GetUserByUsername(string userName)
        {
            var user = _context.Users.FirstOrDefault(x =>
                string.Equals(x.UserName, userName, StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        public void AddRefreshToken(ApplicationUser user, string refreshToken)
        {
            var token = _context.UserTokens.FirstOrDefault(x => x.UserId == user.Id);

            if (token == null)
            {
                token = new IdentityUserToken<Guid>()
                {
                    UserId = user.Id,
                    LoginProvider = "JWT",
                    Name = "REFRESHTOKEN",
                    Value = refreshToken
                };

                _context.UserTokens.Add(token);

            }

            token.Value = refreshToken;

            _context.SaveChanges();
        }

        public string GetTokenByUserId(Guid userId)
        {
            var token = _context.UserTokens.FirstOrDefault(x => x.UserId == userId);
            return token == null ? string.Empty : token.Value;
        }

        public IQueryable<ApplicationUser> GetListProfile(TypeUserEnum typeUserEnum)
        {
            var typeUser = string.Empty;

            if (typeUserEnum != TypeUserEnum.Admin && typeUserEnum != TypeUserEnum.Merchant)
                return GetUsersWithoutClaims();


            switch (typeUserEnum)
            {
                case TypeUserEnum.Admin:
                    typeUser = "ADMIN";
                    break;
                case TypeUserEnum.Merchant:
                    typeUser = "MERCHANT";
                    break;
            }

            return GetUsersByFunction(typeUser);

        }


        IQueryable<ApplicationUser> GetUsersByFunction(string s)
        {
            var result = from c in
                    _context.UserClaims
                        .Where(x => string.Equals(x.ClaimValue, s, StringComparison.Ordinal))
                         join u in _context.Users on c.UserId equals u.Id
                         select u;

            return result;
        }

        IQueryable<ApplicationUser> GetUsersWithoutClaims()
        {
            var result = from u in _context.Users
                         join c in _context.UserClaims on u.Id equals c.UserId into tmpMapp
                         from m in tmpMapp.DefaultIfEmpty()
                         select u;

            return result;
        }
    }
}
