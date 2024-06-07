using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.IOC
{

    public abstract class EnumerableFactoryBean : IEnumerable<BeanDefinition>
    {
        public abstract IEnumerator<BeanDefinition> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}