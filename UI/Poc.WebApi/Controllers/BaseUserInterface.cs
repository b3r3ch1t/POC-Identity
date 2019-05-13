using MediatR;
using Microsoft.AspNetCore.Identity;
using Poc.Infra.CrossCutting.Identity.Commands;
using Poc.Infra.CrossCutting.Identity.Enums;
using Poc.Infra.CrossCutting.Identity.Interfaces;
using Poc.Infra.CrossCutting.Identity.Models;
using Poc.Infra.CrossCutting.Identity.Models.AccountViewModels;
using Poc.Infra.CrossCutting.Identity.Models.ManageViewModels;
using Poc.Infra.CrossCutting.Identity.Repository;
using Poc.WebApi.Interfaces;
using POC.Domain.Core;
using POC.Domain.Core.Bus;
using POC.Domain.Core.Interfaces;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Poc.WebApi.Controllers
{
    public class BaseUserInterface : ApiController, IBaseUserInterface
    {
        private readonly IUserIdentityRepository _userIdentityRepository;
        private readonly INotificationHandler<DomainNotification> _notification;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMediatorHandler _mediator;
        public BaseUserInterface(
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator,
            ILoggerInterface logger,
            IUserIdentityRepository userIdentityRepository,
             INotificationHandler<DomainNotification> notification,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService) : base(
            notifications,
            mediator,
            logger)
        {
            _userIdentityRepository = userIdentityRepository;
            _notification = notification;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mediator = mediator;
        }

        public async Task<Result<object, IErrorMessage>> Register(RegisterViewModel model, TypeUserEnum typeUser)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }

            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };

            var resultAddUser = await _userIdentityRepository.CreateAsync(user, model.Password).ConfigureAwait(false);

            if (resultAddUser.Succeeded)
            {

                switch (typeUser)
                {
                    case TypeUserEnum.Merchant:
                        var claimMerchant = new Claim(type: "UserType", value: "Merchant");

                        var resultAddClaimMerchant = await _userIdentityRepository.AddClaimAsync(user, claimMerchant);

                        if (resultAddClaimMerchant.Succeeded)
                        {

                            var to = model.Email;
                            var subject = "Registration.";
                            var body = $"Hi {model.Username}, welcome!";


                            var command = new UserMerchantRegistredCommand(to: to, subject: subject, body: body);

                            await _mediator.SendCommandAsync(command).ConfigureAwait(false);
                        }
                        break;

                    case TypeUserEnum.Admin:

                        var claimAdmin = new Claim(type: "UserType", value: "Admin");

                        var resultAddClaimAdmin = await _userIdentityRepository.AddClaimAsync(user, claimAdmin).ConfigureAwait(false);

                        if (resultAddClaimAdmin.Succeeded)
                        {

                            var to = model.Email;
                            var subject = "Registration.";
                            var body = $"Hi {model.Username}, welcome!";

                            var command = new UserAdminRegistredCommand(to: to, subject: subject, body: body);

                            await _mediator.SendCommandAsync(command).ConfigureAwait(true);
                        }
                        break;

                }

                return Response();
            }

            AddIdentityErrors(resultAddUser);

            return Response();
        }

        public async Task<Result<object, IErrorMessage>> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                NotifyError(code: "User doesn't exists.",
                    message: $"Unable to load user with ID '{userId}'.");
                return Response();
            }

            var user = await _userIdentityRepository.FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                NotifyError(code: "User doesn't exists.",
                    message: $"Unable to load user with ID '{userId}'.");
                return Response();
            }


            var result = await _userIdentityRepository.ConfirmEmailAsync(user, code).ConfigureAwait(false);

            if (result.Succeeded)
            {
                var to = user.Email;
                var subject = "Registration.";
                var body = $"The {user.UserName}, was confirmed !";

                var command = new RegistrationConfirmedCommand(to: to, body: body, subject: subject);

                await _mediator.SendCommandAsync(command).ConfigureAwait(true);

            }

            return Response();
        }

        public string Login(LoginUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.Values.SelectMany(v => v.Errors).ToString();
            }


            var result = _signInManager.PasswordSignInAsync(model.Username, model.Password, false, true)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            if (!result.Succeeded) return "User or password is not valid !!!!";

            var response = _tokenService.CreateTokenUser(model.Username);
            return response;
        }

        public async Task<Result<object, IErrorMessage>> Delete(Guid userId)
        {
            var user = await _userIdentityRepository.GetUserByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                NotifyError(code: $"Unable to load user with ID '{userId}'.",
                    message: $"Unable to load user with ID '{userId}'.");
                return Response();
            }

            user.Delete();

            var result = await _userIdentityRepository.UpdateUserAsync(user).ConfigureAwait(false);


            if (!result.Succeeded) return Response(result);
            var to = user.Email;
            var subject = "Delete.";
            var body = $"Hi {user.UserName}, was deleted!";


            var command = new UserDeletedCommand(to: to, subject: subject, body: body);
            await _mediator.SendCommandAsync(command).ConfigureAwait(false);
            return Response(result);
        }

        public async Task<Result<object, IErrorMessage>> List(TypeUserEnum typeUser)
        {
            IQueryable<ApplicationUser> listUsers = null;
            var result = new List<ApplicationUser>();

            #region Type Of User
            switch (typeUser)
            {
                case TypeUserEnum.Admin:

                    listUsers = _userIdentityRepository.GetListProfile(TypeUserEnum.Admin);
                    break;

                case TypeUserEnum.Merchant:
                    listUsers = _userIdentityRepository.GetListProfile(TypeUserEnum.Merchant);
                    break;

                case TypeUserEnum.User:
                    listUsers = _userIdentityRepository.GetListProfile(TypeUserEnum.User);
                    break;
            }
            #endregion

            return Response(result);
        }

        public async Task<Result<object, IErrorMessage>> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }

            var user = await _userIdentityRepository.GetUserByIdAsync(model.Id).ConfigureAwait(false);

            if (user == null)
            {
                NotifyError(code: "User doesn't exists.",
                    message: $"Unable to load user with ID '{model.Id}'.");
                return Response(model);
            }

            var changePasswordResult = await _userIdentityRepository.ChangePasswordAsync(user, model.OldPassword, model.NewPassword).ConfigureAwait(false);

            if (changePasswordResult.Succeeded) return Response();
            AddIdentityErrors(changePasswordResult);
            return Response(model);
        }

        public async Task<Result<object, IErrorMessage>> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }

            var user = await _userIdentityRepository.GetUserByIdAsync(model.Id).ConfigureAwait(false);

            if (user == null)
            {
                NotifyError(code: "User doesn't exists.",
                    message: $"Unable to load user with ID '{model.Id}'.");
                return Response(model);
            }

            var addPasswordResult = await _userIdentityRepository.AddPasswordAsync(user, model.NewPassword).ConfigureAwait(false);
            if (addPasswordResult.Succeeded) return Response();

            AddIdentityErrors(addPasswordResult);
            return Response(model);
        }

        public async Task<Result<object, IErrorMessage>> GenerateRecoveryCodes(Guid userId)
        {
            var user = await _userIdentityRepository.GetUserByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                NotifyError(code: "User doesn't exists.",
                    message: $"Unable to load user with ID '{userId}'.");
                return Response();
            }


            var recoveryCodes = await _userIdentityRepository.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAwait(false);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            return Response(model);
        }

        public async Task<Result<object, IErrorMessage>> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }

            var user = await _userIdentityRepository.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null || !await _userIdentityRepository.IsEmailConfirmedAsync(user).ConfigureAwait(false))
            {
                // Don't reveal that the user does not exist or is not confirmed
                NotifyError(code: "User doesn't exists or password is not correct.",
                    message: $"User doesn't exists or password is not correct.");
                return Response();
            }

            // For more information on how to enable account confirmation and password reset please
            var code = await _userIdentityRepository.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

            var result = new ForgotPasswordResponseViewModel()
            {
                To = user.Email,
                Subject = "Forgot",
                Code = code
            };

            return Response(result);
        }

        public async Task<Result<object, IErrorMessage>> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }

            var user = await _userIdentityRepository.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                NotifyError(code: "User doesn't exists or password is not correct.",
                    message: $"User does'n exists or password is not correct.");
                return Response();
            }
            var resetPasswordResult = await _userIdentityRepository.ResetPasswordAsync(user, model.Code, model.Password).ConfigureAwait(false);

            if (resetPasswordResult.Succeeded)
            {
                return Response("Password reseted !!!");
            }

            AddIdentityErrors(resetPasswordResult);
            return Response(model);
        }

        public async Task<Result<object, IErrorMessage>> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }

            var user = await _userIdentityRepository.GetUserByIdAsync(model.UserId).ConfigureAwait(false);

            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                NotifyError(code: $"Unable to load user with ID '{model.UserId}'.",
                    message: $"Unable to load user with ID '{model.UserId}'.");
                return Response();

            }

            var code = await _userIdentityRepository.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);


            var result = new SendVerificationEmailViewModel()
            {
                To = user.Email,
                Subject = "SendVerificationEmail",
                Code = code
            };


            return Response(result);
        }

        public async Task<Result<object, IErrorMessage>> GetById(Guid userId)
        {
            var result = await _userIdentityRepository.GetUserByIdAsync(userId).ConfigureAwait(false);

            return Response(result);
        }


        public void Dispose()
        {
            _userIdentityRepository?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
