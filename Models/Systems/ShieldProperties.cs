using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   [Serializable]
   public class ShieldProperties
   {
      #region Declarations

      private string _bodyName;
      private double _bodyAnchorX;
      private double _bodyAnchorY;
      private double _radius;
      private int _zLevel;

      #endregion

      #region Constructors

      public ShieldProperties()
      {
         _bodyName = "notDefined";
         _bodyAnchorX = 0;
         _bodyAnchorY = 0;
         _radius = 100;
         _zLevel = 147;
      }

      #endregion

      #region Properties

      [XmlAttribute("posX")]
      public double PosX
      {
         get { return _posX; }
         set { _posX = value; }
      }

      [XmlAttribute("posY")]
      public double PosY
      {
         get { return _posY; }
         set { _posY = value; }
      }

      [XmlAttribute("width")]
      public double Width
      {
         get { return _width; }
         set { _width = value; }
      }

      [XmlAttribute("height")]
      public double Height
      {
         get { return _height; }
         set { _height = value; }
      }

      [XmlAttribute("spawnInitial")]
      public int SpawnInitial
      {
         get { return _spawnInitial; }
         set { _spawnInitial = value; }
      }
      
      [XmlAttribute("intensity")]
      public double Intensity
      {
         get { return _intensity; }
         set { _intensity = value; }
      }

      [XmlAttribute("lifeTime")]
      public int LifeTime
      {
         get { return _lifeTime; }
         set { _lifeTime = value; }
      }

      #endregion

   }
}
