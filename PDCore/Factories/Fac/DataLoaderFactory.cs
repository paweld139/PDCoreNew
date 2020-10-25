using PDCore.Enums;
using PDCore.Helpers;
using PDCore.Helpers.DataLoaders;
using PDCore.Interfaces;

namespace PDCore.Factories.Fac
{
    public class DataLoaderFactory : Factory<Loaders, IDataLoader>
    {
        protected override string ElementsNamespace => typeof(LocalLoader).Namespace;

        protected override string ElementsPostfix => "Loader";

        protected override void ConfigureContainer(Container container)
        {
        }
    }
}
