using System.Reflection;

namespace ZenBlog.Persistance;

public static class AssemblyReference
{
    public static readonly Assembly assembly = typeof(Assembly).Assembly;
}
