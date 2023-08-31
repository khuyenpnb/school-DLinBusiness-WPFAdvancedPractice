using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K204160664_PhamNguyenBichKhuyen_OnTapThucHanhWPF.Model
{
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string HireDate { get; set; }
        public bool Seniority5Years { get; set; }
        public string EmployeeType { get; set; }
        public float RevenueOrAllowance { get; set; }
        public int IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
