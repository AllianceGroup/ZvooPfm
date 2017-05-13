using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace mPower.OfferingsSystem
{
    public interface IFileLoader
    {
        IEnumerable<TextReader> LoadFor<T>();
    }
}