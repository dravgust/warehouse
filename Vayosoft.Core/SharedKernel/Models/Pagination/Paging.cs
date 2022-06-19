using System;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public abstract class Paging<TEntity, TOrderKey> : IPaging<TEntity, TOrderKey>
        where TEntity : class, IEntity
    {
        private readonly Sorting<TEntity, TOrderKey> _orderBy;

        private int _page;

        private int _take;

        protected Paging(int page, int take, Sorting<TEntity, TOrderKey> orderBy)
        {
            Page = page;
            Take = take;

            _orderBy = orderBy ?? throw new ArgumentException("OrderBy can't be null", nameof(orderBy));
        }

        protected Paging()
        {
            Page = 1;
            Take = 30;
            // ReSharper disable once VirtualMemberCallInConstructor
            _orderBy = BuildDefaultSorting();
            if (_orderBy == null)
            {
                throw new ArgumentException("OrderBy can't be null", nameof(_orderBy));
            }
        }

        protected abstract Sorting<TEntity, TOrderKey> BuildDefaultSorting();

        public int Page
        {
            get => _page;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("page must be >= 0", nameof(value));
                }

                _page = value;
            }
        }

        public int Take
        {
            get => _take;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("take must be > 0", nameof(value));
                }

                _take = value;
            }
        }

        public Sorting<TEntity, TOrderKey> OrderBy => _orderBy;
    }
}
