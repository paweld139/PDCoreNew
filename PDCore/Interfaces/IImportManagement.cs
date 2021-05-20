namespace PDCore.Interfaces
{
    public interface IImportManagement
    {
        string ImportFileName { get; }
        string ImportDefaultFileName { get; }
        void SaveDefaultImportFile();
        void SaveImportFile();
        void ClearData();
        void RestoreData();
    }
}
