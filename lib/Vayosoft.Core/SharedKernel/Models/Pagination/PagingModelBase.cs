using System;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public abstract class PagingModelBase<TEntity, TOrderKey> : IPagingModel<TEntity, TOrderKey>
        where TEntity : class, IEntity
    {
        private int _page;

        private int _size;

        protected PagingModelBase() : this(null)
        { }

        protected PagingModelBase(int page, int size, Sorting<TEntity, TOrderKey> orderBy)
        {
            Page = page;
            PageSize = size;

            OrderBy = orderBy ?? throw new ArgumentException("OrderBy can't be null", nameof(orderBy));
        }

        protected PagingModelBase(Sorting<TEntity, TOrderKey> orderBy)
        {
            Page = 1;
            PageSize = 1000;

            // ReSharper disable once VirtualMemberCallInConstructor
            OrderBy = orderBy ?? BuildDefaultSorting();
            if (OrderBy == null)
            {
                throw new ArgumentException("OrderBy can't be null", nameof(OrderBy));
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

        public int PageSize
        {
            get => _size;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("take must be > 0", nameof(value));
                }

                _size = value;
            }
        }

        public Sorting<TEntity, TOrderKey> OrderBy { get; set; }
    }
}
