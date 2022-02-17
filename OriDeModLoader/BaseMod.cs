using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OriDeModLoader
{
    public interface IMod
    {
        void Init();
        void Unload();

        string Name { get; }
    }
}
