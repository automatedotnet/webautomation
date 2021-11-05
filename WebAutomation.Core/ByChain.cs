using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace WebAutomation.Core
{
    public class ByChain : IEnumerable<By>
    {
        public bool AutoFixXPath { get; set; } = true;

        public List<By> Chain { get; }

        public ByChain(params By[] chain)
        {
            Chain = new List<By>();
            // this can be rewritten using SelectMany, BUT it does not guarantee order of items according to documentation(THOUGH looking decompiled code it does)
            foreach (var by in chain)
            {
                var wrapper = by as ByChainBy;
                if (wrapper != null)
                    Chain.AddRange(wrapper.Chain);
                else
                    Chain.Add(by);
            }
        }

        public IEnumerator<By> GetEnumerator() => Chain.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ByChain Concat(ByChain chain) => new(Chain.Concat(chain).ToArray());

        public override string ToString() => string.Join(", ", this.Select(b => $"'{b}'"));

        public static implicit operator By(ByChain chain)
        {
            if (chain.Chain?.Count == 1)
                return chain.Chain.Single();

            // return wrapper around chain if it contains more than 1 link
            return new ByChainBy(chain);
        }

        public static implicit operator ByChain(By by) => new(by);

        /// <summary>
        /// Wrapper class around ByChain
        /// </summary>
        private class ByChainBy : By
        {
            internal List<By> Chain { get; }

            public ByChainBy(ByChain chain)
            {
                Chain = chain.Chain;
                FindElementMethod = WithValidation(context => context.FindElement(Chain[0]));
                FindElementsMethod = WithValidation(context => context.FindElements(Chain[0]));
            }

            private Func<ISearchContext, T> WithValidation<T>(Func<ISearchContext, T> method)
            {
                return context =>
                {
                    if (Chain == null || Chain.Count != 1)
                        throw new InvalidCastException("By converted from ByChain with more or less than one Link can not be searched!");

                    return method(context);
                };
            }
        }
    }
}