using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeapfrogEditor
{
   public class BorderTextureViewModel : MicroMvvm.ViewModelBase
   {
      #region Declarations

      private BorderTexture _modelObject;

      #endregion

      #region Constructors

      public BorderTextureViewModel(BorderTexture modelObject)
      {
         _modelObject = modelObject;
      }

      #endregion

      #region Properties

      public BorderTexture ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("ModelObject");
         }
      }

      public string Texture
      {
         get { return _modelObject.Texture; }
         set
         {
            _modelObject.Texture = value;
            OnPropertyChanged("Texture");
         }
      }

      public double HorisontalOffset
      {
         get { return _modelObject.HorisontalOffset; }
         set
         {
            _modelObject.HorisontalOffset = value;
            OnPropertyChanged("HorisontalOffset");
         }
      }

      public double TextureWidth
      {
         get { return _modelObject.TextureWidth; }
         set
         {
            _modelObject.TextureWidth = value;
            OnPropertyChanged("TextureWidth");
         }
      }

      public double TextureHeight
      {
         get { return _modelObject.TextureHeight; }
         set
         {
            _modelObject.TextureHeight = value;
            OnPropertyChanged("TextureHeight");
         }
      }

      #endregion
   }
}
