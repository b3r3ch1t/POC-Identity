using Poc.Infra.CrossCutting.Identity.Enums;
using Poc.Infra.CrossCutting.Identity.Models;
using Poc.Infra.CrossCutting.Identity.Models.AccountViewModels;
using Poc.Infra.CrossCutting.Identity.Models.ManageViewModels;
using POC.Domain.Core;
using POC.Domain.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Poc.WebApi.Interfaces
{
    public interface IBaseUserInterface : IDisposable
    {
        Task<Result<object, IErrorMessage>> Register(RegisterViewModel model, TypeUserEnum typeUser);

        Task<Result<object, IErrorMessage>> ConfirmEmail(string userId, string code);

        string Login(LoginUserViewModel model);

        Task<Result<object, IErrorMessage>> Delete(Guid userId);

        Task<Result<object, IErrorMessage>> List(TypeUserEnum typeUser, int page = 0, int pageSize = 0, string search = "", string sort = "");

        Task<Result<object, IErrorMessage>> ChangePassword(ChangePasswordViewModel model);
        Task<Result<object, IErrorMessage>> SetPassword(SetPasswordViewModel model);
        Task<Result<object, IErrorMessage>> GenerateRecoveryCodes(Guid userId);
        Task<Result<object, IErrorMessage>> ForgotPassword(ForgotPasswordViewModel model);
        Task<Result<object, IErrorMessage>> ResetPassword(ResetPasswordViewModel model);
        Task<Result<object, IErrorMessage>> SendVerificationEmail(IndexViewModel model);
        Task<Result<object, IErrorMessage>> GetById(Guid userId);
    }
}