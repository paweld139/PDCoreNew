﻿using PDCore.Models.Shop;
using PDCore.Repositories.Repo.Shop.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PDCore.Repositories.Repo.Shop
{
    public class ProductsRepository : IProductRepository
    {
        private readonly Dictionary<string, (Product Product, int Stock)> products;

        public ProductsRepository()
        {
            products = new Dictionary<string, (Product Product, int Stock)>();

            Add(new Product("EOSR1", "Canon EOS R", 1099), 2);
            Add(new Product("EOS70D", "Canon EOS 70D", 699), 1);
            Add(new Product("ATOMOSNV", "Atomos Ninja V", 799), 0);
            Add(new Product("SM7B", "Shure SM7B", 399), 5);
        }

        public void Add(Product product, int stock)
        {
            products[product.ArticleId] = (product, stock);
        }

        public void DecreaseStockBy(string articleId, int amount)
        {
            if (!products.ContainsKey(articleId)) return;

            products[articleId] =
                (products[articleId].Product, products[articleId].Stock - amount);
        }

        public void IncreaseStockBy(string articleId, int amount)
        {
            if (!products.ContainsKey(articleId)) return;

            products[articleId] =
                (products[articleId].Product, products[articleId].Stock + amount);
        }

        public IEnumerable<Product> All()
        {
            return products.Select(x => x.Value.Product);
        }

        public int GetStockFor(string articleId)
        {
            if (products.ContainsKey(articleId))
            {
                return products[articleId].Stock;
            }

            return 0;
        }

        public Product FindBy(string articleId)
        {
            if (products.ContainsKey(articleId))
            {
                return products[articleId].Product;
            }

            return null;
        }
    }
}
