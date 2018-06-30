﻿#pragma checksum "..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "29E4AEDE4DC520A820F772F209939DB0CA8EBF0C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LeapfrogEditor;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using ZoomAndPan;


namespace LeapfrogEditor {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LeapfrogEditor.MainViewModel myMainViewModel;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer scroller;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ZoomAndPan.ZoomAndPanControl zoomAndPanControl;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid theGrid;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas content;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas dragZoomCanvas;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border dragZoomBorder;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LeapfrogEditor;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\MainWindow.xaml"
            ((LeapfrogEditor.MainWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.myMainViewModel = ((LeapfrogEditor.MainViewModel)(target));
            return;
            case 3:
            this.scroller = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 4:
            this.zoomAndPanControl = ((ZoomAndPan.ZoomAndPanControl)(target));
            
            #line 74 "..\..\MainWindow.xaml"
            this.zoomAndPanControl.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.zoomAndPanControl_MouseDown);
            
            #line default
            #line hidden
            
            #line 75 "..\..\MainWindow.xaml"
            this.zoomAndPanControl.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.zoomAndPanControl_MouseUp);
            
            #line default
            #line hidden
            
            #line 76 "..\..\MainWindow.xaml"
            this.zoomAndPanControl.MouseMove += new System.Windows.Input.MouseEventHandler(this.zoomAndPanControl_MouseMove);
            
            #line default
            #line hidden
            
            #line 77 "..\..\MainWindow.xaml"
            this.zoomAndPanControl.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.zoomAndPanControl_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 5:
            this.theGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.content = ((System.Windows.Controls.Canvas)(target));
            
            #line 94 "..\..\MainWindow.xaml"
            this.content.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Canvas_MouseUp);
            
            #line default
            #line hidden
            
            #line 95 "..\..\MainWindow.xaml"
            this.content.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Canvas_MouseDown);
            
            #line default
            #line hidden
            
            #line 96 "..\..\MainWindow.xaml"
            this.content.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.MouseWheel_Rotate);
            
            #line default
            #line hidden
            return;
            case 7:
            this.dragZoomCanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.dragZoomBorder = ((System.Windows.Controls.Border)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

