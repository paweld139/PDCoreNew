namespace PDCoreNew.Interfaces
{
    public interface IDictionaryWithTag : ISimpleDictionary
    {
        string Tag { get; set; }
    }
}
