﻿

#pragma checksum "C:\Users\DonCampos\Source\Workspaces\InfoApp\InfoAppWindows8\ICNG Phone\PivotPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "634B62890EC9B92A29E4B77D497687A5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ICNG_Phone
{
    partial class PivotPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 639 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.TextBox_TextChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 611 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonContent_Click_1;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 576 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonLetsGo_Click_1;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 62 "..\..\..\PivotPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.RefreshFeeds;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

