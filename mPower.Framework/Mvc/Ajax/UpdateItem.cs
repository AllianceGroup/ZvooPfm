using System;

namespace mPower.Framework.Mvc.Ajax
{
    public enum UpdateStyle
    {
        Replace = 0,
        Append = 1,
        Prepend = 2,
        Insert = 3,
        After = 4,
        Before = 5,
        Value = 6
    }

    public class UpdateItem
    {
        /// <summary>
        /// JQuery string to find node on the page
        /// </summary>
        public String Query { get; set; }

        /// <summary>
        /// Html to update node content
        /// </summary>
        public String Html { get; set; }

        /// <summary>
        /// Style of updating
        /// </summary>
        public UpdateStyle UpdateStyle { get; set; }

        /// <summary>
        /// Initialization
        /// </summary>
        public UpdateItem(string nodeQuery, string html, UpdateStyle updateStyle)
        {
            Query = nodeQuery;
            Html = html;
            UpdateStyle = updateStyle;
        }

        /// <summary>
        /// Constructor overriding
        /// </summary>
        public UpdateItem(String nodeQuery, String html) : this(nodeQuery, html, UpdateStyle.Insert) { }
    }
}
