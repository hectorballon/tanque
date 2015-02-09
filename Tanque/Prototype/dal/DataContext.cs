using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Entity;
using System.Data.Entity;

namespace Prototype.dal
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
    }
}
