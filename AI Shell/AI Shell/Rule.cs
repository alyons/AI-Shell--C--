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

namespace AI_Shell
{
    [Flags]
    public enum RuleEvaluationFlags
    {
        None = 0x00,
        UsePriority = 0x01,
        UseWeight = UsePriority << 1,
        ReturnRandom = UseWeight << 1,
        Standard = UsePriority | UseWeight
    }

    public class Rule
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
        public RuleEvaluationFlags RuleEvaluationFlags
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        protected Rule()
        {
            conditions = new List<Condition>();
            subRules = new List<Rule>();
            ReturnSelf = true;
            this.Parent = null;
            Priority = Int32.MaxValue;
            Weight = 0;
            RuleEvaluationFlags = AI_Shell.RuleEvaluationFlags.Standard;
        }
        public Rule(String name)
            :this()
        {
            Name = name;
        }
        public Rule(String name, List<Condition> conditions)
            :this(name)
        {
            Conditions.AddRange(conditions);
        }
        #endregion

        #region Methods
        #region Sub-Rule Methods
        public bool AddSubRule(Rule subRule)
        {
            if (SubRuleExists(s => s.Equals(subRule))) return false;

            if (ConditionConflict(subRule)) return false;

            subRules.Add(subRule);

            return true;
        }
        public bool SubRuleExists(Func<Rule, bool> match)
        {
            return subRules.Any(match);
        }
        public Rule FindSubRule(Func<Rule, bool> match)
        {
            return subRules.First(match);
        }
        public void RemoveSubRule(Rule subRule)
        {
            subRules.Remove(subRule);
        }
        public void RemoveSubRuleAt(int index)
        {
            subRules.RemoveAt(index);
        }
        protected bool ConditionConflict(Rule other)
        {
            List<bool> conditionIsUnique = new List<bool>();
            List<bool> conditionIsEqual = new List<bool>();
            List<bool> conditionIsSubset = new List<bool>();
            List<Condition> ancestorConditions = new List<Condition>();

            Rule ancestor = this;

            while (ancestor != null)
            {
                foreach (Condition condition in ancestor.Conditions)
                    if (!ancestorConditions.Any(c => c.IsEqualSetTo(condition)))
                        ancestorConditions.Add(condition);

                ancestor = ancestor.Parent;
            }

            for (int i = 0; i < other.Conditions.Count; i++)
            {
                conditionIsUnique.Add(true);
                conditionIsEqual.Add(true);
                conditionIsSubset.Add(true);
            }

            for (int i = 0; i < other.Conditions.Count; i++)
            {
                conditionIsUnique[i] = !(ancestorConditions.Any(c => c.SimilarTo(other.Conditions[i])));
                conditionIsEqual[i] = ancestorConditions.Any(c => c.IsEqualSetTo(other.Conditions[i]));
                conditionIsSubset[i] = ancestorConditions.Any(c => c.IsProperSubsetOf(other.Conditions[i]));
            }

            for (int i = 0; i < conditionIsUnique.Count; i++)
                if (!conditionIsUnique[i])
                    if (!(conditionIsEqual[i] || conditionIsSubset[i]))
                        return true;

            if (conditionIsUnique.All(c => c == false))
                if (conditionIsSubset.Any(c => c == false))
                    return true;

            return false;
        }
        #endregion
        #region Condition Methods
        public bool AddCondition(Condition condition)
        {
            if (ConditionExists(c => c.Equals(condition))) return false;

            conditions.Add(condition);

            return true;
        }
        public bool ConditionExists(Func<Condition, bool> match)
        {
            return conditions.Any(match);
        }
        public Condition FindCondition(Func<Condition, bool> match)
        {
            return conditions.First(match);
        }
        public void RemoveCondition(Condition condition)
        {
            conditions.Remove(condition);
        }
        public void RemoveConditionAt(int index)
        {
            conditions.RemoveAt(index);
        }
        #endregion
        #region Variable List
        public List<String> VariableList()
        {
            List<String> output = new List<string>();

            foreach (Condition condition in Conditions)
            {
                if (!output.Contains(condition.FirstVariable))
                    output.Add(condition.FirstVariable);

                if (!output.Contains(condition.SecondVariable))
                    output.Add(condition.SecondVariable);
            }
                

            foreach (Rule subRule in SubRules)
                foreach (String variable in subRule.VariableList())
                    if (!output.Contains(variable))
                        output.Add(variable);

            return output;
        }
        #endregion
        #region Overriden Methods
        public override bool Equals(object obj)
        {
            if (obj is Rule)
            {
                return Name.Equals((obj as Rule).Name);
            }

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() * 97;
        }
        #endregion
        #region Evaluate Rules
        public virtual Rule EvaluateRules(Dictionary<string, object> dataDictionary)
        {
            return EvaluateRules(dataDictionary, RuleEvaluationFlags);
        }
        public virtual Rule EvaluateRules(Dictionary<string, object> dataDictionary, RuleEvaluationFlags ruleFlags)
        {
            if (Conditions.Any(c => !c.EvaluateCondition(dataDictionary))) return null;

            List<Rule> rulesMet = new List<Rule>();

            rulesMet = EvaluateSubRules(dataDictionary);
            if (ReturnSelf) rulesMet.Add(this);
            if ((ruleFlags & RuleEvaluationFlags.UsePriority) == RuleEvaluationFlags.UsePriority) HighestPriority(ref rulesMet);
            if ((ruleFlags & RuleEvaluationFlags.UseWeight) == RuleEvaluationFlags.UseWeight) HighestWeight(ref rulesMet);

            if (rulesMet.Count > 0)
                if ((ruleFlags & RuleEvaluationFlags.ReturnRandom) == RuleEvaluationFlags.ReturnRandom)
                    return RandomRule(ref rulesMet);
                else
                    return rulesMet[0];
            else
                return null;
            
        }
        protected virtual List<Rule> EvaluateSubRules(Dictionary<string, object> dataDictionary)
        {
            List<Rule> output = new List<Rule>();

            if ((RuleEvaluationFlags & RuleEvaluationFlags.ReturnRandom) == RuleEvaluationFlags.ReturnRandom)
            {
                foreach (Rule subRule in SubRules)
                {
                    output.AddRange(subRule.EvaluateAllRules(dataDictionary));
                }
            }
            else
            {
                foreach (Rule subRule in SubRules)
                {
                    output.Add(subRule.EvaluateRules(dataDictionary));
                }
            }

            return output;
        }
        protected virtual List<Rule> EvaluateAllRules(Dictionary<string, object> dataDictionary)
        {
            if (Conditions.Any(c => !c.EvaluateCondition(dataDictionary))) return null;

            List<Rule> rulesMet = new List<Rule>();

            rulesMet = EvaluateSubRules(dataDictionary);
            if (ReturnSelf) rulesMet.Add(this);
            if ((RuleEvaluationFlags & RuleEvaluationFlags.UsePriority) == RuleEvaluationFlags.UsePriority) HighestPriority(ref rulesMet);
            if ((RuleEvaluationFlags & RuleEvaluationFlags.UseWeight) == RuleEvaluationFlags.UseWeight) HighestWeight(ref rulesMet);

            if (rulesMet.Count > 0)
                return rulesMet;
            else
                return null;
        }
        protected virtual void HighestPriority(ref List<Rule> rules)
        {
            int lowestValue = Int32.MaxValue;
            foreach (Rule rule in rules) lowestValue = Math.Min(lowestValue, rule.Priority);
            rules.RemoveAll(r => r.Priority > lowestValue);
        }
        protected virtual void HighestWeight(ref List<Rule> rules)
        {
            int highestValue = Int32.MinValue;
            foreach (Rule rule in rules) highestValue = Math.Max(highestValue, rule.Weight);
            rules.RemoveAll(r => r.Weight < highestValue);
        }
        protected virtual Rule RandomRule(ref List<Rule> rules)
        {
            int max = 0;
            int target = 0;
            int runningNumber = 0;
            Random rnd = new Random();

            foreach (Rule rule in rules) max += rule.Weight;

            target = rnd.Next(max);

            foreach (Rule rule in rules)
            {
                runningNumber += rule.Weight;
                if (target <= runningNumber) return rule;
            }

            throw new Exception("Failed to return a random rule.");
        }
        #endregion
        #region Helper Methods
        public int CompareByWeight(Rule other)
        {
            return Weight.CompareTo(other.Weight);
        }
        public int CompareByPriority(Rule other)
        {
            return Priority.CompareTo(other.Priority);
        }
        #endregion
        #endregion
    }
}
