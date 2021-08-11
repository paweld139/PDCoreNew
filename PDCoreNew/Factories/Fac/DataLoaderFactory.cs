using PDCoreNew.Enums;
using PDCoreNew.Helpers;
using PDCoreNew.Helpers.DataLoaders;
using PDCoreNew.Interfaces;

namespace PDCoreNew.Factories.Fac
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
