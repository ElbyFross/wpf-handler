//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using BusinessLogic.Descriptros;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Configuration;
using MySql.Data.MySqlClient;
using WpfHandler.UI.Controls;

namespace BusinessLogic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MySqlConnection connection;

        readonly DataTableDescriptor tableDescriptor = new DataTableDescriptor();
        readonly NewItemTabDescriptor newElementDescriptor = new NewItemTabDescriptor();
        readonly MySqlConnectionFormDescriptor serverCnnectionFormDescriptor = new MySqlConnectionFormDescriptor();
        readonly AutoLayoutVeiw tableView = new AutoLayoutVeiw();
        readonly AutoLayoutVeiw newElementView = new AutoLayoutVeiw();

        public MainWindow()
        {
            InitializeComponent();

            // Declaring Palette as global sharable option.
            var sharedOptions = new ISharableGUILayoutOption[] 
            { new PaletteAttribute("#ff4757", "#FCFEFF", "#D8E6F2", "#2f3542", "#F2E8D8")};

            // Applying sharable objects to the descriptors.
            // In that case sharable options will affect not only elements instantiated by descriptor
            // but also the descriptor Panel itself.
            //tableDescriptor.SharedLayoutOptions = sharedOptions;
            newElementDescriptor.SharedLayoutOptions = sharedOptions;

            // Appllying descriptor instances to the AutoLayoutVeiw.
            tableView.OnLayout(tableDescriptor);
            newElementView.OnLayout(newElementDescriptor);

            // Defining current active tab.
            switchPanel.Current = tableView;

            tableDescriptor.controlPanel.NewItemTab += async delegate ()
            {
                // Switches to the `New item` tab.
                await switchPanel.SwitchToAsync(newElementView);
            };

            tableDescriptor.controlPanel.Refresh += RefreshData;

            // Defining behavior for `ToTableTab` action at the `New item` form.
            newElementDescriptor.ToTableTab += async delegate ()
            {
                // Switches back to the `Table` tab.
                await switchPanel.SwitchToAsync(tableView);
            };

            // Defining the handler for AddItem action.
            newElementDescriptor.form.AddItem += delegate ()
            {
                AddItem(
                    newElementDescriptor.form.title,
                    newElementDescriptor.form.description,
                    newElementDescriptor.form.price);
            };

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Occurs when main window is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Getting table as AutoCollection element.
            var table = tableDescriptor.GetField<AutoCollection>("table");

            // Overriding default remove (`-`) button handler.
            table.OnRemoveClick += delegate (object taget)
                {
                    if (table.SelectedIndex >= 0)
                    {
                        // Removeing element from server.
                        RemoveItem(((TableRowDescriptor)table.SelectedField.Value).id);

                        // removing element from table.
                        table.RemoveAt(table.SelectedIndex);
                    }
                };

            // Hiding main window.
            this.Hide();

            // Requesting connection.
            var connectionWindow = new Window()
            {
                MinHeight = 265,
                MaxHeight = 265,
                MinWidth = 350,
                MaxWidth = 350,
                WindowStyle = WindowStyle.None,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // Close the app if connection canceled.
            serverCnnectionFormDescriptor.cancel += delegate ()
            {
                connectionWindow.Close();
                this.Close();
            };

            // Validating connection.
            serverCnnectionFormDescriptor.Connect = delegate ()
            {
                bool result = ValidateConnection();

                if(result)
                {
                    this.Show();
                    connectionWindow.Close();

                    // Requesting data.
                    RefreshData();
                }
            };

            // Creating auto layout view and applying the descriptor.
            var connectionFormView = new AutoLayoutVeiw();
            connectionFormView.OnLayout(serverCnnectionFormDescriptor);

            // Oppening the window.
            connectionWindow.Content = connectionFormView;
            connectionWindow.ShowDialog();
        }

        // Attention: code below is not an example of good practice.
        #region SQL related API
        /// <summary>
        /// Configurating connection to the mysql server.
        /// </summary>
        public void MySqlConnectionInit()
        {
            string connectionString;
            connectionString =
                "SERVER=" + serverCnnectionFormDescriptor.server + ";" +
                (string.IsNullOrEmpty(serverCnnectionFormDescriptor.database) ? "" : "DATABASE=" + serverCnnectionFormDescriptor.database + ";") +
                "port=" + serverCnnectionFormDescriptor.port + ";" +
                "User Id=" + serverCnnectionFormDescriptor.userId + ";" +
                "PASSWORD=" + serverCnnectionFormDescriptor.password + ";";

            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Trying to connect to a server with current params.
        /// </summary>
        /// <returns>Restul of connection.</returns>
        public bool ValidateConnection()
        {
            MySqlConnectionInit();

            try
            {
                connection.Open();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!\n" + ex.Message);
                return false;
            }

            return true;
        }

        public void RefreshData()
        {
            // Receiving field instantiated by descriptor.
            var table = tableDescriptor.GetField<IList>("table");

            // Clearing current data.
            table.Clear();

            // Executing select query.
            string sql = " SELECT * FROM `item`  ";
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                connection.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                /// Adding data to the table.
                while (reader.Read())
                {
                    // Genarating source.
                    var item = new TableRowDescriptor()
                    {
                        id = reader.GetInt32("id"),
                        title = reader.GetString("title"),
                        description = reader.GetString("description"),
                        price = reader.GetFloat("price"),
                    };

                    // Adding to the table. (affect UI and binded member)
                    table.Add(item);
                }
            }
        }

        public void AddItem(string title, string description, float price)
        {
            ExecuteScalarSql(string.Format(
                "INSERT INTO  `item` (`title`, `description`, `price`) VALUES('{0}', '{1}', {2});",
                title, description, price));

            RefreshData();
        }

        public void RemoveItem(int id)
        {
            ExecuteScalarSql("DELETE FROM `item` WHERE `id`=" + id);
        }

        public void UpdateItem(TableRowDescriptor row)
        {
            ExecuteScalarSql(string.Format(
               "UPDATE `item` SET `title`={0}, `description`={1}, `price`={2} WHERE `id`={3};",
               row.title, row.description, row.price, row.id));
        }

        private void ExecuteScalarSql(string sql)
        {
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                cmd.ExecuteScalar();
                connection.Close();
            }
        }
        #endregion
    }
}
