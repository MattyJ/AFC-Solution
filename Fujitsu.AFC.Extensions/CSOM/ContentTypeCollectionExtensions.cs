using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace Fujitsu.AFC.Extensions.CSOM
{
    [ExcludeFromCodeCoverage]
    public static class ContentTypeCollectionExtensions
    {
        //TODO: Only load what is needed
        public static List<ContentType> GetGroupContentTypesByGroupName(this ContentTypeCollection value, string name)
        {
            var context = value.Context;
            context.Load(value);
            context.ExecuteQuery();

            return value.Where(ct => ct.Group == name).ToList();
        }

        public static List<ContentTypeId> GetGroupContentTypeIdsByGroupName(this ContentTypeCollection value, string name)
        {
            var context = value.Context;
            context.Load(value);
            context.ExecuteQuery();

            return value.Where(ct => ct.Group == name).Select(ct => ct.Id).ToList();
        }

    }
}
