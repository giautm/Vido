﻿namespace Vido.Parking.Events
{
  using System;

  public delegate void DataInEventHandler(object s, DataInEventArgs e);

  public class DataInEventArgs : EventArgs
  {
    #region Public Properties
    /// <summary>
    /// Gets data.
    /// </summary>
    public byte[] Data { get; private set; }
    #endregion

    #region Constructors
    public DataInEventArgs(byte[] data)
    {
      this.Data = data;
    }
    #endregion
  }
}