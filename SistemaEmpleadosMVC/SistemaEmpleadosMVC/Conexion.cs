﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;

namespace SistemaEmpleadosMVC
{
    class Conexion
    {
        SqlConnection connection = new SqlConnection("Data Source=LAPTOP-FO7R70NF;Initial Catalog=Empleados Diseño;Integrated Security=True");
        private SqlCommandBuilder cmb;
        public DataSet ds = new DataSet();
        public SqlDataAdapter da;
        public SqlCommand command;

        public void connect()
        {
            try
            {
                connection.Open();

            }
            catch
            {
                MessageBox.Show("Ocurrio un problema con la conexion");
            }
            finally
            {
                connection.Close();
            }
        }

        public bool Ejecutar(string sql)
        {
            connection.Open();
            command = new SqlCommand(sql, connection);
            int x = command.ExecuteNonQuery();
            connection.Close();
            if (x < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void infoEmpleados()
        {
            string sql = "select * from Empleado";
            string tabla = "Empleado";
            ds.Tables.Clear();
            da = new SqlDataAdapter(sql, connection);
            cmb = new SqlCommandBuilder(da);
            da.Fill(ds, tabla);
        }
        public void infoPuesto()
        {
            string sql = "select * from Puesto";
            string tabla = "Puesto";
            ds.Tables.Clear();
            da = new SqlDataAdapter(sql, connection);
            cmb = new SqlCommandBuilder(da);
            da.Fill(ds, tabla);

        }

        public bool update(string sql)
        {
            connection.Open();

            command = new SqlCommand(sql, connection);
            int x = command.ExecuteNonQuery();
            connection.Close();
            if (x < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ObtenerDatosFamiliares(int cedula)
        {
           /* */
            string sql = "SELECT * " +
                "        FROM (SELECT IFD.Identificacion, IFD.Nombre, IFD.Apellidos, " +
                        "IFD.Telefono, RF.[Parentesco con Empleado] " +
                        "FROM[Información de Familiares] IFD " +
                        "INNER JOIN[Relaciones Familiares] RF on IFD.Identificacion = RF.IdFamiliar " +
                        "INNER JOIN Empleado E on E.Identificación = RF.IdEmpleado " +
                        "WHERE E.Identificación =" + cedula.ToString()+") AS A";
            string tabla = "A";
            ds.Tables.Clear();
            da = new SqlDataAdapter(sql, connection);
            cmb = new SqlCommandBuilder(da);
            da.Fill(ds, tabla);
        }

        public void ObtenerHistorialVacaciones(int cedula)
        {
            string sql = "select * from [Historial de Vacaciones] where IdEmpleado = " + cedula.ToString();
            string tabla = "[Historial de Vacaciones]";
            ds.Tables.Clear();
            da = new SqlDataAdapter(sql, connection);
            cmb = new SqlCommandBuilder(da);
            da.Fill(ds, tabla);
        }

        public bool AgregarVacaciones(int cedula, DateTime fechaInicial, DateTime fechaFinal)
        {
            string sql = "insert into [Historial de Vacaciones](IdEmpleado, [Fecha Inicio], [Fecha Fin])" +
                         "values (" + cedula + ", '" + fechaInicial.ToString("yyyy-MM-dd") + "' , '" + fechaFinal.ToString("yyyy-MM-dd") + "')";
            return Ejecutar(sql);
        }

        public List<PuestoClass> ObtenerPuesto()
        {
            List<PuestoClass> listaPuesto = new List<PuestoClass>();
            connection.Open();
            SqlCommand comando = new SqlCommand("select * from Puesto", connection);
            SqlDataReader read = comando.ExecuteReader();
            while (read.Read())
            {
                PuestoClass qPuesto = new PuestoClass();
                int x = int.Parse(read["IdPuesto"].ToString());

                decimal y = decimal.Parse(read["Salario Pagado"].ToString());
                qPuesto.IdPuesto = x;
                qPuesto.Nombre_Puesto = read["Nombre Puesto"].ToString();
                qPuesto.Salario_Pagado = y;
                listaPuesto.Add(qPuesto);
            }
            connection.Close();
            return listaPuesto;
        }

        public PuestoClass ObtenerInformacionLaboral(int cedula)
        {
            connection.Open();
            string sql = "select p.*" +
                         "from[Información Laboral] il inner join " +
                         "Puesto p on il.IdPuesto = p.IdPuesto " +
                         "where IdEmpleado = " + cedula.ToString();
            SqlCommand comando = new SqlCommand(sql, connection);
            SqlDataReader read = comando.ExecuteReader();

            PuestoClass puesto = null;
            if (read.Read())
            {
                puesto = new PuestoClass
                {
                    IdPuesto = Int32.Parse(read["IdPuesto"].ToString()),
                    Nombre_Puesto = read["Nombre Puesto"].ToString(),
                    Salario_Pagado = decimal.Parse(read["Salario Pagado"].ToString())
                };
            }
            connection.Close();

            return puesto;
        }

        public string obtenerSalario(int IdPuesto)
        {
            string salario = "";
            connection.Open();
            SqlCommand comando = new SqlCommand("select * from Puesto where IdPuesto=" + IdPuesto, connection);
            SqlDataReader read = comando.ExecuteReader();
            while (read.Read())
            {
                salario = read["Salario Pagado"].ToString();
            }
            connection.Close();


            return salario;
        }

        public bool agregarEmpleado(int respCedula, string respNombre, string respApellido, string respCorreo, int respTelefono)
        {
            string sql = "insert into Empleado(Identificación, Nombre, Apellidos,  Correo, Teléfono) " +
                "values(" + respCedula + ",'" + respNombre + "','" + respApellido + "','" +
                respCorreo + "'," + respTelefono + ")";
            return Ejecutar(sql);
        }

        public bool agregarInfoLaboral(int respPuesto, int respCedula,  DateTime respFechaInicio)
        {
            string sql = "insert into [Información Laboral](IdPuesto, IdEmpleado, [Fecha Inicio]) " +
                "values(" + respPuesto + "," + respCedula+ ", '" + respFechaInicio + "')";
            return Ejecutar(sql);        
        }

        public bool agregarPermiso(int IdEmpleado, DateTime FechaInicio, DateTime FechaFinal, string Motivo)
        {
            string sql = "insert into [Permisos para Ausentarse] (IdEmpleado, [Fecha Inicial], [Fecha Final], Motivo)" +
                         "values(" + IdEmpleado + ",'" + FechaInicio + "','" + FechaFinal + "', '" + Motivo + "')";
            return Ejecutar(sql);
        }

        public bool editarEmpleado(int respCedula, string respNombre, string respApellido, int respTelefono, string respCorreo, int respPuesto)
        {
            string sql = "update Empleado set Identificación= " + respCedula + ", Nombre='" + respNombre + "', Apellidos='" +
                respApellido + "', Teléfono= " + respTelefono + ", Correo= '" + respCorreo + "'  where Identificación = " + respCedula;
            editarInfoLaboral(respPuesto, respCedula);
            connection.Open();
            command = new SqlCommand(sql, connection);
           
            int x = command.ExecuteNonQuery();
            connection.Close();
            if (x < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void editarInfoLaboral(int respPuesto, int respCedula)
        {
            string sql = "update [Información Laboral] set IdPuesto= " + respPuesto + "  where IdEmpleado = " + respCedula;
            connection.Open();
            command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
            connection.Close();

        }

        public void buscarEmpleado(int respCedula)
        {
            string sql = "select * from Empleado where Identificación like '%" + respCedula + "%'";
       
            string tabla = "Empleado";
            ds.Tables.Clear();
            da = new SqlDataAdapter(sql, connection);
            cmb = new SqlCommandBuilder(da);
            da.Fill(ds, tabla);
        }
        public Pagos ObtenerDatosPagos()
        {
            Pagos DatosPagos = new Pagos();
            connection.Open();
            SqlCommand comando = new SqlCommand("SELECT COUNT(E.Nombre) as 'Cantidad Empleados', SUM(P.[Salario Pagado]) as 'Salario Pagado' " +
                                                "FROM Empleado E " +
                                                "INNER JOIN[Información Laboral] IL on E.Identificación = IL.IdEmpleado " +
                                                "INNER JOIN Puesto P on IL.IdPuesto = p.IdPuesto", connection);
            SqlDataReader read = comando.ExecuteReader();
            while (read.Read())
            {


                int x = int.Parse(read["Cantidad Empleados"].ToString());
                //decimal y = decimal.Parse(read["Salario Pagado"].ToString());


                DatosPagos.Cantidad_de_Empleados = x;
              // DatosPagos.Salario_Pagado = y;


            }
            connection.Close();
            return DatosPagos;
        }

        public List<Empleado> ObtenerEmpleados()
        {

            List<Empleado> listaEmpleados = new List<Empleado>();
            connection.Open();
            SqlCommand comando = new SqlCommand("select * from Empleado", connection);
            SqlDataReader read = comando.ExecuteReader();
            while (read.Read())
            {
                Empleado qEmpleado = new Empleado();
                int cedula = int.Parse(read["Identificación"].ToString());
                int telefono = int.Parse(read["Teléfono"].ToString());

                qEmpleado.Identificación = cedula;
                qEmpleado.Teléfono = telefono;

                qEmpleado.Nombre = read["Nombre"].ToString();
                qEmpleado.Apellidos = read["Apellidos"].ToString();
                qEmpleado.Correo = read["Correo"].ToString();
                listaEmpleados.Add(qEmpleado);
            }
            connection.Close();
            return listaEmpleados;
        }

        public void RegistrarPagos(List<Empleado> ListaEmpleados)
        {
            foreach (Empleado x in ListaEmpleados)
            {
                string sql = "insert into [Historial Pagos]([Fecha pago], IdEmpleado) " +
                "values( GETDATE()," + x.Identificación + ")";
                Ejecutar(sql);
            }
        }
        public bool agregarPuesto(string nombre, decimal salario)
        {
            string sql = "insert into Puesto([Nombre Puesto], [Salario Pagado]) " +
              "values('"+nombre+"'," +salario+ ")";
            return Ejecutar(sql);
        }

        public bool editarPuesto(int IdPuesto, string respPuesto, decimal respSalario)
        {
            string sql = "update Puesto set [Nombre Puesto]='" + respPuesto + "', [Salario Pagado]= " + respSalario +
                " where IdPuesto = " + IdPuesto;

            
            connection.Open();
            command = new SqlCommand(sql, connection);

            int x = command.ExecuteNonQuery();
            connection.Close();
            if (x < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}