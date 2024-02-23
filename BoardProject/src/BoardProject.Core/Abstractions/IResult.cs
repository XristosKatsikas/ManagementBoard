using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardProject.Core.Abstractions
{
    public interface IResult<T> : IResultStatus
    {
        T Data { get; }
    }
}
