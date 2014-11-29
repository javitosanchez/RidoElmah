using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RidoElmah.Azure;
using System.Collections.Generic;
using System.Collections;

namespace RidoElmah.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetFirstPage()
        {
            IDictionary config = new Dictionary<string, string>();
            config.Add("connectionString", "UseDevelopmentStorage=true");
            TableErrorLog log = new TableErrorLog(config);
            IList errors = new List<ErrorEntity>();
            var ret = log.GetErrors(0, 100, errors);

            foreach (var item in errors)
            {
                Console.WriteLine(item);
            }
        }
    }
}
