using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace mPower.Tests.Environment
{
    internal interface IMock<T> where T: class 
    {
        Mock<T> Create();
        T Object { get; }
    }
}
