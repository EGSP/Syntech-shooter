using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ResourceSystems
{
    public class WeaponConfig
    {
        /// <summary>
        /// Имя оружия и ассетбандла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Редкость оружия
        /// </summary>
        public string Rarity { get; set; }
    }
}
