namespace GitVisualizer.Interop;

public interface IStorageJsInterop
{
    Task<StorageEstimate> GetStorageEstimateAsync();
}
