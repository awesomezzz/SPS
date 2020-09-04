using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

// 為公司產生工資單
/* 
    * Staff Class
    * Manager : Staff (經理class繼承Staff class)
    * Admin : Staff
    * FileReader(讀取數據)
    * PaySlip(工資單的class)
    * Program(主要的class)
*/

namespace HelloWorld
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<Staff> staffs = new List<Staff>();
            FileReader fileReader = new FileReader();
            int month = 0, year = 0;

            while (year == 0)
            {
                Console.WriteLine("Please enter the year:");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "Please try again");
                }
            }

            while (month == 0)
            {
                Console.WriteLine("Please enter the month:");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());

                    if (month < 1 || month > 12) 
                    {
                        Console.WriteLine("Month must be from 1 to 12. Please try again.");
                        month = 0; 
                    }
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.Message + "Please try again");
                }
            }

            staffs = fileReader.ReadFile(); 

            for (int i = 0; i < staffs.Count; i++)
            {
                try
                {
                    Console.WriteLine("Enter Hours worked for {0}", staffs[i].NameOfStaff);
                    staffs[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    staffs[i].CalculatePay();
                    Console.WriteLine(staffs[i]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(staffs);
            ps.GenerateSummary(staffs);

            Console.Read();



        }
    }

    /*員工類Staff Class*/
    class Staff
    {
        private float hourlyRate;
        private int hWorked;   

        public float TotalPay { get; protected set; } 
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }


        public int HoursWorked
        {
            get
            {
                return hWorked;
            }
            set
            {
                if (value > 0)
                    hWorked = value;
                else
                {
                    hWorked = 0;
                }
            }
        }

        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");

            BasicPay = hWorked * hourlyRate; 
            TotalPay = BasicPay;
        }

        public override string ToString() 
        {
            return "\nName of Staff: " + NameOfStaff +
                "\nHourly rate: " + hourlyRate +
                "\nHours worked: " + HoursWorked +
                "\nBasic pay: " + BasicPay +
                "\nTotal pay: " + TotalPay;
        }
    }

    /*經理類Manager Staff Class*/

    class Manager : Staff
    {
        private const float managerHourlyRate = 50;
                                                   

        public int Allowance { get; private set; }

        public Manager(string name) : base(name, managerHourlyRate)
        
        {
            // 這邊不做任何事，由父類環境設置
        }

        public override void CalculatePay() 
        {
            base.CalculatePay(); 
            Allowance = 1000;

            if (HoursWorked > 160)
                TotalPay = BasicPay + Allowance;
        }

        public override string ToString() 
        {
            return "\nName of Staff: " + NameOfStaff +
                "\nHourly rate: " + managerHourlyRate +
                "\nHours worked: " + HoursWorked +
                "\nBasic pay: " + BasicPay +
                "\nAllowance: " + Allowance +
                "\nTotal pay: " + TotalPay;
        }
    }

    /*管理員類Admin Staff Class*/

    class Admin : Staff
    {
        private const float overtimeRate = 15.5f; 
        private const float adminHourlyRate = 30f; 

        public float Overtime { get; private set; }

        public Admin(string name) : base(name, adminHourlyRate)
        {
            // 這邊不做任何事，由父類環境設置
        }

        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
            {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = Overtime + BasicPay;
            }
        }
        public override string ToString() 
        {
            return "\nName of Staff: " + NameOfStaff +
                "\nHourly rate: " + adminHourlyRate +
                "\nHours worked: " + HoursWorked +
                "\nBasic pay: " + BasicPay +
                "\nOvertime: " + Overtime +
                "\nTotal pay: " + TotalPay;
        }
    }

    /* FileReader(讀取數據)Class */
    class FileReader
    {
        public List<Staff> ReadFile() 
        {
            List<Staff> staffs = new List<Staff>(); 
            string path = "E:/VisualStudio/SalaryPaymentSystem/staff.txt"; 
            string[] result = new string[2];  
            string[] separator = { "," };     

            if (File.Exists(path)) 
            {
                using (StreamReader sr = new StreamReader(path)) 
                {
                    while (!sr.EndOfStream) 
                    {
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                       
                        if (result[1] == "Manager")
                        {
                            staffs.Add(new Manager(result[0]));
                        }
                        else if (result[1] == "Admin")
                        {
                            staffs.Add(new Admin(result[0]));
                        }
                    }
                    sr.Close();
                }

            }
            else
            {
                Console.WriteLine("Error: File does not exist");
            }

            return staffs;
        }

    }

    /* PaySlip(工資單)Class */
    class PaySlip
    {
        private int month;
        private int year;

        enum MonthsOfYear 
        {
            JAN = 1, FEB = 2, MAR = 3, APR = 4,
            MAY = 5, JUN = 6, JUL = 7, AUG = 8,
            SEP = 9, OCT = 10, NOV = 11, DEC = 12
        }

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> staffs)
        {
            string path;

            foreach (Staff s in staffs) 
            {
                path = s.NameOfStaff + ".txt"; 

                using (StreamWriter sw = new StreamWriter(path))
                {
                    /*
                        PAYSLIP FOR DEC 2010
                        =======================
                        Name of Staff: Yvonne
                        Hours Worked: 1231
 
                        Basic Pay: $61,550
                        Allowance: $1,000
 
                        =======================
                        Total Pay: $62,550
                        =======================
                    */
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("=======================");
                    sw.WriteLine("Name of Staff: {0}", s.NameOfStaff);
                    sw.WriteLine("Hours Worked: {0}", s.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("Basic Pay: {0:C}", s.BasicPay);

                    if (s.GetType() == typeof(Manager)) 
                        sw.WriteLine("Allowance: {0:C}", ((Manager)s).Allowance);
                    else if (s.GetType() == typeof(Admin))
                        sw.WriteLine("Overtime: {0:C}", ((Admin)s).Overtime);

                    sw.WriteLine("");
                    sw.WriteLine("=======================");
                    sw.WriteLine("Total Pay: {0:C}", s.TotalPay);
                    sw.WriteLine("=======================");

                    sw.Close();
                }
            }
        }

        public void GenerateSummary(List<Staff> staffs)
        {
            var result =
                from s in staffs 
                where s.HoursWorked < 10  
                orderby s.NameOfStaff ascending
                select new { s.NameOfStaff, s.HoursWorked };

            string path = "summary.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");

                foreach (var r in result) 
                {
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}",
                                 r.NameOfStaff, r.HoursWorked);
                }
                sw.Close();
            }
        }

        public override string ToString()
        {
            return "month = " + month + " year = " + year;
        }
    }

}























