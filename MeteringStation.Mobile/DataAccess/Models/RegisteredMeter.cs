using SQLite;

namespace MeteringStation.Mobile.DataAccess.Models
{
    [Table("RegisteredMeters")]
    public class RegisteredMeter
    {
        [PrimaryKey]
        [Column("deviceId")]
        [MaxLength(32)]
        public string DeviceId { get; set; }

        [Column("deviceIp")]
        [MaxLength(16)]
        public string DeviceIp { get; set; }
    }
}
