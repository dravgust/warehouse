﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Vayosoft.Commands;
using Vayosoft.Persistence;
using Vayosoft.Commons.Exceptions;
using Vayosoft.Utilities;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Enums;
using Warehouse.Core.Domain.Exceptions;

namespace Warehouse.Core.Application.SystemAdministration.Commands;

public class SaveUser : ICommand
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Phone { get; set; }
    public string Type { get; set; }
    public DateTime? Registered { get; set; }
    public DateTime? Deregistered { get; set; }
    public string CultureId { get; set; }
    public long ProviderId { get; set; }
    public LogEventType? LogLevel { get; set; }
    public string Password { get; set; }
    public List<string> Roles { get; set; }

    public class SaveUserValidator : AbstractValidator<SaveUser>
    {
        public SaveUserValidator()
        {
            RuleFor(u => u.Username).NotEmpty().EmailAddress();
            RuleFor(u => u.Phone).NotEmpty().PhoneNumber();
        }
    }
}

public class HandleSaveUser : ICommandHandler<SaveUser>
{
    private readonly IUserRepository _userRepository;
    private readonly ILinqProvider _linqProvider;
    private readonly IUserContext _userContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<HandleSaveUser> _logger;

    public  HandleSaveUser(
        IUserRepository userRepository,
        ILinqProvider linqProvider,
        IUserContext userContext,
        IPasswordHasher passwordHasher,
        ILogger<HandleSaveUser> logger)
    {
        _userRepository = userRepository;
        _linqProvider = linqProvider;
        _userContext = userContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Unit> Handle(SaveUser command, CancellationToken cancellationToken)
    {
        try
        {
            var identity = Guard.NotNull(_userContext.User.Identity);
            var identityType = identity.GetUserType();
            
            await _userContext.LoadSessionAsync();

            if (!_userContext.IsAdministrator)
                throw new NotEnoughPermissionsException();

            UserEntity entity;
            if (command.Id > 0)
            {
                entity = await _userRepository.FindByIdAsync(command.Id, cancellationToken);
                if (entity == null)
                    throw EntityNotFoundException.For<UserEntity>(command.Id);

                if (!string.IsNullOrEmpty(command.Password))
                    entity.PasswordHash = _passwordHasher.HashPassword(command.Password);
            }
            else
            {
                Guard.NotEmpty(command.Password);

                entity = new UserEntity(command.Username)
                {
                    PasswordHash = _passwordHasher.HashPassword(command.Password),
                    Email = command.Username,
                    Registered = DateTime.UtcNow,
                };
            }

            entity.Phone = command.Phone;

            var userType = Enum.Parse<UserType>(command.Type);
            entity.Type = userType > identityType ? userType : identityType;

            var providerId = identity.GetProviderId();
            if (!_userContext.IsSupervisor && command.ProviderId != providerId)
            {
                var provider = _linqProvider.ById<ProviderEntity>(command.ProviderId);
                entity.ProviderId = provider != null && provider.Parent == providerId ? command.ProviderId : providerId;
            }
            else
            {
                entity.ProviderId = command.ProviderId;
            }
            
            entity.LogLevel = command.LogLevel;

            await _userRepository.UpdateAsync(entity, cancellationToken);

            if (command.Roles.Any())
            {
                var userRoles = new List<string>();
                foreach (var commandRole in command.Roles)
                {
                    var role = await _userRepository.FindRoleByIdAsync(commandRole, cancellationToken);
                    if (role != null)
                        userRoles.Add(role.Id);
                }
                await _userRepository.UpdateUserRolesAsync(entity.Id, userRoles, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
        }

        return Unit.Value;
    }
}
