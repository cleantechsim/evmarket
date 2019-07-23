
using System;
using System.Collections.Generic;

namespace CleanTechSim.MainPage.Helpers.Storage
{
    public interface IDataStorage
    {
        IEnumerable<T> GetAll<T>(Type type);
    }
}


