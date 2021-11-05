using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebAutomation.Core.Exceptions;

namespace WebAutomation.Core.Helpers
{
    public class ResultCollection<TProjection> : IEnumerable<TProjection>
    {
        private readonly List<TProjection> webElementProjections;

        public ResultCollection(List<TProjection> webElementProjections) => this.webElementProjections = webElementProjections;

        public TProjection Single
        {
            get
            {
                VerifySingleResultCount();
                return webElementProjections.Single();
            }
        }

        public void VerifySingleResultCount()
        {
            if (webElementProjections?.Count != 1)
                throw new WebAutomationException($"{webElementProjections?.Count} element is found, but 1 was expected!");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<TProjection> GetEnumerator() => webElementProjections.GetEnumerator();
    }
}