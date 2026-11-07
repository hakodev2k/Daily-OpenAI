namespace FreightManifest;

public sealed class BatchRunner
{
    private readonly ImportCoordinator _coordinator;
    public BatchRunner(ImportCoordinator coordinator) => _coordinator = coordinator;
    public ManifestLine Execute(string row) => _coordinator.Import(row);
}
