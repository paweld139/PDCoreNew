using PDCoreNew.Interfaces;
using System;

namespace PDCoreNew.Models
{
    public class EntityInfo<TKey> where TKey : IEquatable<TKey>
    {
        public EntityInfo(IEntity<TKey> entity)
        {
            Id = entity.Id;
        }

        public TKey Id { get; set; }
    }
}
