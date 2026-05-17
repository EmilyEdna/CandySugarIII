using CandySugar.Com.Data.Entity;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data
{
    public interface IService<T> where T : BasicEntity
    {
        Guid Insert(T input);

        void Remove(Guid Id);

        List<T> QueryAll();
    }
}
