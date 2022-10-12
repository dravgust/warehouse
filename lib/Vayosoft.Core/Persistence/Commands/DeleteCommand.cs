using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Core.Persistence.Commands;

public record DeleteCommand<TEntity>(TEntity Entity) : ICommand where TEntity : IEntity;

public class DeleteCommandHandler<TKey, TEntity> : ICommandHandler<DeleteCommand<TEntity>>
    where TEntity : class, IEntity
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCommand<TEntity> command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command.Entity, nameof(command.Entity));

        var id = command.Entity.Id;

        var entity = _unitOfWork.Find<TEntity>(id);
        if (entity == null)
        {
            throw EntityNotFoundException.For<TEntity>(id);
        }

        _unitOfWork.Delete(entity);
        await _unitOfWork.CommitAsync();

        return Unit.Value;
    }
}
