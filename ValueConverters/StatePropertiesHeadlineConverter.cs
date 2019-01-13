using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LeapfrogEditor
{
   class StatePropertiesHeadlineConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value is ChildObjectStatePropertiesViewModel) 
         {
            ChildObjectStatePropertiesViewModel vm = (ChildObjectStatePropertiesViewModel)value;
            ChildObjectViewModel chvm = vm.TreeParent as ChildObjectViewModel;

            string s = chvm.Name + " State Properties";

            return s;
         }

         return null;

      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return DependencyProperty.UnsetValue;
      }
   }

}
