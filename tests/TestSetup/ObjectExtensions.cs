using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace TandemBooking.Tests.FullStackTests
{
    public static class ObjectExtensions
    {
        public static FormUrlEncodedContent ToContent(this object o)
        {
            var keyValuePairs = o.GetType().GetProperties()
                .SelectMany(p => p.PropertyType.IsArray
                        ? ((IEnumerable<object>) p.GetValue(o) ?? new object[] {}).Select(
                            x => new {Name = p.Name, Value = x, Type = p.PropertyType}
                        )
                        : new[] {new {Name = p.Name, Value = p.GetValue(o), Type = p.PropertyType}}
                )
                .Where(x => x?.Value != null)
                .Select(x => new KeyValuePair<string, string>(x.Name, x.Value.ToString()));

            return new FormUrlEncodedContent(keyValuePairs);
        }
    }
}