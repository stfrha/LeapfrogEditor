using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeapfrogEditor
{
   class ShapePoint
   {
      #region Declarations

      // This is in local coordinates 
      private Point _p = new Point();
      private LfShapeViewModel _shape;

      #endregion

      #region Constructor

      public ShapePoint(Point p, LfShapeViewModel shape)
      {
         _p = p;
         _shape = shape;
      }

      #endregion

      #region Properties

      public Point P
      {
         get { return _p; }
         set { _p = value; }
      }

      public Point RotatedPoint
      {
         get
         {
            Point p2 = new Point();

            p2.X = _p.X * Math.Cos(_shape.Angle) 


            return p2;            
         }
      }

      public ShapePoint ShapeP
      {
         get
         {


         }
         set
         {

         }
      }

      public CoPoint CoP
      {
         get
         {
            return GetCoPoint();
         }
         set
         {
            SetCoPoint(value);
         }
      }

      public ScenePoint ScenP
      {
         get
         {
            CoPoint cop = GetCoPoint();

            return cop.ScenP;
         }
         set
         {
            CoPoint cop = new CoPoint(_shape.Parent);

            cop.ScenP = value;

            SetCoPoint(cop);
         }
      }

      #endregion

      #region Private methods

      private CoPoint GetCoPoint()
      {
         CoPoint cop = new CoPoint(_shape.Parent);
         // Transform Shape point to CoPoint using PosX, PosY and angle



         cop.P.Offset(_shape.PosX, _shape.PosY);
         return cop;
      }

      private void SetCoPoint(CoPoint sp)
      {
         _p = sp.P;

         // Transform CoPoint to ShapePoint using PosX, PosY and angle

         _p.Offset(-_shape.PosX, -_shape.PosY);
      }

      #endregion

   }
}
