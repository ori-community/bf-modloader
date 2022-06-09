using System.IO;

namespace Injector
{
    public class Config
    {
        public bool steam = false;
        public string path = null;

        public static Config FromFile(string file)
        {
            Config cfg = new Config();

            if (!File.Exists(file))
                return cfg;

            using var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] components = line.Split("=", 2);
                if (components.Length != 2)
                    continue;

                if (components[0] == "steam") cfg.steam = bool.Parse(components[1]);
                if (components[0] == "path") cfg.path = components[1];
            }

            return cfg;
        }

        public void WriteToFile(string filename)
        {
            File.WriteAllLines(filename, new string[] {
                $"steam={steam}",
                $"path={path}"
            });
        }
    }
}
