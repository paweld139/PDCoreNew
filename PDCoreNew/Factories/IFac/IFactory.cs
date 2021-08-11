namespace PDCoreNew.Factories.IFac
{
    public interface IFactory<TEnum, TElement> where TEnum : struct
    {
        TElement ExecuteCreation(TEnum type);
    }
}
