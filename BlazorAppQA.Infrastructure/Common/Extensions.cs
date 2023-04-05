using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlazorAppQA.Infrastructure.Common
{
    public static class Extensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 5;
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            await Task.WhenAll(enumerable.Select(item => action(item)));
        }

        public static dynamic ToExpando(this object obj)
        {
            var result = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<ExpandoObject>(result);
        }
    }
}
