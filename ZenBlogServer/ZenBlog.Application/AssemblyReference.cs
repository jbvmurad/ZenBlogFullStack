using System.Reflection;

namespace ZenBlog.Application;

public static class AssemblyReference
{
    public static readonly Assembly assembly = typeof(Assembly).Assembly;
}
