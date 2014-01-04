namespace QueryManager
{
    public interface IDataStorage
    {
        void Initialize();
        string ConnectionString { get; set; }
    }
}