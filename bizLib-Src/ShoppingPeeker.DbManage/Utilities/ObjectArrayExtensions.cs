using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingPeeker.DbManage.Utilities
{
    public static class ObjectArrayExtensions
    {
        public static T[] Remove<T>(this T[] objects, Func<T, bool> condition)
        {
            var hopeToDeleteObjs = objects.Where(condition);

            T[] newObjs = new T[objects.Length - hopeToDeleteObjs.Count()];

            int counter = 0;
            for (int i = 0; i < objects.Length; i++)
            {
                if (!hopeToDeleteObjs.Contains(objects[i]))
                {
                    newObjs[counter] = objects[i];
                    counter += 1;
                }
            }

            return newObjs;
        }
    }
}
