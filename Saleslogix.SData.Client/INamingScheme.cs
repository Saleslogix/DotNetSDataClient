using System.Reflection;

namespace Sage.SData.Client
{
    public interface INamingScheme
    {
        string GetName(MemberInfo member);
    }
}