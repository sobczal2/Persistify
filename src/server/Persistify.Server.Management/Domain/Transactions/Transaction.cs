using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Management.Domain.Transactions;

public class Transaction
{
    public bool Write { get; set; }
    public List<int> TemplateIds { get; set; }
    public bool Global { get; set; }
    public Stack<Func<ValueTask>> RollbackActions;

    public Transaction()
    {
        Write = false;
        TemplateIds = new List<int>(1);
        Global = false;
        RollbackActions = new Stack<Func<ValueTask>>();
    }
}
