using System;
using Ale2Project.Model;
using Ale2Project.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ale2ProjectUnitTest
{
    [TestClass]
    public class Assignment3Test
    {
        [TestMethod]
        public void GetAutomaton_Test()
        {
            //arrange
            RegularExpressionParserService regularExpressionParserService = new RegularExpressionParserService();
            string input = "|(.(a,c),|(*b,*d))";

            //act

            //assert
        }
    }
}
