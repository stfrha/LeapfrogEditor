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
   public class GunProperties
   {
      #region Declarations

      private string _bodyName;
      private double _emitterOriginX;
      private double _emitterOriginY;
      private double _angle;
      private double _fireRate;
      private int _lifeTime;
      private double _impulseMagnitude;
      private bool _bouncy;

      #endregion

      #region Constructors

      public GunProperties()
      {
         _posX = 0;
         _posY = 0;
         _width = 100;
         _height = 100;
         _spawnInitial = 150;
         _intensity = 0.4;
         _lifeTime = 60000;
      }

      #endregion

      #region Properties

      [XmlAttribute("body")]
      public string BodyName
      {
         get { return _bodyName; }
         set { _bodyName = value; }
      }

      [XmlAttribute("emitterOriginX")]
      public double EmitterOriginX
      {
         get { return _emitterOriginX; }
         set { _emitterOriginX = value; }
      }

      [XmlAttribute("emitterOriginY")]
      public double EmitterOriginY
      {
         get { return _emitterOriginY; }
         set { _emitterOriginY = value; }
      }

      [XmlAttribute("angle")]
      public double Angle
      {
         get { return _angle; }
         set { _angle = value; }
      }

      [XmlAttribute("fireRate")]
      public double FireRate
      {
         get { return _fireRate; }
         set { _fireRate = value; }
      }

      [XmlAttribute("lifeTime")]
      public int LifeTime
      {
         get { return _lifeTime; }
         set { _lifeTime = value; }
      }

      [XmlAttribute("impulseMagnitude")]
      public double ImpulseMagnitude
      {
         get { return _impulseMagnitude; }
         set { _impulseMagnitude = value; }
      }

      [XmlAttribute("bouncy")]
      public bool Bouncy
      {
         get { return _bouncy; }
         set { _bouncy = value; }
      }

      #endregion

   }
}
