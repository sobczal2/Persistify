﻿using System.Collections.Generic;

namespace Persistify.Management.Types.Abstractions;

public interface ITypeManagerFactory
{
    IEnumerable<ITypeManager> GetAll();

    ITypeManager<TQuery, THit> Get<TQuery, THit>()
        where TQuery : ITypeManagerQuery
        where THit : ITypeManagerHit;
}