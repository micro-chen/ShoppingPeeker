using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ShoppingPeeker.Plugins
{
    public static class AssemblyExtentsion
    {
        /// <summary>
        /// 获取程序集所在的目录
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetDirectoryPath(this Assembly assembly)
        {
            string filePath = new Uri(assembly.CodeBase).LocalPath;
            return Path.GetDirectoryName(filePath);

        }
    }
}
