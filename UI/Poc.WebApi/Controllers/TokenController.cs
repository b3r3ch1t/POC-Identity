using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poc.Infra.CrossCutting.Identity.Interfaces;
using Poc.Infra.CrossCutting.Identity.Repository;
using POC.Domain.Core;
using POC.Domain.Core.Bus;
using POC.Domain.Core.Interfaces;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;
using System.Threading.Tasks;

namespace Poc.WebApi.Controllers
{
    public class TokenController : ApiController
    {

        private readonly ITokenService _tokenService;
        private readonly IUserIdentityRepository _userIdentityRepository;

        public TokenController(
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator,
            ILoggerInterface logger,
            ITokenService tokenService, IUserIdentityRepository userIdentityRepository) : base(
            notifications,
            mediator,
            logger)
        {
            _tokenService = tokenService;
            _userIdentityRepository = userIdentityRepository;
        }

        [HttpPost]
        [Route("/api/v1/token/refresh/{refreshToken}")]
        public async Task<Result<object, IErrorMessage>> Refresh(string refreshToken)
        {

            var result = _tokenService.ValidateToken(refreshToken);

            if (result.Valid) return Response(result.Message);


            NotifyError("Token Invalid", "Token Invalid");
            return Response(refreshToken);
        }

    }
}

