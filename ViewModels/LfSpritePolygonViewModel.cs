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
   class LfSpritePolygonViewModel : LfScalableTexturePolygonViewModel
   {
      #region Declarations

      #endregion

      #region Constructors

      public LfSpritePolygonViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, LfSpritePolygon modelObject) :
         base(mainVm, parent)
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public new LfSpritePolygon LocalModelObject
      {
         get { return (LfSpritePolygon)ModelObject; }
      }

      #endregion

   }
}
