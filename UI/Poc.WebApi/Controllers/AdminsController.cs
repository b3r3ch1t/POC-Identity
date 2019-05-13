using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poc.Infra.CrossCutting.Identity.Enums;
using Poc.Infra.CrossCutting.Identity.Models;
using Poc.Infra.CrossCutting.Identity.Models.AccountViewModels;
using Poc.Infra.CrossCutting.Identity.Models.ManageViewModels;
using Poc.WebApi.Interfaces;
using POC.Domain.Core;
using POC.Domain.Core.Bus;
using POC.Domain.Core.Interfaces;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;
using System;
using System.Threading.Tasks;

namespace Poc.WebApi.Controllers
{
    public class AdminsController : ApiController
    {
        private readonly IBaseUserInterface _baseUserInterface;

        public AdminsController(
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator,
            ILoggerInterface logger,
            IBaseUserInterface baseUserInterface
            ) : base(
            notifications,
            mediator,
            logger
            )
        {
            _baseUserInterface = baseUserInterface;
        }

        [HttpPost]
        [Route("/api/v1/admins/register")]
        public async Task<Result<object, IErrorMessage>> Register([FromBody] RegisterViewModel model)
        {
            var result = await _baseUserInterface.Register(model, TypeUserEnum.Admin);

            return result;
        }

        [HttpPost]
        [Route("/api/v1/admins/change-password")]
        public async Task<Result<object, IErrorMessage>> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            var result = await _baseUserInterface.ChangePassword(model);
            return result;
        }

        [HttpPost]
        [Route("/api/v1/admins/set-password")]
        public async Task<Result<object, IErrorMessage>> SetPassword([FromBody] SetPasswordViewModel model)
        {
            var result = await _baseUserInterface.SetPassword(model);
            return result;
        }

        [HttpPost]
        [Route("/api/v1/admins/generate-recovery-code")]
        public async Task<Result<object, IErrorMessage>> GenerateRecoveryCodes(Guid userId)
        {
            var result = await _baseUserInterface.GenerateRecoveryCodes(userId);
            return result;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("/api/v1/admins/confirm-email/{UserId}/{Code}")]
        public async Task<Result<object, IErrorMessage>> ConfirmEmail(string userId, string code)
        {
            var result = await _baseUserInterface.ConfirmEmail(userId: userId, code: code);
            return result;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("/api/v1/admins/forgot-password")]
        public async Task<Result<object, IErrorMessage>> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            var result = await _baseUserInterface.ForgotPassword(model);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/v1/admins/reset-password")]
        public async Task<Result<object, IErrorMessage>> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var result = await _baseUserInterface.ResetPassword(model);
            return result;
        }

        [HttpPost]
        [Route("/api/v1/admins/send-verification-email")]
        public async Task<Result<object, IErrorMessage>> SendVerificationEmail([FromBody] IndexViewModel model)
        {
            var result = await _baseUserInterface.SendVerificationEmail(model);
            return result;
        }

        [HttpDelete]
        [Route("/api/v1/admins/delete/{userId}")]
        public async Task<Result<object, IErrorMessage>> Delete(Guid userId)
        {
            var result = await _baseUserInterface.Delete(userId);
            return result;
        }

        [HttpGet]
        [Route("/api/v1/admins/{userId}")]
        public async Task<Result<object, IErrorMessage>> GetById(Guid userId)
        {
            var result = await _baseUserInterface.GetById(userId);
            return result;
        }


        [HttpGet]
        [Route("/api/v1/admins/{page}/{pageSize}/{search}/{sort}")]
        [Route("/api/v1/admins/")]

        public async Task<Result<object, IErrorMessage>> List(int page = 0, int pageSize = 0, string search = "", string sort = "")
        {
            var result = await _baseUserInterface.List(typeUser: TypeUserEnum.Admin, page: page, pageSize: pageSize,
                search: search, sort: sort);

            return result;
        }


        [HttpPost]
        [Route("/api/v1/admins/login")]
        public string Login([FromBody] LoginUserViewModel model)
        {
            var result = _baseUserInterface.Login(model);

            return result;
        }

    }

}
