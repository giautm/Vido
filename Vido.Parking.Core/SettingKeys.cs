﻿namespace Vido.Parking
{
  public static class SettingKeys
  {
    public const string RootImageDirectoryName = "RootImageDirectoryName";
    public const string Lanes = "Lanes";
    public const string Captures = "Captures";

    public const string InFormat = "InFormat";
    public const string OutFormat = "OutFormat";

    /// <summary>
    /// {0} - Directory separator char,
    /// YYYY - year,
    /// MM - month,
    /// DD - day
    /// </summary>
    public const string DailyDirectoryFormat = "DailyDirectoryNameFormat";

    /// <summary>
    /// {0} - Time,
    /// {1} - In/Out,
    /// {2} - Plate number,
    /// {3} - Uid data,
    /// </summary>
    public const string FrontImageNameFormat = "FrontImageNameFormat";

    /// <summary>
    /// {0} - Time,
    /// {1} - In/Out,
    /// {2} - Plate number,
    /// {3} - Uid data,
    /// </summary>
    public const string BackImageNameFormat = "BackImageNameFormat";
  }
}
