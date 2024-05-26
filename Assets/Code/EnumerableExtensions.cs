using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infringed
{
    public static class EnumerableExtensions
    {
        public static T FindComponent<T>(this IEnumerable<GameObject> source) where T : Component
        {
            return source.Select(obj => obj.GetComponent<T>())
                         .FirstOrDefault(component => component != null);
        }

        public static T FindComponent<T>(this IEnumerable<Component> source) where T : Component
        {
            return source.Select(obj => obj.GetComponent<T>())
                         .FirstOrDefault(component => component != null);
        }
    }
}
