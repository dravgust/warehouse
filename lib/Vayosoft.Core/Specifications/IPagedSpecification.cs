﻿using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Core.Specifications
{
    public interface IPagedSpecification<TEntity, TOrderKey> :
        ISpecification2<TEntity>, IPagingModel<TEntity, TOrderKey>
        where TEntity : class, IEntity
    { }
}
