using CandySugar.Com.Data.Entity;
using CandySugar.Com.Data.Entity.AxgleEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data
{
    public interface IService<T> where T : BasicEntity
    {
        void Insert(T input);

        void Remove(Guid Id);

        List<T> QueryAll();
       
    }
}
