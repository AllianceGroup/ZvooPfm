﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Default.Models.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.4.1.0")]
    public static class TransactionHelper
    {

public static System.Web.WebPages.HelperResult Split(dynamic entryId)
        {
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {



#line 4 "..\..\Models\Helpers\TransactionHelper.cshtml"
         
        

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "        <div class=\"split-trans-container\">    \r\n        ");



WebViewPage.WriteLiteralTo(@__razor_helper_writer, "\r\n        <div class=\"account\">\r\n            <label class=\"lightbox-form-label\">a" +
"ccount i.d.</label>\r\n            <input type=\"text\" name=\"Entries[");



#line 10 "..\..\Models\Helpers\TransactionHelper.cshtml"
  WebViewPage.WriteTo(@__razor_helper_writer, entryId);

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "].AccountId\" class=\"account-form\" />\r\n        </div>\r\n\r\n        <div class=\"payee" +
"\">\r\n            <label class=\"lightbox-form-label\">payee</label>\r\n            <i" +
"nput type=\"text\" name=\"Entries[");



#line 15 "..\..\Models\Helpers\TransactionHelper.cshtml"
  WebViewPage.WriteTo(@__razor_helper_writer, entryId);

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "].Payee\" class=\"payee-form\" />\r\n        </div>\r\n\r\n        <div class=\"amount\">\r\n " +
"           <label class=\"lightbox-form-label\">amount</label>\r\n            <input" +
" type=\"text\" name=\"Entries[");



#line 20 "..\..\Models\Helpers\TransactionHelper.cshtml"
  WebViewPage.WriteTo(@__razor_helper_writer, entryId);

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "].Amount\" class=\"amount-form\" />\r\n        </div>\r\n        <div class=\"clear\"></di" +
"v>\r\n        <div class=\"memo\">\r\n            <label class=\"lightbox-form-label\">m" +
"emo</label>\r\n            <input type=\"text\" name=\"Entries[");



#line 25 "..\..\Models\Helpers\TransactionHelper.cshtml"
  WebViewPage.WriteTo(@__razor_helper_writer, entryId);

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "].Memo\" class=\"memo-form\" />\r\n        </div>\r\n                \r\n        <br/>\r\n  " +
"      <div class=\"clear\"></div>\r\n        <a id=\"remove\">remove</a>\r\n        </di" +
"v>\r\n");



#line 32 "..\..\Models\Helpers\TransactionHelper.cshtml"

    
#line default
#line hidden

});

    }


    }
}
#pragma warning restore 1591
