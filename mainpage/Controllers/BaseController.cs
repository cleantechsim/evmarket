
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using CleanTechSim.MainPage.Models.Domain;

using CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage;


namespace CleanTechSim.MainPage.Controllers
{
    public abstract class BaseController : Controller
    {
        private AzureTableStorage storage;

        public BaseController()
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING");

            this.storage = new AzureTableStorage(
                connectionString,
                typeof(MonthlyCountryEVCarSales),
                typeof(BatteryCost),
                typeof(Vehicle));
        }

        internal IEnumerable<T> GetAll<T>(Type type)
        {
            return storage.GetAll<T>(type);
        }
    }
}
