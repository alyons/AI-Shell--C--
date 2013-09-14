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
    public enum RuleEvaluationFlags
    {
        None = 0x00,
        UseTriggers = 0x01,
        UsePriority = UseTriggers << 1,
        UseWeight = UsePriority << 1,
        ReturnRandom = UseWeight << 1,
        Standard = UseTriggers | UsePriority | UseWeight,
        Question = Standard | ReturnRandom
    }

    public class Rule : IComparable<Rule>
    {
        #region Variables
        private List<Condition> conditions;
        private List<Rule> subRules;
        private List<String> triggers;
        const String hashModifier = "RulesForAIShellMetalGear";
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
        public List<String> Triggers
        {
            get { return triggers; }
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
        private Rule()
        {
            conditions = new List<Condition>();
            subRules = new List<Rule>();
            ReturnSelf = true;
            triggers = new List<String>();
            this.Parent = null;
            Priority = Int32.MaxValue;
            Weight = 0;
        }
        public Rule(String name)
            : this()
        {
            Name = name;
        }
        public Rule(String name, List<Condition> conditions)
            : this(name)
        {
            Conditions.AddRange(conditions);
        }
        public Rule(String name, List<Condition> conditions, List<String> triggers)
        {
            Triggers.AddRange(triggers);
        }
        #endregion

        #region Methods
        #region Sub-Rule Methods
        public bool AddSubRule(Rule subRule)
        {
            if (SubRuleExists(s => s.Equals(subRule))) return false;

            //if (!subRule.Triggers.TrueForAll(t => this.Triggers.Exists(o => o.Equals(t)))) return false;
            if (!subRule.Triggers.All(t => this.Triggers.Contains(t))) return false;

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
        #region Trigger Methods
        public bool AddTrigger(String trigger)
        {
            if (TriggerExists(t => t.Equals(trigger))) return false;

            triggers.Add(trigger);

            if (Parent != null) Parent.AddTrigger(trigger);

            return true;
        }
        public bool TriggerExists(Func<String, bool> match)
        {

            return triggers.Any(match);

        }
        public String FindTrigger(Func<String, bool> match)
        {
            return triggers.First(match);
        }
        public void RemoveTrigger(String trigger)
        {
            triggers.Remove(trigger);
        }
        public void RemoveTriggerAt(int index)
        {
            triggers.RemoveAt(index);
        }
        #endregion
        #region Variable List
        public List<String> VariableList(List<String> triggers)
        {
            List<String> output = new List<string>();

            foreach (String trigger in triggers)
            {
                if (TriggerExists(t => t.Equals(trigger)))
                {
                    foreach (Condition condition in Conditions)
                    {
                        if (!output.Contains(condition.FirstVariable))
                            output.Add(condition.FirstVariable);

                        if (!output.Contains(condition.SecondVariable))
                            output.Add(condition.SecondVariable);
                    }
                }
            }

            foreach (Rule subRule in SubRules)
                foreach (String variable in subRule.VariableList(triggers))
                    if (!output.Contains(variable))
                        output.Add(variable);

            return output;
        }
        public List<String> VariableList(String trigger)
        {
            List<String> triggers = new List<String>();
            triggers.Add(trigger);
            return VariableList(triggers);
        }
        public List<String> VariableList()
        {
            return VariableList(Triggers);
        }
        #endregion
        #region Overridden Methods
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
            return Name.GetHashCode() ^ hashModifier.GetHashCode();
        }
        public int CompareTo(Rule other)
        {
            return Name.CompareTo(other.Name);
        }
        #endregion
        #region Evaluate Rules
        public Rule EvaluateRules(Dictionary<string, object> dataDictionary, String trigger)
        {
            return EvaluateRules(dataDictionary, trigger, RuleEvaluationFlags.Standard);
        }
        public Rule EvaluateRules(Dictionary<string, object> dataDictionary, String trigger, RuleEvaluationFlags ruleFlags)
        {
            List<String> triggers = new List<String>();
            triggers.Add(trigger);
            return EvaluateRules(dataDictionary, triggers, ruleFlags);
        }
        public Rule EvaluateRules(Dictionary<string, object> dataDictionary, List<String> triggers)
        {
            return EvaluateRules(dataDictionary, triggers, RuleEvaluationFlags.Standard);
        }
        public Rule EvaluateRules(Dictionary<string, object> dataDictionary, List<String> triggers, RuleEvaluationFlags ruleFlags)
        {
            if (!(Triggers.Any(t => triggers.Contains(t)) || IsRoot)) return null;
            if (Conditions.Any(c => !c.EvaluateCondition(dataDictionary))) return null;

            List<Rule> rulesMet = new List<Rule>();

            rulesMet = EvaluateSubRules(dataDictionary, triggers, ruleFlags);
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
        protected List<Rule> EvaluateSubRules(Dictionary<string, object> dataDictionary, List<String> triggers, RuleEvaluationFlags ruleFlags)
        {
            List<Rule> output = new List<Rule>();

            if ((ruleFlags & RuleEvaluationFlags.ReturnRandom) == RuleEvaluationFlags.ReturnRandom)
            {
                foreach (Rule subRule in SubRules)
                {
                    output.AddRange(subRule.EvaluateAllRules(dataDictionary, triggers, ruleFlags));
                }
            }
            else
            {
                foreach (Rule subRule in SubRules)
                {
                    output.Add(subRule.EvaluateRules(dataDictionary, triggers, ruleFlags));
                }
            }

            return output;
        }
        protected List<Rule> EvaluateAllRules(Dictionary<string, object> dataDictionary, List<String> triggers, RuleEvaluationFlags ruleFlags)
        {
            if (!(Triggers.Any(t => triggers.Contains(t)) || IsRoot)) return null;
            if (Conditions.Any(c => !c.EvaluateCondition(dataDictionary))) return null;

            List<Rule> rulesMet = new List<Rule>();

            rulesMet = EvaluateSubRules(dataDictionary, triggers, ruleFlags);
            if (ReturnSelf) rulesMet.Add(this);
            if ((ruleFlags & RuleEvaluationFlags.UsePriority) == RuleEvaluationFlags.UsePriority) HighestPriority(ref rulesMet);
            if ((ruleFlags & RuleEvaluationFlags.UseWeight) == RuleEvaluationFlags.UseWeight) HighestWeight(ref rulesMet);

            if (rulesMet.Count > 0)
                return rulesMet;
            else
                return null;
        }
        protected void HighestPriority(ref List<Rule> rules)
        {
            int lowestPriority = Int32.MaxValue;
            foreach (Rule rule in rules) lowestPriority = Math.Min(lowestPriority, rule.Priority);
            List<String> asdfsafas = new List<string>();
            rules.RemoveAll(r => r.Priority != lowestPriority);
        }
        protected void HighestWeight(ref List<Rule> rules)
        {
            int highestWeight = Int32.MinValue;
            foreach (Rule rule in rules) highestWeight = Math.Max(highestWeight, rule.Weight);
            rules.RemoveAll(r => r.Weight != highestWeight);
        }
        protected Rule RandomRule(ref List<Rule> rules)
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

            throw new Exception("Failed to pull out a random rule.");
        }
        #endregion
        #endregion
    }
}