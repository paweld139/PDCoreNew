using PDCoreTest.Factory.Managers;

namespace PDCoreTest.Factory.Factories
{
    public class CoolingFactory : AirConditionerFactory
    {
        public override IAirConditioner Create(double temperature) => new CoolingManager(temperature);
    }
}
