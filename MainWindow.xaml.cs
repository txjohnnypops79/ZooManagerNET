using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZooManagerNET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        string showAllAnimals = "Select * from animal";
        string showAllzoos = "select * from zoo";
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            SQLQueries(showAllzoos, "Location",ListZoos);
            SQLQueries(showAllAnimals, "Name",ListAddAnimalToZoo);
            
        }
        private void SQLQueries(string query, string name, ListBox list)
        {
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);
                    list.DisplayMemberPath = name;
                    list.SelectedValuePath = "Id";
                    list.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }

        }

        private void ShowAnimals()
        {
            try
            {
                string query = "select * from Animal a " +
                    "inner join zooAnimal za on za.animalId = a.Id " +
                    "where za.ZooId = @ZooId ";

                SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
                var sqlDa = new SqlDataAdapter(sqlCommand);

                using (sqlDa)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);


                    DataTable animalTable = new DataTable();
                    sqlDa.Fill(animalTable);
                    ListAnimal.DisplayMemberPath = "Name";
                    ListAnimal.SelectedValuePath = "Id";
                    ListAnimal.ItemsSource = animalTable.DefaultView;

                }
            }
            catch (Exception e)
            {

               MessageBox.Show(e.ToString());
            }
            finally
            {
               
            }
        }
       
     
        private void ListZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListZoos.SelectedItem != null)
            {
                ShowAnimals();

            }
            else
            {
                ListAnimal.ClearValue(ItemsControl.ItemsSourceProperty);
               
            }
        }

      

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from zoo where id = @zooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@zooId", ListZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
              
                SQLQueries(showAllzoos, "Location", ListZoos);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addAnimal_Click(object sender, RoutedEventArgs e)
        {
            string query = "Insert into Animal values(@animal)";
            SqlCommand sqlcmd = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
           
            sqlcmd.Parameters.AddWithValue("@animal", myText.Text);
          
            sqlcmd.ExecuteReader();
            sqlConnection.Close();
            SQLQueries(showAllAnimals, "Name", ListAddAnimalToZoo);
        }

        private void addZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "Insert into Zoo values(@location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myText.Text);

                if (myText.Text == "") return;
                
                sqlCommand.ExecuteScalar();
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                SQLQueries(showAllzoos, "Location", ListZoos);
            }

        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            string query = "Insert into ZooAnimal values(@ZooId,@animalID)";
            SqlCommand sqlcmd = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlcmd.Parameters.AddWithValue("@zooID", ListZoos.SelectedValue);
            sqlcmd.Parameters.AddWithValue("@animalID", ListAddAnimalToZoo.SelectedValue);
          
            sqlcmd.ExecuteReader();
            sqlConnection.Close();
            ShowAnimals();
        }

        private void deleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            string query = "Delete from Animal where id = @animalID";
            SqlCommand sqlcmd = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlcmd.Parameters.AddWithValue("@animalID", ListAddAnimalToZoo.SelectedValue);

            sqlcmd.ExecuteScalar();
            sqlConnection.Close();
            SQLQueries(showAllAnimals, "Name", ListAddAnimalToZoo);
            if (ListZoos.SelectedValue!=null)
            {
                ShowAnimals();
            }
            
        }

        private void removeAnimal_Click(object sender, RoutedEventArgs e)
        {
            string query = "Delete from ZooAnimal where AnimalId = @animalID and ZooID = @zooID " ;
            SqlCommand sqlcmd = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlcmd.Parameters.AddWithValue("@animalID", ListAnimal.SelectedValue);

            if (ListZoos.SelectedValue != null && ListAnimal.SelectedValue != null)
            {
                sqlcmd.ExecuteScalar();
                ShowAnimals();
            }
            sqlConnection.Close();
        }
    }

   
}
