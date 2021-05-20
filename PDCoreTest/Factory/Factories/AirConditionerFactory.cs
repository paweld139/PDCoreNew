using PDCoreTest.Factory.Managers;

namespace PDCoreTest.Factory.Factories
{
    public abstract class AirConditionerFactory
    {
        public abstract IAirConditioner Create(double temperature);
    }
}
