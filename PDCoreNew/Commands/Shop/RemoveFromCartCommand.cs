﻿using PDCoreNew.Models.Shop;
using PDCoreNew.Repositories.Repo.Shop.Interfaces;
using System;

namespace PDCoreNew.Commands.Shop
{
    public class RemoveFromCartCommand : ICommand
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductRepository productRepository;
        private readonly Product product;

        public RemoveFromCartCommand(IShoppingCartRepository shoppingCartRepository,
            IProductRepository productRepository,
            Product product)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.product = product;
        }

        public bool CanExecute()
        {
            if (product == null) return false;

            return shoppingCartRepository.Get(product.ArticleId).Quantity > 0;
        }

        public void Execute()
        {
            if (product == null) return;

            var lineItem = shoppingCartRepository.Get(product.ArticleId);

            productRepository.IncreaseStockBy(product.ArticleId, lineItem.Quantity);

            shoppingCartRepository.RemoveAll(product.ArticleId);
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
