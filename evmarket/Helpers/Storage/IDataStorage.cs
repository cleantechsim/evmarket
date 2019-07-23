
using System;
using System.Collections.Generic;

namespace CleanTechSim.EVMarket.Helpers.Storage
{
    public interface IDataStorage
    {
        IEnumerable<T> GetAll<T>(Type type);
    }
}


