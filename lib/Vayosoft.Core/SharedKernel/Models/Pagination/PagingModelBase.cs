using System;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public abstract class PagingModelBase : IPagingModel
    {
        private int _page;

        private int _size;

        protected PagingModelBase() :this(1, IPagingModel.DefaultSize) {}

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
                Guard.Assert(value >= 0, "Page must be >= 0");
                _page = value;
            }
        }

        public int Size
        {
            get => _size;
            set
            {
                Guard.Assert(value >= 0, "PageSize must be >= 0");
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

        protected PagingModelBase(Sorting<TEntity, TOrderKey> orderBy)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            OrderBy = orderBy ?? BuildDefaultSorting();

            Guard.NotNull(OrderBy, "OrderBy can't be null");
        }

        protected abstract Sorting<TEntity, TOrderKey> BuildDefaultSorting();

        public Sorting<TEntity, TOrderKey> OrderBy { get; set; }
    }
}
