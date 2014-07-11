﻿using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace NXKit.XForms.Xml
{

    public static class XElementExtensions
    {

        /// <summary>
        /// Gets the value of a given variable from a given <see cref="XElement"/>.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetVariableValue(this XElement self, string name)
        {
            Contract.Requires<ArgumentNullException>(self != null);
            Contract.Requires<ArgumentNullException>(name != null);

            // search referencable objects for id, or obtain from cache
            return GetVariables(self)
                .Where(i => i.Name == name)
                .Select(i => i.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns an enumeration of canidate 'var' elements implementations.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        static IEnumerable<Var> GetVariables(this XElement self)
        {
            yield break;
        }

    }

}
