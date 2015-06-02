﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deyo.Controls.Common
{
    public class NamedObjectsCollection<T> : IEnumerable, IEnumerable<T>
        where T : INamedObject
    {
        private readonly List<T> elements;

        public NamedObjectsCollection()
        {
            this.elements = new List<T>();
        }

        public void AddLast(T element)
        {
            this.EnsureUniqueName(element);
            this.elements.Add(element);
        }

        public void AddFirst(T element)
        {
            this.EnsureUniqueName(element);
            this.elements.Insert(0, element);
        }

        public void InsertBefore(T elementToInsert, string existingElementName)
        {
            this.InsertRelativeTo(elementToInsert, 0, existingElementName);
        }

        public void InsertAfter(T elementToInsert, string existingElementName)
        {
            this.InsertRelativeTo(elementToInsert, 1, existingElementName);
        }

        private void InsertRelativeTo(T elementToInsert, int relativeIndexDistance, string existingElementName)
        {
            int index = this.GetExistingElementIndex(existingElementName);
            this.EnsureUniqueName(elementToInsert);
            this.elements.Insert(index + relativeIndexDistance, elementToInsert);
        }

        private int GetExistingElementIndex(string alreadyAddedName)
        {
            for (int index = 0; index < this.elements.Count; index++)
            {
                if (this.elements[index].Name.Equals(alreadyAddedName))
                {
                    return index;
                }
            }

            throw new InvalidOperationException(string.Format("Cannot find element with the specified name: {0}", alreadyAddedName));
        }

        private void EnsureUniqueName(INamedObject newlyAddedElement)
        {
            if (this.elements.Any((existingElement) => existingElement.Name.Equals(newlyAddedElement.Name)))
            {
                throw new InvalidOperationException(string.Format("Cannot add element with the same name: {0}", newlyAddedElement.Name));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T element in this.elements)
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T element in this.elements)
            {
                yield return element;
            }
        }
    }
}
