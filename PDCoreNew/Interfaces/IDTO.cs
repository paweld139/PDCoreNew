namespace PDCoreNew.Interfaces
{
    public interface IDTO<TKey>
    {
        TKey Id { get; set; }
    }

    public interface IDTO : IDTO<int>
    {

    }
}
