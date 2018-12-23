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

      #endregion

      #region Constructors

      public LfStaticPolygonViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, LfStaticPolygon modelObject) :
         base(mainVm, parent)
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public new LfStaticPolygon LocalModelObject
      {
         get { return (LfStaticPolygon)ModelObject; }
      }

      #endregion

   }
}
