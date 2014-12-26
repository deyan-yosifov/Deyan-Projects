using System;
using System.Collections.Generic;
using Vrml.Core;

namespace Vrml.Model.Animations
{
    public abstract class Interpolator<T> : IVrmlElement
        where T : IVrmlSimpleType
    {
        private readonly Collection<KeyValuePair<double, T>> keyValues;

        public Interpolator()
        {
            this.keyValues = new Collection<KeyValuePair<double, T>>();
        }

        public abstract string Name { get; }

        public string DefinitionName { get; set; }

        public string Comment { get; set; }

        public Collection<KeyValuePair<double, T>> KeyValues
        {
            get
            {
                return this.keyValues;
            }
        }

        public IEnumerable<double> Keys
        {
            get
            {
                foreach (KeyValuePair<double, T> keyValue in this.KeyValues)
                {
                    yield return keyValue.Key;
                }
            }
        }

        public IEnumerable<T> Values
        {
            get
            {
                foreach (KeyValuePair<double, T> keyValue in this.KeyValues)
                {
                    yield return keyValue.Value;
                }
            }
        }
    }
}
