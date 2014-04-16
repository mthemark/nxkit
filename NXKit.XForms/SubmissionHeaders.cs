﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NXKit.Util;

namespace NXKit.XForms
{

    public class SubmissionHeaders :
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {

        Dictionary<string, List<string>> items =
            new Dictionary<string, List<string>>();

        List<string> Get(string name)
        {
            return items.GetOrAdd(name, () => new List<string>());
        }

        public void Add(string name, string value)
        {
            Get(name).Add(value);
        }

        public void Add(string name, IEnumerable<string> values)
        {
            Get(name).AddRange(values);
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return items
                .Where(i => i.Value.Any())
                .Select(i => new KeyValuePair<string, IEnumerable<string>>(i.Key, i.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
