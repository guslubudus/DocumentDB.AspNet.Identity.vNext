using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DocumentDB.AspNet.Identity {
    internal static class Extensions {
        internal static PropertyInfo[] GetPublicProperties( this Type type, params Type[] exclude ) {
            var typeProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).AsQueryable();
            foreach ( var inhertedTypes in exclude )
                typeProps = typeProps.Where( tp => !inhertedTypes.GetProperties( BindingFlags.Public | BindingFlags.Instance ).Select( ip => ip.Name ).Contains( tp.Name ) );
            return typeProps.ToArray( );
        }
        internal static IList<T> ToIList<T>( this IEnumerable<T> enumerable ) {
            return enumerable.ToList( );
        }

        public static IList<T> Remove<T>( this IList<T> collection, Func<T, bool> predicate ) {
            var item = collection.FirstOrDefault(predicate);
            if ( item != null )
                collection.Remove( item );
            return collection;
        }
    }
}
