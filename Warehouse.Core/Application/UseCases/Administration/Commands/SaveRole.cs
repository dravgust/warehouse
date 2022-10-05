using MediatR;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel.Exceptions;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Domain.Entities.Security;
using Warehouse.Core.Domain.Exceptions;

namespace Warehouse.Core.Application.UseCases.Administration.Commands
{
    public class SaveRole : SecurityRoleEntity, ICommand
    { }

    public class HandleSaveRole : ICommandHandler<SaveRole>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;
        private readonly ILogger<HandleSaveRole> _logger;

        public HandleSaveRole(IUserRepository userRepository, IUserContext userContext, ILogger<HandleSaveRole> logger)
        {
            _userRepository = userRepository;
            _userContext = userContext;
            _logger = logger;
        }
        public async Task<Unit> Handle(SaveRole command, CancellationToken cancellationToken)
        {
            try
            {
                await _userContext.LoadSessionAsync();

                if (!_userContext.IsAdministrator)
                    throw new NotEnoughPermissionsException();

                if (!_userContext.IsSupervisor)
                    command.ProviderId = _userContext.User.Identity?.GetProviderId();

                if (!string.IsNullOrEmpty(command.Id))
                {
                    var old = await _userRepository.FindRoleByIdAsync(command.Id, cancellationToken);
                    if (old == null)
                        throw EntityNotFoundException.For<SecurityRoleEntity>(command.Id);

                    if (!old.Name.Equals(command.Name) || !string.Equals(old.Description, command.Description))
                        await _userRepository.UpdateSecurityRoleAsync(command, cancellationToken);
                }
                else
                {
                    await _userRepository.UpdateSecurityRoleAsync(command, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
            }

            return Unit.Value;
        }
    }
}
