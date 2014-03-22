﻿namespace Vido.Parking.Utilities
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.IO;
  using System.Threading;
  using Vido.Capture.Interfaces;
  using Vido.Parking.Controls;
  using Vido.Parking.Interfaces;

  public static class Utilities
  {
    public static string EntryString
    {
      get { return ("0"); }
    }
    public static string ExitString
    {
      get { return ("1"); }
    }

    public static string SuffixPlateFileName
    {
      get { return ("P.jpg"); }
    }
    public static string SuffixFaceFileName
    {
      get { return ("F.jpg"); }
    }

    public static string RootImageDirectoryName
    {
      get { return (@"F:\Khanh\SkyDrive\Development\Github\Vido's Projects\Vido\RootImages"); }
    }

    
    public static string GetPlateNumber(Image image)
    {
      return (string.Empty);
    }
  }
}
