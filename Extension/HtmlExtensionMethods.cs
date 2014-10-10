using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace InterWebs.Extension
{
    public static class HtmlExtensionMethods
    {
        public static IHtmlString JavaScriptSerialize(this HtmlHelper helper, object value, params JsonConverter[] converters)
        {
            return new HtmlString(JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                Converters = converters,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            }));
        }
    }
}