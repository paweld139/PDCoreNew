﻿using PDCore.Models.Shop;
using System.Collections.Generic;

namespace PDCore.Repositories.Repo.Shop.Interfaces
{
    public interface IShoppingCartRepository
    {
        (Product Product, int Quantity) Get(string articleId);
        IEnumerable<(Product Product, int Quantity)> All();
        void Add(Product product);
        void RemoveAll(string articleId);
        void IncreaseQuantity(string articleId);
        void DecraseQuantity(string articleId);
    }
}
