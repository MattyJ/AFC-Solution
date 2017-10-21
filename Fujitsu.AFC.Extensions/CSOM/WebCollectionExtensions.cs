using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace Fujitsu.AFC.Extensions.CSOM
{
    [ExcludeFromCodeCoverage]
    public static class WebCollectionExtensions
    {
        public static Web GetWebByUrl(this WebCollection value, string name)
        {
            var context = value.Context;
            context.Load(value);
            context.ExecuteQuery();

            return value.First(ct => ct.Url == name);
        }

    }
}
