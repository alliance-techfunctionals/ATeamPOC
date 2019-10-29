namespace Ex8.Helper.Assembly
{
    using System;
    using System.Reflection;

    public static class AssemblyHelper
    {
        public static string GetCurrentAssemblyPath()
        {
            var asm = Assembly.GetEntryAssembly();
            var fileDirectory = FileSystem.DirectoryHelper.GetDirectoryName(asm.Location)?.Replace(asm.FullName, string.Empty);
            return fileDirectory;
        }

        public static string GetCurrentExecutingAssemblyPath()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fileDirectory = FileSystem.DirectoryHelper.GetDirectoryName(asm.Location)?.Replace(asm.FullName, string.Empty);
            return fileDirectory;
        }

        public static Type GetTypeFromCurrentDomain(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}