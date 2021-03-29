namespace PDCore.Interfaces
{
    public interface ISimpleType : IEntity<int>
    {
        string Type { get; set; }
    }
}
