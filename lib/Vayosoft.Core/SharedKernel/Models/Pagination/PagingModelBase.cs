using System;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public abstract class PagingModelBase : IPagingModel
    {
        private int _page;

        private int _size;

        protected PagingModelBase(){}

        protected PagingModelBase(int page, int pageSize)
        {
            Page = page;
            Size = pageSize;
        }

        public int Page
        {
            get => _page;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Page must be >= 0", nameof(value));
                }

                _page = value;
            }
        }

        public int Size
        {
            get => _size;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("PageSize must be > 0", nameof(value));
                }

                _size = value;
            }
        }
    }

    public abstract class PagingModelBase<TEntity, TOrderKey> : PagingModelBase, IPagingModel<TEntity, TOrderKey>
        where TEntity : class, IEntity
    {
        protected PagingModelBase() : this(null)
        { }

        protected PagingModelBase(int page, int size, Sorting<TEntity, TOrderKey> orderBy) : base(page, size) {
            OrderBy = orderBy ?? throw new ArgumentException("OrderBy can't be null", nameof(orderBy));
        }

        protected PagingModelBase(Sorting<TEntity, TOrderKey> orderBy) : base(1, 30)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            OrderBy = orderBy ?? BuildDefaultSorting();
            if (OrderBy == null)
            {
                throw new ArgumentException("OrderBy can't be null", nameof(OrderBy));
            }
        }

        protected abstract Sorting<TEntity, TOrderKey> BuildDefaultSorting();

        public Sorting<TEntity, TOrderKey> OrderBy { get; set; }
    }
}
