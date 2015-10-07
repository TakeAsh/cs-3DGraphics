using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaHelper {

    public static class IServiceProviderExtensionMethods {

        public static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class {

            return serviceProvider != null ?
                serviceProvider.GetService(typeof(T)) as T :
                null;
        }
    }
}
