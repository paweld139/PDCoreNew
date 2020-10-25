namespace PDCore.Common.Context.IContext
{
    public interface IMainDbContext : IEntityFrameworkDbContext, IHasLogDbSet, IHasFileDbSet
    {

    }
}
