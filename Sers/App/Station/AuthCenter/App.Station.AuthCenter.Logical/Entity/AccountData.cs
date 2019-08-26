using Dapper.Contrib.Extensions;

namespace App.AuthCenter.Logical.Entity
{
    [Table("tb_account")]
    public class AccountData
    {
        [Key]
        public long userId { get; set; }
        public string account { get; set; }
        public string pwd { get; set; }

        public string name { get; set; }


    }

}
