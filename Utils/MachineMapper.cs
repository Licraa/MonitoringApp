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
            new MachineIdentity { Id = 3, Name = "MESIN B01", Line = "Line B" },
            new MachineIdentity { Id = 4, Name = "MESIN B02", Line = "Line B" },
            new MachineIdentity { Id = 5, Name = "MESIN C01", Line = "Line C" },
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