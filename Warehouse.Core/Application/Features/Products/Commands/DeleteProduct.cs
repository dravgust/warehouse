﻿using FluentValidation;
using Vayosoft.Core.SharedKernel.Commands;

namespace Warehouse.Core.Application.Features.Products.Commands
{
    public class DeleteProduct : ICommand
    {
        public string Id { get; set; }
        public class CertificateRequestValidator : AbstractValidator<DeleteProduct>
        {
            public CertificateRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }
}
