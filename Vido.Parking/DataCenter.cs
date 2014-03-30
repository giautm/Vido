﻿namespace Vido.Parking
{
  using System;
  using System.Data.Entity;
  using System.Linq;
  using Vido.Parking.Events;
  using Vido.Parking;
  using Vido.Parking.Utilities;

  public class DataCenter : IParking, ICardManagement, IDisposable
  {
    #region Data Members
    private VidoParkingEntities entities = null;
    #endregion

    #region Public Events
    /// <summary>
    /// Sự kiện kích hoạt khi nhận được thông báo mới từ Bãi.
    /// </summary>
    public event EventHandler NewMessage;
    #endregion

    #region Public Constructors
    public DataCenter()
    {
      this.entities = new VidoParkingEntities();
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Id của User hiện tại đã đăng nhập.
    /// </summary>
    public string CurrentUserId { get; set; }
    #endregion

    #region Private Methods
    private void RaiseNewMessage(string message)
    {
      if (NewMessage != null)
      {
        NewMessage(this, new NewMessageEventArgs(message));
      }
    }
    #endregion

    #region Implementation of IParking

    #region Public Properties
    /// <summary>
    /// Số lượng chỗ đỗ tối đa của Bãi.
    /// </summary>
    public int  MaximumSlots { get; set; }

    /// <summary>
    /// Trạng thái Bãi đầy.
    /// </summary>
    public bool IsFull { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Kiểm tra xem phương tiện có thể Vào bãi hay không.
    /// </summary>
    /// <param name="uniqueId">Dữ liệu Uid</param>
    /// <param name="plateNumber">Biển số phương tiện</param>
    /// <returns>true - Nếu phương tiện có thể Vào bãi, ngược lại: false</returns>
    public bool CanIn(string uniqueId, string plateNumber)
    {
      /// TODO: Trả về vị trí Phương tiện có thể Đỗ.

      try
      {
        var inRecords = from Records in entities.InOutRecord
                        where
                          Records.CardId == uniqueId &&
                          Records.OutEmployeeId == null &&
                          Records.OutLaneCode == null &&
                          Records.OutTime == null &&
                          Records.OutBackImg == null &&
                          Records.OutFrontImg == null
                        select Records;

        return (inRecords.Count() == 0);
      }
      catch
      {
        /// TODO: Địa phương hóa chuỗi thông báo.
        RaiseNewMessage("CanIn: Lỗi truy xuất dữ liệu.");
        return (false);
      }
    }

    /// <summary>
    /// Xử lý phương tiện Vào bãi.
    /// </summary>
    /// <param name="inArgs">Thông tin phương tiện vào bãi.</param>
    public void In(InOutArgs inArgs)
    {
      var parking = this as IParking;
      try
      {
        /// Thêm thông tin phương tiện VÀO.
        entities.InOutRecord.Add(new InOutRecord()
        {
          CardId = inArgs.Data,
          UserData = inArgs.PlateNumber,

          InEmployeeId = CurrentUserId,
          InLaneCode = inArgs.Lane,
          InTime = inArgs.Time.ToString(ISO8601DateTimeFormat),
          InBackImg = inArgs.BackImage,
          InFrontImg = inArgs.FrontImage
        });

        /// Cập nhật thông tin vào DB.
        entities.SaveChanges();

        /// Kiểm tra trạng thái Bãi.
        /// Đếm số phương tiện chưa RA Bãi
        /// và so sánh với số lượng vị trí tối đa.
        parking.IsFull = parking.MaximumSlots <= entities.InOutRecord.Count(r =>
          r.OutEmployeeId == null &&
          r.OutLaneCode == null &&
          r.OutTime == null &&
          r.OutBackImg == null &&
          r.OutFrontImg == null);
      }
      catch
      {
        /// TODO: Địa phương hóa chuỗi thông báo.
        RaiseNewMessage("In: Lỗi truy xuất dữ liệu.");
        throw;
      }
    }

    /// <summary>
    /// Kiểm tra xem phương tiện có thể Ra bãi hay không.
    /// </summary>
    /// <param name="uniqueId">Dữ liệu Uid</param>
    /// <param name="plateNumber">Biển số phương tiện</param>
    /// <returns>true - Nếu phương tiện có thể Ra bãi, ngược lại: false</returns>
    public bool CanOut(string uniqueId, string plateNumber, ref string inBackImage, ref string inFrontImage)
    {
      try
      {
        var inRecords = from Records in entities.InOutRecord
                        where
                          Records.UserData == plateNumber &&
                          Records.CardId == uniqueId &&
                          Records.OutEmployeeId == null &&
                          Records.OutLaneCode == null &&
                          Records.OutTime == null &&
                          Records.OutBackImg == null &&
                          Records.OutFrontImg == null
                        select Records;

        if (inRecords.Count() == 1)
        {
          inBackImage = inRecords.ToArray()[0].InBackImg;
          inFrontImage = inRecords.ToArray()[0].InFrontImg;

          return (true);
        }

        return (false);
      }
      catch
      {
        /// TODO: Địa phương hóa chuỗi thông báo.
        RaiseNewMessage("CanOut: Lỗi truy xuất dữ liệu.");
        return (false);
      }
    }

    /// <summary>
    /// Xử lý phương tiện Ra bãi.
    /// </summary>
    /// <param name="outArgs">Thông tin phương tiện ra.</param>
    public void Out(InOutArgs outArgs)
    {
      try
      {
        /// Lấy những bản ghi có CardId và PlateNumber khớp,
        /// và chưa có thông tin Ra.
        var inRecords = from Records in entities.InOutRecord
                        where
                          Records.CardId == outArgs.Data &&
                          Records.UserData == outArgs.PlateNumber &&
                          Records.OutEmployeeId == null &&
                          Records.OutLaneCode == null &&
                          Records.OutTime == null &&
                          Records.OutBackImg == null &&
                          Records.OutFrontImg == null
                        select Records;

        /// Chỉ có duy nhất 1 bảng ghi khớp,
        /// Nếu có nhiều hơn hoặc không có bản ghi nào 
        /// => Xuất thông báo lỗi Database.
        if (inRecords.Count() == 1)
        {
          var record = inRecords.ToArray()[0];

          /// Thêm thông tin phương tiện RA.
          record.OutEmployeeId = CurrentUserId;
          record.OutLaneCode = outArgs.Lane;
          record.OutTime = outArgs.Time.ToString(ISO8601DateTimeFormat);
          record.OutBackImg = outArgs.BackImage;
          record.OutFrontImg = outArgs.FrontImage;

          /// Cập nhật dữ liệu vào Database.
          entities.SaveChanges();

          /// Đặt lại trạng thái Bãi chưa đầy.
          (this as IParking).IsFull = false;
        }
        else
        {
          /// TODO: Địa phương hóa chuỗi thông báo.
          RaiseNewMessage("Out: Lỗi CSDL, không có hoặc có nhiều hơn một thông tin phương tiện vào.");
        }
      }
      catch
      {
        /// TODO: Địa phương hóa chuỗi thông báo.
        RaiseNewMessage("Out: Lỗi truy xuất dữ liệu.");
      }
    }
    #endregion

    #endregion

    #region Implementation of ICardManagement

    #region Public Methods
    /// <summary>
    /// Kiểm tra thẻ có tồn tại và đang được sử dụng hay không.
    /// </summary>
    /// <param name="cardId">Dữ liệu thẻ</param>
    /// <returns></returns>
    public bool IsExistAndUsing(string cardId)
    {
      var cards = from Cards in entities.Card
                  where
                    Cards.CardId == cardId
                  select Cards;

      return (cards.Count() == 1 && cards.ToArray()[0].State == 0);
    }
    #endregion

    #endregion

    #region Implementation of IDisposable

    #region Protected Methods
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // dispose managed resources
        entities.Dispose();
      }
    }
    #endregion

    #region Public Methods
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion

    #endregion

    #region Utilities
    /// <summary>
    /// Chuỗi định dạng thời gian theo chuẩn ISO-8601
    /// </summary>
    private static string ISO8601DateTimeFormat
    {
      get { return ("yyyy-MM-dd HH:mm:ss.fff"); }
    }
    #endregion
  }
}