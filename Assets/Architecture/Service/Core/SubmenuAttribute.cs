using System;

namespace Service.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SubmenuAttribute : Attribute
    {
        public string Path;

        public SubmenuAttribute(string path)
        {
            Path = path;
        }
    }
}