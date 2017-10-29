using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpirableListLib;
using System.Collections.Generic;
using System.Linq;

namespace ExpirableListTests
{
    [TestClass]
    public class ExpirableListTests
    {
        [TestMethod]
        public void CompleteListBeforeExpiracy_RaiseFinishCompletedEvent()
        {
            // Arrange
            bool raised = false;
            ExpirableList<string> expirableList = new ExpirableList<string>(500, 3);
            expirableList.ListFinished += (sender, e) => 
            {
                Assert.IsTrue(e.IsListComplete);
                Assert.AreEqual(3, e.Items.Count());
                raised = true;
            };

            expirableList.Add("item1");
            expirableList.Add("item2");
            expirableList.Add("item3");

            // Execute
            System.Threading.Thread.Sleep(600);

            // Assert
            Assert.IsTrue(raised);
            Assert.IsTrue(expirableList.IsFinished);
        }

        [TestMethod]
        public void ExpireListBeforeComplete_RaiseFinishIncompletedEvent()
        {
            // Arrange
            bool raised = false;
            ExpirableList<string> expirableList = new ExpirableList<string>(500, 3);
            expirableList.ListFinished += (sender, e) => 
            {
                Assert.IsFalse(e.IsListComplete);
                Assert.AreEqual(2, e.Items.Count());
                raised = true;
            };

            expirableList.Add("item1");
            expirableList.Add("item2");

            // Execute
            System.Threading.Thread.Sleep(600);

            // Assert
            Assert.IsTrue(raised);
            Assert.IsTrue(expirableList.IsFinished);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddingMoreItems_ThrowException()
        {
            // Arrange
            bool complete = false;
            ExpirableList<string> expirableList = new ExpirableList<string>(500, 3);
            expirableList.ListFinished += (sender, e) => { complete = e.IsListComplete; };
            expirableList.Add("item1");
            expirableList.Add("item2");
            expirableList.Add("item3");
            expirableList.Add("item4");

            // Execute
            System.Threading.Thread.Sleep(500);
        }

    }
}
