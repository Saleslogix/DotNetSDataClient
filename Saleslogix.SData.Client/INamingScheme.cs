using System.Reflection;

namespace Saleslogix.SData.Client
{
    public interface INamingScheme
    {
        string GetName(MemberInfo member);
    }
}