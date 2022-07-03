using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Persistence.Commands
{
    public class CreateOrUpdateHandler<TKey, TEntity> : ICommandHandler<CreateOrUpdateCommand>
        where TEntity : class, IEntity<TKey>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateOrUpdateHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<Unit> Handle(CreateOrUpdateCommand command, CancellationToken cancellationToken)
        {
            var id = command.Id;
            var entity = id != null && !default(TKey)!.Equals(id)
                ? _mapper.Map(command, _unitOfWork.Find<TEntity>(id))
                : _mapper.Map<TEntity>(command);

            _unitOfWork.Add(entity);
            _unitOfWork.Commit();

            return Task.FromResult(Unit.Value);
        }
    }
}
