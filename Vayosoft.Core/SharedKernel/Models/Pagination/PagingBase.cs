using System;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public abstract class PagingBase<TEntity, TOrderKey> : IPagingModel<TEntity, TOrderKey>
        where TEntity : class, IEntity
    {
        private readonly Sorting<TEntity, TOrderKey> _orderBy;

        private int _page;

        private int _size;

        protected PagingBase(int page, int size, Sorting<TEntity, TOrderKey> orderBy)
        {
            Page = page;
            Size = size;

            _orderBy = orderBy ?? throw new ArgumentException("OrderBy can't be null", nameof(orderBy));
        }

        protected PagingBase()
        {
            Page = 1;
            Size = 30;
            
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

        public int Size
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

        public Sorting<TEntity, TOrderKey> OrderBy => _orderBy;
    }
}
