using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LeapfrogEditor
{
   class TextureRectValueConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value is ScalableTexturePolygonViewModel)
         {
            ScalableTexturePolygonViewModel stpvm = (ScalableTexturePolygonViewModel)value;

            Rect r = new Rect(stpvm.TextureOffsetX, stpvm.TextureOffsetY, stpvm.TextureWidth, stpvm.TextureHeight);

            return r;
         }


         return null;

      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }
}
