namespace PDCore.Interfaces
{
    public interface IDictionaryWithTag : ISimpleDictionary
    {
        string Tag { get; set; }
    }
}
