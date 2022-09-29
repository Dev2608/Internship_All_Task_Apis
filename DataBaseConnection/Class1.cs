using System;
using System.Data.SqlClient;
using System.Data;

namespace DataBaseConnection
{
    public class connect
    {
        SqlConnection con;

        public void connection()
        {
            con = new SqlConnection(
                "Data Source = DESKTOP-EFHH4AI\\SQLEXPRESS; " +
                "Initial Catalog = FinalDB; " +
                "Integrated Security = true;");
        }

        public string generateUniqueNumber()
        {
            string lastFourDigit, s;
            int LASTfourDIGIT;

            connection();

            con.Open();

            SqlCommand cmd = new SqlCommand("spGetUniqueNumber", con);
            var num = cmd.ExecuteScalar();

            con.Close();

            if (num == null)
            {
                lastFourDigit = "0001";
                return lastFourDigit;
            }
            else
            {
                s = num.ToString();
                LASTfourDIGIT = Convert.ToInt32(s);
                LASTfourDIGIT++;

                s = LASTfourDIGIT.ToString();
                s = s.PadLeft(4, '0');

                return s;
            }
        }

        //---------------------------------------------------------------------------------------

        public DataSet spSelectData(String P)
        {
            connection();

            con.Open();

            SqlDataAdapter a = new SqlDataAdapter(P, con);
            DataSet ds = new DataSet();
            a.Fill(ds);

            return (ds);

            con.Close();
        }

        public DataSet selectEmployee(String p)
        {
            con.Open();

            SqlDataAdapter a = new SqlDataAdapter(p, con);
            DataSet ds = new DataSet();
            a.Fill(ds);

            return (ds);

            con.Close();
        }

        //-------------------------------------------------------------------------------------------------------------
    
        public DataSet selectStudent(String p)
        {
            con.Open();

            SqlDataAdapter a = new SqlDataAdapter(p, con);
            DataSet ds = new DataSet();
            a.Fill(ds);

            return (ds);

            con.Close();
        }

        //----------------------------------------------------------------------------------------------------------

        public DataSet select_Register(String p)
        {
            con.Open();

            SqlDataAdapter a = new SqlDataAdapter(p, con);
            DataSet ds = new DataSet();
            a.Fill(ds);

            return (ds);

            con.Close();
        }

        //---------------------------------------------------------------------------------------------------------

        public DataSet selectStudentOAuth(String p)
        {
            con.Open();

            SqlDataAdapter a = new SqlDataAdapter(p, con);
            DataSet ds = new DataSet();
            a.Fill(ds);

            return (ds);

            con.Close();
        }

    }
}