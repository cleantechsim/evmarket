
using System;

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage
{
    public abstract class CompositeKeyAttribute : Attribute
    {
        public string name;

        public int Order { get; }

        public CompositeKeyAttribute(int order)
        {

        }
    }
}