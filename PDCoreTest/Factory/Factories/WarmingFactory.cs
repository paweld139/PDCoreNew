using PDCoreTest.Factory.Managers;

namespace PDCoreTest.Factory.Factories
{
    public class WarmingFactory : AirConditionerFactory
    {
        public override IAirConditioner Create(double temperature) => new WarmingManager(temperature);
    }
}
