
using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;

using CleanTechSim.MainPage.Models.Domain;

using Microsoft.Azure.Cosmos.Table;

namespace CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage
{
    public class ReflectionMapperTest
    {

        [Test]
        public void TestMonthlyCarSalesDecoding()
        {
            ReflectionMapper reflectionMapper = new ReflectionMapper(typeof(MonthlyCountryEVCarSales));

            Dictionary<string, EntityProperty> properties = new Dictionary<string, EntityProperty>();

            properties["Sources"] = EntityProperty.GeneratePropertyForString(
                "http://testsource.com, http://anothersource.com"
            );

            properties["SalesPercent"] = EntityProperty.GeneratePropertyForDouble(34.6);
            MonthlyCountryEVCarSales sales = reflectionMapper.Convert<MonthlyCountryEVCarSales>(typeof(MonthlyCountryEVCarSales), "Norway", "2019_1", properties);

            Assert.IsNotNull(sales);
            Assert.AreEqual(sales.Country, "Norway");
            Assert.AreEqual(sales.Year, 2019);
            Assert.AreEqual(sales.Month, 1);
            Assert.AreEqual(sales.SalesPercent, 34.6m);
            Assert.AreEqual(sales.Sources, new string[] { "http://testsource.com", "http://anothersource.com" }.ToList());
        }
    }
}

