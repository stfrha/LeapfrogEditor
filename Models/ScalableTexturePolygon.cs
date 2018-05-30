﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   [Serializable]
   public class ScalableTexturePolygon : EditablePolygon
    {
      #region Declarations

      private double _textureWidth;
      private double _textureHeight;
      private double _textureOffsetX;
      private double _textureOffsetY;

      #endregion

      #region Constructors

      public ScalableTexturePolygon()
      {
      }

      #endregion

      #region Properties

      [XmlAttribute("textureMeterWidth")]
      public double TextureWidth
      {
         get { return _textureWidth; }
         set { _textureWidth = value; }
      }

      [XmlAttribute("textureMeterHeight")]
      public double TextureHeight
      {
         get { return _textureHeight; }
         set { _textureHeight = value; }
      }

      [XmlAttribute("textureOffsetMeterX")]
      public double TextureOffsetX
      {
         get { return _textureOffsetX; }
         set { _textureOffsetX = value; }
      }

      [XmlAttribute("textureOffsetMeterY")]
      public double TextureOffsetY
      {
         get { return _textureOffsetY; }
         set { _textureOffsetY = value; }
      }

      #endregion
   }
}
