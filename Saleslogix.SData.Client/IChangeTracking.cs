namespace Saleslogix.SData.Client
{
    public interface IChangeTracking : System.ComponentModel.IChangeTracking
    {
        object GetChanges();
    }
}