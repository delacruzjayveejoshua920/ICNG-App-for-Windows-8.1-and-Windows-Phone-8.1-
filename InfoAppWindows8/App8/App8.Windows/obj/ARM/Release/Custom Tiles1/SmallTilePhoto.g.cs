﻿

#pragma checksum "C:\Users\DonCampos\Source\Workspaces\InfoApp\InfoAppWindows8\App8\App8.Windows\Custom Tiles1\SmallTilePhoto.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BB6E55164DE97F0DB3DAFAD191737679"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomLiveTiles
{
    partial class SmallTilePhoto : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 12 "..\..\..\Custom Tiles1\SmallTilePhoto.xaml"
                ((global::Windows.UI.Xaml.Media.Animation.Timeline)(target)).Completed += this.liveTileAnimTop1_Part1_Completed;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 21 "..\..\..\Custom Tiles1\SmallTilePhoto.xaml"
                ((global::Windows.UI.Xaml.Media.Animation.Timeline)(target)).Completed += this.liveTileAnimTop1_Part2_Completed;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 30 "..\..\..\Custom Tiles1\SmallTilePhoto.xaml"
                ((global::Windows.UI.Xaml.Media.Animation.Timeline)(target)).Completed += this.liveTileAnimTop2_Part1_Completed;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 39 "..\..\..\Custom Tiles1\SmallTilePhoto.xaml"
                ((global::Windows.UI.Xaml.Media.Animation.Timeline)(target)).Completed += this.liveTileAnimTop2_Part2_Completed;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


