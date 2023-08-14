using System.Collections.Generic;
using Persistify.Server.Persistence.Core.Documents;
using Persistify.Server.Persistence.Core.Repositories;

namespace Persistify.Server.Persistence.Core.Transactions;

public class TransactionDescriptor
{
    public bool WriteGlobal { get; set; }

    public List<IRepository> ReadRepositories { get; set; }
    public List<IRepository> WriteRepositories { get; set; }

    public TransactionDescriptor()
    {
        WriteGlobal = false;
        ReadRepositories = new List<IRepository>();
        WriteRepositories = new List<IRepository>();
    }
}
