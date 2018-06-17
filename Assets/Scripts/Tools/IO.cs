#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Tools.Patterns;

namespace Tools
{
    public class IO : Singleton<IO>
    {
        public string FindScriptFilePath(string className, string startDir)
        {
            return FindFilePath(className, startDir, "cs");
        }

        public string FindFilePath(string name, string startDir, string ext)
        {
            try
            {
                foreach (var file in Directory.GetFiles(startDir))
                {
                    if (file.Contains(name + '.'+ ext))
                        return file.Replace('\\', '/');
                }
                foreach (var dir in Directory.GetDirectories(startDir))
                {
                    var value = FindFilePath(name, dir, ext);
                    if (value != null)
                    {
                        return value;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public void FindAllFile(string startDir, string ext, ref List<string> outputFullPaths)
        {
            try
            {
                foreach (var file in Directory.GetFiles(startDir))
                {
                    if (file.Contains('.' + ext) && !file.Contains(".meta"))
                    {
                        outputFullPaths.Add(file);
                    }
                }
                foreach (var dir in Directory.GetDirectories(startDir))
                {
                    FindAllFile(dir, ext, ref outputFullPaths);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
#endif