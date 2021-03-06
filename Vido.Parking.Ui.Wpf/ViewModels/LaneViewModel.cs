﻿namespace Vido.Parking.Ui.Wpf.ViewModels
{
  using System;
  using System.Collections.ObjectModel;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Windows.Input;
  using System.Windows.Media.Imaging;
  using Vido.Capture.Events;
  using Vido.Parking.Controls;
  using Vido.Parking.Utilities;

  public class LaneViewModel : Utilities.NotificationObject
  {
    #region Data Members

    #region Text of Labels
    private string uniqueIdText = null;
    private string userDataText = null;
    private string backCamText = null;
    private string frontCamText = null;
    private string backImgText = null;
    private string frontImgText = null;
    #endregion

    private readonly Lane lane = null;

    private string message = null;
    private string laneCode = null;
    private string uniqueId = null;
    private string userData = null;
    private BitmapSource backImageSaved = null;
    private BitmapSource frontImageSaved = null;
    private BitmapSource frontImageCamera = null;
    private BitmapSource backImageCamera = null;

    private ICommand stopCommand = null;
    #endregion

    public string Message
    {
      get { return (message); }
      set
      {
        message = value;
        RaisePropertyChanged(() => Message);
      }
    }

    public string UniqueIdText
    {
      get { return (uniqueIdText); }
      set
      {
        uniqueIdText = value;
        RaisePropertyChanged(() => UniqueIdText);
      }
    }
    public string UserDataText
    {
      get { return (userDataText); }
      set
      {
        userDataText = value;
        RaisePropertyChanged(() => UserDataText);
      }
    }
    public string BackCamText
    {
      get { return (backCamText); }
      set
      {
        backCamText = value;
        RaisePropertyChanged(() => BackCamText);
      }
    }
    public string FrontCamText
    {
      get { return (frontCamText); }
      set
      {
        frontCamText = value;
        RaisePropertyChanged(() => FrontCamText);
      }
    }
    public string BackImgText
    {
      get { return (backImgText); }
      set
      {
        backImgText = value;
        RaisePropertyChanged(() => BackImgText);
      }
    }
    public string FrontImgText
    {
      get { return (frontImgText); }
      set
      {
        frontImgText = value;
        RaisePropertyChanged(() => FrontImgText);
      }
    }

    public string LaneCode
    {
      get { return (laneCode); }
      set
      {
        laneCode = value;
        RaisePropertyChanged(() => LaneCode);
      }
    }
    public string UniqueId
    {
      get { return (uniqueId); }
      set
      {
        uniqueId = value;
        RaisePropertyChanged(() => UniqueId);
      }
    }
    public string UserData
    {
      get { return (userData); }
      set
      {
        userData = value;
        RaisePropertyChanged(() => UserData);
      }
    }
    public BitmapSource BackImageCamera
    {
      get { return (backImageCamera); }
      set
      {
        backImageCamera = value;
        RaisePropertyChanged(() => BackImageCamera);
      }
    }
    public BitmapSource FrontImageCamera
    {
      get { return (frontImageCamera); }
      set
      {
        frontImageCamera = value;
        RaisePropertyChanged(() => FrontImageCamera);
      }
    }
    public BitmapSource BackImageSaved
    {
      get { return (backImageSaved); }
      set
      {
        backImageSaved = value;
        RaisePropertyChanged(() => BackImageSaved);
      }
    }
    public BitmapSource FrontImageSaved
    {
      get { return (frontImageSaved); }
      set
      {
        frontImageSaved = value;
        RaisePropertyChanged(() => FrontImageSaved);
      }
    }

    #region Public Construtors
    public LaneViewModel(Lane lane)
    {
      if (lane == null)
        throw new ArgumentNullException("lane");

      this.lane = lane;
      this.lane.Entry += lane_Entry;
      this.lane.SavedImages += lane_LastImages;
      this.lane.NewMessage += lane_NewMessage;

      this.LaneCode = lane.Code;

      if (this.lane.BackCamera != null)
      {
        this.lane.BackCamera.NewFrame += BackCamera_NewFrame;
      }

      if (this.lane.FrontCamera != null)
      {
        this.lane.FrontCamera.NewFrame += FrontCamera_NewFrame;
      }

      /// TODO: Địa phương hóa chuỗi thông báo.
      UniqueIdText = "Thẻ xe";
      UserDataText = "Biển số";
      
      BackCamText = "Camera phía sau";
      FrontCamText = "Camera phía trước";

      if (this.lane.Direction == Enums.Direction.In)
      {
        LaneCode += " \nVào";
        BackImgText = "Ảnh phía sau";
        FrontImgText = "Ảnh phía trước";
      }
      else
      {
        LaneCode += " \nRa";
        BackImgText = "Ảnh lúc VÀO phía sau";
        FrontImgText = "Ảnh lúc VÀO phía trước";
      }
    }
    #endregion


    #region Event Handlers
    private void lane_Entry(object sender, EventArgs e)
    {
      var args = e as Events.EntryEventArgs;

      this.UniqueId = Encode.GetDataString(args.DataIn.Data, args.DataIn.Printable);
      this.UserData = args.PlateNumber;

      /// Xóa thông báo hiện tại trên View.
      this.Message = string.Empty;
    }

    private void lane_LastImages(object sender, EventArgs e)
    {
      var args = e as Events.SavedImagesEventArgs;

      if (args.Back != null)
      {
        this.BackImageSaved = ConvertToBitmapSource(args.Back);
      }

      if (args.Front != null)
      {
        this.FrontImageSaved = ConvertToBitmapSource(args.Front);
      }
    }

    private void lane_NewMessage(object sender, EventArgs e)
    {
      this.Message = (e as Events.NewMessageEventArgs).Message;
    }

    private void BackCamera_NewFrame(object sender, EventArgs e)
    {
      var args = e as NewFrameEventArgs;
      this.BackImageCamera = ConvertToBitmapSource(args.Bitmap);
    }

    private void FrontCamera_NewFrame(object sender, EventArgs e)
    {
      var args = e as NewFrameEventArgs;
      this.FrontImageCamera = ConvertToBitmapSource(args.Bitmap);
    }
    #endregion

    #region Stop Commnand
    public ICommand StopCommand
    {
      get
      {
        return (stopCommand ?? (stopCommand = new Commands.RelayCommand<object>(StopExecute, StopCanExecute)));
      }
    }
    private bool StopCanExecute(object obj)
    {
      return (true);
    }
    private void StopExecute(object obj)
    {
      lane.StopEntry();
    }
    #endregion

    #region Utilities
    private static BitmapSource ConvertToBitmapSource(Image image)
    {
      if (image != null)
      {
        MemoryStream ms = new MemoryStream();
        image.Save(ms, ImageFormat.Jpeg);
        ms.Seek(0, SeekOrigin.Begin);

        BitmapImage bi = new BitmapImage();
        bi.BeginInit();
        bi.StreamSource = ms;
        bi.EndInit();
        bi.Freeze();
        return (bi);
      }
      return (null);
    }
    #endregion
  }
}
