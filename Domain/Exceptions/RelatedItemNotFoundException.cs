using System;
using Domain.Exceptions;

namespace Domain.Exceptions
{
    public sealed class RelatedItemNotFoundException:NotFoundException
    {
        public RelatedItemNotFoundException(int productId)
            : base($"Related product with the identifier {productId} was not found.")
        {
        }
    }
}

