﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TDL.Client.Utils
{
    public struct Maybe<T>
    {
        private readonly IEnumerable<T> values;

        public static Maybe<T> Some(T value)
        {
            if (value == null)
            {
                throw new InvalidOperationException();
            }

            return new Maybe<T>(new[] {value});
        }

        public static Maybe<T> None => new(new T[0]);

        public bool HasValue => values != null && values.Any();

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Maybe does not have a value");
                }

                return values.Single();
            }
        }

        private Maybe(IEnumerable<T> values)
        {
            this.values = values;
        }
    }
}
