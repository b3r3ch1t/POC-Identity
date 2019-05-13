using System;
using System.Collections.Generic;

namespace POC.Domain.Core
{
    [Serializable]
    public class Result<T, TM>
    {
        public bool Success;
        public T Data;
        public IList<TM> Errors;

        public Result()
        {
            Errors = new List<TM>();
        }
    }
}
