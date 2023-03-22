using System.Collections.Generic;

namespace MVP.Model
{
    /// <summary>
    /// Represents a condition for a state-change within a state machine
    /// </summary>
    public struct Condition
    {
        /// <summary>
        /// Retrieves the default condition with the name "DEFAULT".
        /// It is globally unique.
        /// </summary>
        /// <returns>the default condition.</returns>
        public static Condition GetDefaultInstance() => GetConditionInstance(DEFAULTNAME);

        /// <summary>
        /// Retrieves the globally unique condition with the supplied name (key).
        /// </summary>
        /// <param name="name">The name of the condition to return.</param>
        /// <returns>the condition with the supplied name.</returns>
        #region GetConditionInstance
        public static Condition GetConditionInstance(string name)
        {
            if (existingConditions == null)
            {
                existingConditions = new Dictionary<string, Condition>();
                existingConditions.Add(DEFAULTNAME, new Condition(DEFAULTNAME));
            }
            if (!existingConditions.ContainsKey(name))
                existingConditions.Add(name, new Condition(name));
            return existingConditions[name];
        }
        #endregion

        /// <summary>
        /// Checks for equality between the two Conditions.
        /// They are equal iff their names are equal.
        /// </summary>
        /// <param name="a">The left side Condition to compare.</param>
        /// <param name="b">The right side Condition to compare.</param>
        /// <returns>true, iff both Condition are equal. False otherwise.</returns>
        public static bool operator ==(Condition a, Condition b) => a.Equals(b);

        /// <summary>
        /// The inverse of the == operation.
        /// Checks for inequality between the two Conditions.
        /// They are equal iff their names are equal.
        /// </summary>
        /// <param name="a">The left side Condition to compare.</param>
        /// <param name="b">The right side Condition to compare.</param>
        /// <returns>false, iff both Condition are equal. True otherwise.</returns>
        public static bool operator !=(Condition a, Condition b) => !a.Equals(b);

        /// <summary>
        /// Returns the name (key) of this condition.
        /// It is globally unique among all conditions.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Converts this struct into it's string-representation.
        /// It is identical to 'Name' property.
        /// </summary>
        /// <returns>the name of the Condition</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Determines whether of not this Condition equals the supplied object.
        /// The supplied object needs to be a Condition as well and neds to have the same 'Name' for it to be equal.
        /// </summary>
        /// <param name="obj">The object to compare this Condition to.</param>
        /// <returns>whether the supplied object is equal to this Condition.</returns>
        #region Equals
        public override bool Equals(object obj)
        {
            if (!typeof(Condition).IsAssignableFrom(obj?.GetType()))
                return false;
            var other = (Condition)obj;
            return other.Name == this.Name;
        }
        #endregion

        /// <summary>
        /// Computes the hash-code for this Condition object.
        /// The hash is identical to the hash of it's string-representation.
        /// </summary>
        /// <returns>the hash-code for this Condition struct.</returns>
        public override int GetHashCode() => this.ToString().GetHashCode();

        #region Internal
        private static readonly string DEFAULTNAME = "DEFAULT";

        private static Dictionary<string, Condition> existingConditions;

        private Condition(string name) => Name = name;
        #endregion
    }
}