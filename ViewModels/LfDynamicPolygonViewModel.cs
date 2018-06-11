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
   class LfDynamicPolygonViewModel : LfStaticPolygonViewModel
   {
      #region Declarations

      #endregion

      #region Constructors

      public LfDynamicPolygonViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, LfDynamicPolygon modelObject) :
         base(mainVm, parent, modelObject)
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public new LfDynamicPolygon LocalModelObject
      {
         get { return (LfDynamicPolygon)ModelObject; }
      }

      public double Density
      {
         get
         {
            if (LocalModelObject == null) return 0;

            return LocalModelObject.Density;
         }
         set
         {
            if (LocalModelObject == null) return;

            LocalModelObject.Density = value;
            OnPropertyChanged("Density");
         }
      }

      #endregion

   }
}
