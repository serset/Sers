using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MsTest.Model
{ 
    [Table("tb_corporation")]
    public class User 
    {
        [Key]
        public int OrgId { get; set; }
        public string OperatorName { get; set; }
        public DateTime? AddTime { get; set; }
    }
     
}
