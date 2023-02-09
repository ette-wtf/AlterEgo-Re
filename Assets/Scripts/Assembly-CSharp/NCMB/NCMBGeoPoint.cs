using System;

namespace NCMB
{
	public struct NCMBGeoPoint
	{
		private double latitude;

		private double longitude;

		public double Latitude
		{
			get
			{
				return latitude;
			}
			set
			{
				if (value > 90.0 || value < -90.0)
				{
					throw new NCMBException(new ArgumentException("Latitude must be within the range -90.0~90.0"));
				}
				latitude = value;
			}
		}

		public double Longitude
		{
			get
			{
				return longitude;
			}
			set
			{
				if (value > 180.0 || value < -180.0)
				{
					throw new NCMBException(new ArgumentException("Longitude must be within the range -180~180"));
				}
				longitude = value;
			}
		}

		public NCMBGeoPoint(double latitude, double longitude)
		{
			this = default(NCMBGeoPoint);
			Latitude = latitude;
			Longitude = longitude;
		}
	}
}
