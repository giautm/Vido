﻿namespace Vido.Parking.Interfaces
{
  using System;
  using System.Collections.Generic;
  using Vido.Parking.Events;

  public interface IUidDeviceList
  {
    event DevicesChangedEventHandler DevicesChanged;
    IList<IUidDevice> GetDevicesList();

    IUidDevice Register(string deviceName);
    void Unregister(IUidDevice device);
  }
}