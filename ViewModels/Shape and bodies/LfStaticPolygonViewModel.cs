using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LeapfrogEditor
{
   class LfStaticPolygonViewModel : LfScalableTexturePolygonViewModel
   {
      #region Declarations

      private BorderTextureViewModel _groundBorder = null;
      private BorderTextureViewModel _ceilingBorder = null;
      private BorderTextureViewModel _leftWallBorder = null;
      private BorderTextureViewModel _rightWallBorder = null;

      #endregion

      #region Constructors

      public LfStaticPolygonViewModel(
         TreeViewViewModel treeParent, 
         CompoundObjectViewModel parentVm, 
         MainViewModel mainVm, 
         LfStaticPolygon modelObject) :
         base(treeParent, parentVm, mainVm)
      {
         ModelObject = modelObject;

         GroundBorder = new BorderTextureViewModel(modelObject.PolygonBorder.GroundBorder);
         CeilingBorder = new BorderTextureViewModel(modelObject.PolygonBorder.CeilingBorder);
         LeftWallBorder = new BorderTextureViewModel(modelObject.PolygonBorder.LeftWallBorder);
         RightWallBorder = new BorderTextureViewModel(modelObject.PolygonBorder.RightWallBorder);
      }

      #endregion

      #region Properties

      public new LfStaticPolygon LocalModelObject
      {
         get { return (LfStaticPolygon)ModelObject; }
      }

      public double LeftGroundAngle
      {
         get { return ((LfStaticPolygon)ModelObject).PolygonBorder.LeftGroundAngle; }
         set
         {
            ((LfStaticPolygon)ModelObject).PolygonBorder.LeftGroundAngle = value;
            OnPropertyChanged("LeftGroundAngle");
         }
      }

      public double RightGroundAngle
      {
         get { return ((LfStaticPolygon)ModelObject).PolygonBorder.RightGroundAngle; }
         set
         {
            ((LfStaticPolygon)ModelObject).PolygonBorder.RightGroundAngle = value;
            OnPropertyChanged("RightGroundAngle");
         }
      }

      public double LeftCeilingAngle
      {
         get { return ((LfStaticPolygon)ModelObject).PolygonBorder.LeftCeilingAngle; }
         set
         {
            ((LfStaticPolygon)ModelObject).PolygonBorder.LeftCeilingAngle = value;
            OnPropertyChanged("LeftCeilingAngle");
         }
      }

      public double RightCeilingAngle
      {
         get { return ((LfStaticPolygon)ModelObject).PolygonBorder.RightCeilingAngle; }
         set
         {
            ((LfStaticPolygon)ModelObject).PolygonBorder.RightCeilingAngle = value;
            OnPropertyChanged("RightCeilingAngle");
         }
      }

      public BorderTextureViewModel GroundBorder
      {
         get { return _groundBorder; }
         set
         {
            _groundBorder = value;
            OnPropertyChanged("GroundBorder");
         }
      }

      public BorderTextureViewModel CeilingBorder
      {
         get { return _ceilingBorder; }
         set
         {
            _ceilingBorder = value;
            OnPropertyChanged("CeilingBorder");
         }
      }

      public BorderTextureViewModel LeftWallBorder
      {
         get { return _leftWallBorder; }
         set
         {
            _leftWallBorder = value;
            OnPropertyChanged("LeftWallBorder");
         }
      }

      public BorderTextureViewModel RightWallBorder
      {
         get { return _rightWallBorder; }
         set
         {
            _rightWallBorder = value;
            OnPropertyChanged("RightWallBorder");
         }
      }

      #endregion

   }
}
