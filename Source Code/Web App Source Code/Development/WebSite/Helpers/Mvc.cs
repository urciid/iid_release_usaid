using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IID.WebSite.Helpers
{
    public static class Mvc
    {
        public static MvcHtmlString MailTo(this HtmlHelper html, string emailAddress, string displayText = null)
        {
            if (String.IsNullOrEmpty(displayText))
                displayText = emailAddress;

            var markup = String.Format("<a href=\"mailto:{0}\" title=\"{0}\">{1}</a>", emailAddress, HttpUtility.HtmlEncode(displayText));
            return new MvcHtmlString(markup);
        }

        public static MvcHtmlString LabelForForm<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return LabelForForm(html, expression, null);
        }

        public static MvcHtmlString LabelForForm<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelForForm(html, expression, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString LabelForForm<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
                return MvcHtmlString.Empty;

            TagBuilder label = new TagBuilder("label");
            label.MergeAttributes(htmlAttributes);
            label.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            label.SetInnerText(labelText + ":");
            return MvcHtmlString.Create(label.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Disable(this MvcHtmlString helper, bool disabled)
        {
            if (helper == null)
                throw new ArgumentNullException();

            if (disabled)
            {
                string html = helper.ToString();
                html = html.Insert(html.IndexOf('>'), " disabled=\"disabled\"");
                return MvcHtmlString.Create(html);
            }

            return helper;
        }

        public static string EscapeForJavaScript(string value)
        {
            if (value == null) return "";  // fix (maybe temp) to avoid null object reference

            string escape = "\\";

            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements.Add(escape, escape + escape);
            replacements.Add("'", escape + "'");
            replacements.Add("\r", escape + "r");
            replacements.Add("\n", escape + "n");

            string escaped = value;
            foreach (var replacement in replacements)
                escaped = escaped.Replace(replacement.Key, replacement.Value);
            return escaped;
        }

        public static string GetShortText(string value)
        {
            const int charLimit = 40;

            if (String.IsNullOrEmpty(value))
                return String.Empty;

            bool abbreviate = value.Length > charLimit;
            return (value.Substring(0, abbreviate ? charLimit : value.Length) + (abbreviate ? "..." : String.Empty));
        }
    }
}