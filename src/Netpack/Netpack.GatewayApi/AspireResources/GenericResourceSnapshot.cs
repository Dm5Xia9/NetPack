using Aspire.Hosting.ApplicationModel;
using Google.Protobuf.WellKnownTypes;
using Netpack.GatewayApi.AspireResources;

internal sealed class GenericResourceSnapshot(CustomResourceSnapshot state) : ResourceSnapshot
{
    public override string ResourceType => state.ResourceType;

    protected override IEnumerable<(string Key, Value Value, bool IsSensitive)> GetProperties()
    {
        foreach (var (key, value, isSensitive) in state.Properties)
        {
            var result = ConvertToValue(value);

            yield return (key, result, isSensitive);
        }
    }
}
