using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Persistence.Commands
{
    public class DeleteCommandHandler<TKey, TEntity> : ICommandHandler<DeleteCommand<TKey>>
        where TEntity : class, IEntity
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Unit> Handle(DeleteCommand<TKey> command, CancellationToken cancellationToken)
        {
            var entity = _unitOfWork.Find<TEntity>(command.Id);
            if (entity == null)
            {
                throw new ArgumentException($"Entity {typeof(TEntity).Name} with id={command.Id} doesn't exists");
            }

            _unitOfWork.Delete(entity);
            _unitOfWork.Commit();

            return Task.FromResult(Unit.Value);
        }
    }
}
