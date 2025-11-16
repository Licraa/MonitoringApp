using System.Collections.Generic;
using System.Linq;
using MachineMonitoringApp.Models;

namespace MachineMonitoringApp.Utils
{
    public static class MachineMapper
    {
        private static readonly List<MachineIdentity> _identities = new List<MachineIdentity>
        {
            new MachineIdentity { Id = 1, Name = "MESIN A01", Line = "Line A" },
            new MachineIdentity { Id = 2, Name = "MESIN A02", Line = "Line A" },
            new MachineIdentity { Id = 3, Name = "MESIN B01", Line = "Line C" },
            new MachineIdentity { Id = 4, Name = "MESIN B02", Line = "Line D" },
            new MachineIdentity { Id = 5, Name = "MESIN C01", Line = "Line E" },
            new MachineIdentity { Id = 6, Name = "MESIN A01", Line = "Line A" },
            new MachineIdentity { Id = 7, Name = "MESIN A02", Line = "Line A" },
            new MachineIdentity { Id = 8, Name = "MESIN B01", Line = "Line C" },
            new MachineIdentity { Id = 9, Name = "MESIN B02", Line = "Line D" },
            new MachineIdentity { Id = 11, Name = "MESIN C01", Line = "Line E" },
            new MachineIdentity { Id = 12, Name = "MESIN C01", Line = "Line G" },
            new MachineIdentity { Id = 13, Name = "MESIN C01", Line = "Line H" },
            new MachineIdentity { Id = 14, Name = "MESIN C01", Line = "Line I" },
            new MachineIdentity { Id = 15, Name = "MESIN C01", Line = "Line J" },
            new MachineIdentity { Id = 16, Name = "MESIN C01", Line = "Line K" },
            // ... Tambahkan mesin lainnya di sini ...
        };

        public static MachineIdentity GetIdentityById(int id)
        {
            return _identities.FirstOrDefault(i => i.Id == id);
        }

        public static List<MachineIdentity> GetAllIdentities()
        {
            return _identities;
        }
    }
}