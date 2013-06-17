/// Copyright Alexander Lyons 2013
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

namespace AI_Shell
{
    [Flags]
    public enum NeoRuleEvaluationFlags
    {
        None = 0x00,
        UsePriority = 0x01,
        UseWeight = UsePriority << 1,
        ReturnRandom = UseWeight << 1,
    }

    public class NeoRule
    {
        #region Variables
        private List<Condition> conditions;
        private List<Rule> subRules;
        #endregion

        #region Properties
        public List<Condition> Conditions
        {
            get { return conditions; }
            protected set { conditions = value; }
        }
        public List<Rule> SubRules
        {
            get { return subRules; }
            protected set { subRules = value; }
        }
        public String Name
        {
            get;
            set;
        }
        public bool ReturnSelf
        {
            get;
            set;
        }
        public bool IsRoot
        {
            get;
            set;
        }
        public int Priority
        {
            get;
            set;
        }
        public int Weight
        {
            get;
            set;
        }
        public Rule this[int index]
        {
            get { return subRules[index]; }
            set { subRules[index] = value; }
        }
        public Rule this[String name]
        {
            get { return subRules.First(r => r.Name.Equals(name)); }
        }
        public int Count
        {
            get
            {
                int count = 1;
                foreach (Rule rule in subRules) count += rule.Count;
                return count;
            }
        }
        public Rule Parent
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
