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
using System.Collections;

namespace AI_Shell
{
    [Flags]
    public enum Relation
    {
        None = 0x00,
        LessThan = 0x01,
        EqualTo = LessThan << 1,
        GreaterThan = EqualTo << 1,
        IsProperSubSetOf = GreaterThan << 1,
        IsEqualSetTo = IsProperSubSetOf << 1,
        IsProperSuperSetTo = IsEqualSetTo << 1,
        ExistsIn = IsProperSuperSetTo << 1,
        DoesNotExistIn = ExistsIn << 1,
        LessThanOrEqualTo = LessThan | EqualTo,
        GreaterThanOrEqualTo = GreaterThan | EqualTo,
        NotEqualTo = LessThan | GreaterThan,
        IsSubsetOf = IsProperSubSetOf | IsEqualSetTo,
        IsSuperSetTo = IsProperSuperSetTo | IsEqualSetTo,
        OrderRelation = LessThan | EqualTo | GreaterThan,
        SetRelation = IsProperSubSetOf | IsEqualSetTo | IsProperSuperSetTo,
        ExistanceRelation = ExistsIn | DoesNotExistIn
    }

    public class Condition
    {
        #region Variables
        const string toStringFormat = "{0} {1} {2} {3} {4}";
        private Relation goal;
        #endregion

        #region Properties
        public string FirstVariable { get; set; }
        public string SecondVariable { get; set; }
        public double FirstVariableModifier { get; set; }
        public double SecondVariableModifier { get; set; }
        public Relation Goal
        {
            get { return goal; }
            set
            {
                CheckRelation(value);
                goal = value;
            }
        }
        #endregion

        #region Constructors
        public Condition(string firstVar, string secondVar, Relation goal)
        {
            FirstVariable = firstVar;
            SecondVariable = secondVar;
            FirstVariableModifier = 1.0;
            SecondVariableModifier = 1.0;
            Goal = goal;
        }
        public Condition(string firstVar, string secondVar, Relation goal, double firstVarMod, double secondVarMod)
            : this(firstVar, secondVar, goal)
        {
            FirstVariableModifier = firstVarMod;
            SecondVariableModifier = secondVarMod;
        }
        #endregion

        #region Methods
        private void CheckRelation(Relation relation)
        {
            //If the relation has an illegal value, throw an error
            if ((((relation & Relation.OrderRelation) != 0) && ((relation & Relation.SetRelation) != 0)) ||
                (((relation & Relation.OrderRelation) != 0) && ((relation & Relation.ExistanceRelation) != 0)) ||
                (((relation & Relation.SetRelation) != 0) && ((relation & Relation.ExistanceRelation) != 0)))
                throw new ArgumentException("Cannot perform numeric, set, and existance comparison at the same time.");
            if ((relation & Relation.OrderRelation) == Relation.OrderRelation) throw new ArgumentException("Cannot perform comparison of a is less than, greater than, or equal to b.");
            if ((relation & Relation.SetRelation) == Relation.SetRelation) throw new ArgumentException("Cannot perform comparison of set a is a subset, equal set, or super set to set b.");
            if (((relation & Relation.IsProperSubSetOf) == Relation.IsProperSubSetOf) && (relation & Relation.IsProperSuperSetTo) == Relation.IsProperSuperSetTo) throw new ArgumentException("Cannot perform comparison of proper subset and proper super set.");
            if ((relation & Relation.ExistanceRelation) == Relation.ExistanceRelation) throw new ArgumentException("Cannot perform comparison of a exists or does not exist in subset b.");
            if (relation == Relation.None) throw new ArgumentException("Cannot perform comparison of no relation.");
        }
        public bool EvaluateCondition(Dictionary<string, object> dataDictionary)
        {
            return EvaluateCondition(dataDictionary[FirstVariable], dataDictionary[SecondVariable]);
        }
        public bool EvaluateCondition(object data0, object data1)
        {
            if ((Goal & Relation.OrderRelation) != 0)
            {
                if (IsNumeric(data0) && IsNumeric(data1))
                {
                    try
                    {
                        double num0, num1;
                        num0 = FirstVariableModifier * Convert.ToDouble(data0);
                        num1 = SecondVariableModifier * Convert.ToDouble(data1);

                        if ((Goal & Relation.LessThan) == Relation.LessThan)
                            if (num0 < num1)
                                return true;

                        if ((Goal & Relation.EqualTo) == Relation.EqualTo)
                            if (num0 == num1)
                                return true;

                        if ((Goal & Relation.GreaterThan) == Relation.GreaterThan)
                            if (num0 > num1)
                                return true;
                    }
                    catch (OverflowException)
                    {
                        decimal num2, num3;
                        num2 = Convert.ToDecimal(FirstVariableModifier) * Convert.ToDecimal(data0);
                        num3 = Convert.ToDecimal(SecondVariableModifier) * Convert.ToDecimal(data1);

                        if ((Goal & Relation.LessThan) == Relation.LessThan)
                            if (num2 < num3)
                                return true;

                        if ((Goal & Relation.EqualTo) == Relation.EqualTo)
                            if (num2 == num3)
                                return true;

                        if ((Goal & Relation.GreaterThan) == Relation.GreaterThan)
                            if (num2 > num3)
                                return true;
                    }
                }
                else
                {
                    if (!data0.GetType().Equals(data1.GetType())) throw new ArgumentException("The data sets must be of the same type to be compared.");

                    if (data0 is IComparable && data1 is IComparable)
                    {
                        IComparable comp0 = data0 as IComparable;
                        IComparable comp1 = data1 as IComparable;

                        if ((Goal & Relation.LessThan) == Relation.LessThan)
                            if (comp0.CompareTo(comp1) < 0)
                                return true;

                        if ((Goal & Relation.EqualTo) == Relation.EqualTo)
                            if (comp0.CompareTo(comp1) == 0)
                                return true;

                        if ((Goal & Relation.GreaterThan) == Relation.GreaterThan)
                            if (comp0.CompareTo(comp1) > 0)
                                return true;
                    }
                    else
                    {
                        throw new ArgumentException("The data must be comparable to use this function (with interface IComparable).");
                    }
                }
            }
            else if ((Goal & Relation.SetRelation) != 0)
            {
                if (IsList(data0) && IsList(data1))
                {
                    IList list0 = data0 as IList;
                    IList list1 = data1 as IList;
                    List<object> set0 = new List<object>();
                    List<object> set1 = new List<object>();

                    if (!list0[0].GetType().Equals(list1[0].GetType())) throw new ArgumentException("The data sets must be of the same type to be compared.");

                    if ((list0[0] is IComparable) && (list1[0] is IComparable))
                    {
                        foreach (object obj in list0) set0.Add(obj);
                        foreach (object obj in list1) set1.Add(obj);

                        Func<object, object, bool> existsFunction = (obj0, obj1) => obj0.Equals(obj1);

                        /*if ((Goal & Relation.IsProperSubSetOf) == Relation.IsProperSubSetOf)
                            if (set0.TrueForAll(obj0 => set1.Exists(obj1 => obj0.Equals(obj1))))
                                if (!(set1.TrueForAll(obj1 => set0.Exists(obj0 => obj1.Equals(obj0)))))
                                    return true;*/

                        if ((Goal & Relation.IsProperSubSetOf) == Relation.IsProperSubSetOf)
                            if (!(set0.Except(set1).Any()))
                                if (set1.Except(set0).Any())
                                    return true;

                        /*if ((Goal & Relation.IsEqualSetTo) == Relation.IsEqualSetTo)
                            if (set0.TrueForAll(obj0 => set1.Exists(obj1 => obj0.Equals(obj1))))
                                if (set1.TrueForAll(obj1 => set0.Exists(obj0 => obj1.Equals(obj0))))
                                    return true;*/

                        if ((Goal & Relation.IsEqualSetTo) == Relation.IsEqualSetTo)
                            if (!(set0.Except(set1).Any()))
                                if (!set1.Except(set0).Any())
                                    return true;


                        /*if ((Goal & Relation.IsProperSuperSetTo) == Relation.IsProperSuperSetTo)
                            if (!(set0.TrueForAll(obj0 => set1.Exists(obj1 => obj0.Equals(obj1)))))
                                if (set1.TrueForAll(obj1 => set0.Exists(obj0 => obj1.Equals(obj0))))
                                    return true;*/

                        if ((Goal & Relation.IsProperSuperSetTo) == Relation.IsProperSuperSetTo)
                            if (set0.Except(set1).Any())
                                if (!(set1.Except(set0).Any()))
                                    return true;
                    }
                    else
                    {
                        throw new ArgumentException("The data must be comparable to use this function (with interface IComparable).");
                    }
                }
                else
                {
                    throw new ArgumentException("For set operations, the data must be lists (having interface IList).");
                }
            }
            else if ((Goal & Relation.ExistanceRelation) != 0)
            {
                if (IsList(data1))
                {
                    IList list1 = data1 as IList;

                    if (!data0.GetType().Equals(list1[0].GetType())) throw new ArgumentException("The data sets must be of the same type to be compared.");

                    if (data0 is IComparable && list1[0] is IComparable)
                    {
                        List<object> set1 = new List<object>();
                        foreach (object obj in list1) set1.Add(obj);

                        if ((Goal & Relation.ExistsIn) == Relation.ExistsIn)
                            if (set1.Any(i => (i as IComparable).CompareTo(data0) == 0))
                                return true;

                        if ((Goal & Relation.DoesNotExistIn) == Relation.DoesNotExistIn)
                            if (!set1.Any(i => (i as IComparable).Equals(data0)))
                                return true;
                    }
                    else
                    {
                        throw new ArgumentException("The data must be comparable to use this function (with interface IComparable).");
                    }
                }
                else
                {
                    throw new ArgumentException("For existance operations, the data must be a single item and a list (having interface IList).");
                }
            }

            return false;
        }
        public static bool IsNumeric(object value)
        {
            return value is short
                || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
        public static bool IsList(object value)
        {
            return (value is IList && value.GetType().IsGenericType);
        }
        public override string ToString()
        {
            string[] toString = new string[5];
            toString[0] = (FirstVariableModifier == 1.0) ? "" : FirstVariableModifier + " *";
            toString[1] = "" + FirstVariable;
            toString[2] = Goal.ToString();
            toString[3] = (SecondVariableModifier == 1.0) ? "" : SecondVariableModifier + " *";
            toString[4] = "" + SecondVariable;
            return String.Format(toStringFormat, toString);
        }
        public bool IsProperSubsetOf(Condition other)
        {
            if (IsEqualSetTo(other)) return false;
            if (!SimilarTo(other)) return false;
            if (FirstVariable.Equals(other.SecondVariable) && SecondVariable.Equals(other.FirstVariable))
                other = Condition.Reorder(other);

            double ratioThis = FirstVariableModifier / SecondVariableModifier;
            double ratioOther = other.FirstVariableModifier / other.SecondVariableModifier;

            switch (Goal)
            {
                case Relation.LessThan:
                    if (other.Goal == Relation.LessThan && ratioThis > ratioOther)
                        return true;
                    else if (other.Goal == Relation.LessThanOrEqualTo && ratioThis >= ratioOther)
                        return true;
                    else
                        return false;
                case Relation.LessThanOrEqualTo:
                    if (other.Goal == Relation.LessThanOrEqualTo && ratioThis > ratioOther)
                        return true;
                    else
                        return false;
                case Relation.GreaterThan:
                    if (other.Goal == Relation.GreaterThan && ratioThis < ratioOther)
                        return true;
                    else if (other.Goal == Relation.GreaterThanOrEqualTo && ratioThis <= ratioOther)
                        return true;
                    else
                        return false;
                case Relation.GreaterThanOrEqualTo:
                    if (other.Goal == Relation.GreaterThanOrEqualTo && ratioThis < ratioOther)
                        return true;
                    else
                        return false;
                case Relation.EqualTo:
                    if (other.Goal == Relation.LessThanOrEqualTo && ratioThis == ratioOther)
                        return true;
                    else if (other.Goal == Relation.GreaterThanOrEqualTo && ratioThis == ratioOther)
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }
        public bool IsEqualSetTo(Condition other)
        {
            if (!((FirstVariable.Equals(other.FirstVariable) && SecondVariable.Equals(other.SecondVariable)) || (FirstVariable.Equals(other.SecondVariable) && SecondVariable.Equals(other.FirstVariable))))
                return false;

            if (FirstVariable.Equals(other.SecondVariable) && SecondVariable.Equals(other.FirstVariable))
                other = Condition.Reorder(other);

            if (Goal != other.Goal)
                return false;

            if ((FirstVariableModifier / SecondVariableModifier) != (other.FirstVariableModifier / other.SecondVariableModifier))
                return false;

            return true;
        }
        public bool SimilarTo(Condition other)
        {
            return ((FirstVariable.Equals(other.FirstVariable) && SecondVariable.Equals(other.SecondVariable)) || (FirstVariable.Equals(other.SecondVariable) && SecondVariable.Equals(other.FirstVariable)));
        }
        public static Condition Reorder(Condition condition)
        {
            if ((condition.Goal & Relation.ExistanceRelation) != 0) throw new ArgumentException("Cannot reorder existance conditions");

            Relation newGoal = Relation.None;

            if ((condition.Goal & Relation.OrderRelation) != 0)
            {
                newGoal = ((~condition.Goal & Relation.LessThan) | (~condition.Goal & Relation.GreaterThan) | (condition.Goal & Relation.EqualTo)) & Relation.OrderRelation;
            }

            if ((condition.Goal & Relation.SetRelation) != 0)
            {
                newGoal = ((~condition.Goal & Relation.IsProperSubSetOf) | (~condition.Goal & Relation.IsProperSuperSetTo) | (condition.Goal & Relation.IsEqualSetTo)) & Relation.SetRelation;
            }

            return new Condition(condition.SecondVariable, condition.FirstVariable, newGoal, condition.SecondVariableModifier, condition.FirstVariableModifier);
        }
        public static Condition Invert(Condition condition)
        {
            Relation newGoal = Relation.None;

            if ((condition.Goal & Relation.OrderRelation) != 0)
            {
                newGoal = ((~condition.Goal & Relation.LessThan) | (~condition.Goal & Relation.GreaterThan) | (condition.Goal & Relation.EqualTo)) & Relation.OrderRelation;
            }

            if ((condition.Goal & Relation.SetRelation) != 0)
            {
                newGoal = ((~condition.Goal & Relation.IsProperSubSetOf) | (~condition.Goal & Relation.IsProperSuperSetTo) | (condition.Goal & Relation.IsEqualSetTo)) & Relation.SetRelation;
            }

            if ((condition.Goal & Relation.ExistanceRelation) != 0)
            {
                newGoal = ((~condition.Goal & Relation.ExistsIn) | (~condition.Goal & Relation.DoesNotExistIn)) & Relation.ExistanceRelation;
            }

            return new Condition(condition.FirstVariable, condition.SecondVariable, newGoal, condition.FirstVariableModifier, condition.SecondVariableModifier);
        }
        #endregion
    }
}