/// Copyright Alexander Lyons 2013, 2014
///
/// This file is part of AI Shell.
///
/// AI Shell is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
/// 
/// AI Shell is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with AI Shell.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AI_Shell;

namespace AIShellLibraryTest
{
    [TestFixture]
    public class ConditionTest
    {
        List<Condition> conditions = new List<Condition>();

        [Test]
        public void EvaluateConditionTest()
        {
            SetUp();
            bool actual = false;
            int data0, data1;

            data0 = 7;
            data1 = 34;
            actual = conditions[0].EvaluateCondition(data0, data1);
            Assert.AreEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[0].ToString()));

            data0 = 77;
            actual = conditions[0].EvaluateCondition(data0, data1);
            Assert.AreNotEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[0].ToString()));

            data0 = 34;
            actual = conditions[0].EvaluateCondition(data0, data1);
            Assert.AreNotEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[0].ToString()));

            data0 = 7;
            data1 = 34;
            actual = conditions[1].EvaluateCondition(data0, data1);
            Assert.AreNotEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[1].ToString()));

            data0 = 77;
            actual = conditions[1].EvaluateCondition(data0, data1);
            Assert.AreEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[1].ToString()));

            data0 = 34;
            actual = conditions[1].EvaluateCondition(data0, data1);
            Assert.AreEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[1].ToString()));

            List<string> set0 = new List<string> { "Bulbasaur", "Charmander", "Squirtle", "Pikachu" };
            List<string> set1 = new List<string> { "Bulbasaur", "Charmander", "Squirtle", "Pikachu", "Chikorita", "Cyndaquil", "Totodile", "Treeko", "Torchic", "Mudkip", "Turtwig", "Chimchar", "Piplup", "Snivy", "Tepig", "Oshawott" };

            actual = conditions[2].EvaluateCondition(set0, set1);
            Assert.AreEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[2].ToString()));

            string data2 = "Kadabra";
            string data3 = "Abra";

            actual = conditions[3].EvaluateCondition(data2, data3);
            Assert.AreEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[3].ToString()));

            actual = conditions[5].EvaluateCondition(data2, set1);
            Assert.AreEqual(true, actual, String.Format("Failed to evaluate the condition correctly. {0}", conditions[5].ToString()));
        }

        [Test]
        public void ExistsInTestCase()
        {
            List<string> set0 = new List<string>();
            string data0 = "Charmander";
            bool actual = false;
            Condition conditition = new Condition("data0", "set0", Relation.ExistsIn);

            set0.Add("Bulbasaur");
            set0.Add("Charmander");
            set0.Add("Squirtle");
            set0.Add("Pikachu");

            actual = conditition.EvaluateCondition(data0, set0);
            Assert.AreEqual(true, actual, "Failed to evaluate exists in for: " + conditition.ToString());

            data0 = "Cyndaquil";

            actual = conditition.EvaluateCondition(data0, set0);
            Assert.AreNotEqual(true, actual, "Failed to evaluate exists in for: " + conditition.ToString());
        }

        protected void SetUp()
        {
            conditions.Add(new Condition("data0", "data1", Relation.LessThan));
            conditions.Add(new Condition("data0", "data1", Relation.GreaterThanOrEqualTo));
            conditions.Add(new Condition("set0", "set1", Relation.IsProperSubSetOf));
            conditions.Add(new Condition("data2", "data3", Relation.GreaterThan));
            conditions.Add(new Condition("set1", "set0", Relation.IsProperSuperSetTo));
            conditions.Add(new Condition("data2", "set1", Relation.DoesNotExistIn));
        }
    }
}

