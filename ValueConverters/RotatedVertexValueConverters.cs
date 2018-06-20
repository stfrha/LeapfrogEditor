﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LeapfrogEditor
{
   class PreviousMultiRotatedVertexValueConverter : IMultiValueConverter
   {
      public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (values.Count() == 2)
         {
            if ((values[0] is double) && (values[1] is LfDragablePointViewModel))
            {
               LfDragablePointViewModel origVertex = (LfDragablePointViewModel)values[1];
               LfPolygonViewModel polygon = origVertex.Parent;

               int i = polygon.PointVms.IndexOf(origVertex);

               if (i == -1)
               {
                  return null;
               }

               LfDragablePointViewModel vertex;

               if (i > 0)
               {
                  vertex = polygon.PointVms[i - 1];
               }
               else
               {
                  vertex = polygon.PointVms[polygon.PointVms.Count() - 1];
               }

               Point p = new Point(vertex.PosX, vertex.PosY);
               Point rp = polygon.RotatedPointFromLocal(p);

               if (parameter as string == "x")
               {
                  return rp.X;
               }
               else
               {
                  return rp.Y;
               }
            }
         }

         return null;
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }

   class MultiRotatedVertexValueConverter : IMultiValueConverter
   {
      public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (values.Count() == 2)
         {
            if ((values[0] is double) && (values[1] is LfDragablePointViewModel))
            {
               double pos = (double)values[0];
               LfDragablePointViewModel vertex = (LfDragablePointViewModel)values[1];
               LfShapeViewModel shape = vertex.Parent;

               Point p;

               if (parameter as string == "x")
               {
                  p = new Point(pos, vertex.PosY);
                  Point rp = shape.RotatedPointFromLocal(p);
                  return rp.X;
               }
               else
               {
                  p = new Point(vertex.PosX, pos);
                  Point rp = shape.RotatedPointFromLocal(p);
                  return rp.Y;
               }
            }
         }

         return null;
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }

   class RotatedVertexXValueConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (parameter as string == "debug")
         {
            int a = 10;
         }

         if (parameter as string == "debug2")
         {
            int a = 10;
         }

         if (value is LfDragablePointViewModel)
         {
            LfDragablePointViewModel vertex = (LfDragablePointViewModel)value;
            LfShapeViewModel shape = vertex.Parent;

            Point p = new Point(vertex.PosX, vertex.PosY);
            Point rp = shape.RotatedPointFromLocal(p);
            
            return rp.X;
         }

         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }

   class RotatedVertexYValueConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value is LfDragablePointViewModel)
         {
            LfDragablePointViewModel vertex = (LfDragablePointViewModel)value;
            LfShapeViewModel shape = vertex.Parent;

            Point p = new Point(vertex.PosX, vertex.PosY);
            Point rp = shape.RotatedPointFromLocal(p);

            return rp.Y;
         }

         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}
