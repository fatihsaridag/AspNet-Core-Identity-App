#pragma checksum "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7ecfbc906d0581c4513dae98b847a49936ee9766"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Admin_Claims), @"mvc.1.0.view", @"/Views/Admin/Claims.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 3 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\_ViewImports.cshtml"
using IdentityApp.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\_ViewImports.cshtml"
using IdentityApp.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7ecfbc906d0581c4513dae98b847a49936ee9766", @"/Views/Admin/Claims.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"95cade4efbbd7107a73bb230e2c4dd587df5ddab", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Admin_Claims : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<System.Security.Claims.Claim>>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml"
  
    ViewData["Title"] = "Claims";
    Layout = "~/Views/Admin/_AdminLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>Claims</h2>\r\n<hr />\r\n<table class=\"table table-bordered table-striped table-responsive\">\r\n    <tr>\r\n        <td>Kim</td>\r\n        <td>Dağıtıcı</td>\r\n        <td>Ad</td>\r\n        <td>Değer</td>\r\n    </tr>\r\n");
#nullable restore
#line 17 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml"
     foreach (System.Security.Claims.Claim  item in Model)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <tr>\r\n            <td> ");
#nullable restore
#line 20 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml"
            Write(item.Subject.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("  </td>\r\n            <td> ");
#nullable restore
#line 21 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml"
            Write(item.Issuer);

#line default
#line hidden
#nullable disable
            WriteLiteral("  </td>\r\n            <td> ");
#nullable restore
#line 22 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml"
            Write(item.Type);

#line default
#line hidden
#nullable disable
            WriteLiteral("  </td>\r\n            <td> ");
#nullable restore
#line 23 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml"
            Write(item.Value);

#line default
#line hidden
#nullable disable
            WriteLiteral("  </td>\r\n        </tr>\r\n");
#nullable restore
#line 25 "C:\Users\fatih\source\repos\AspNet-Core-Identity-App\IdentityApp\Views\Admin\Claims.cshtml"
                          
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</table>\r\n\r\n");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<System.Security.Claims.Claim>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
