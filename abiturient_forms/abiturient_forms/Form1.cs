using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace abiturient_forms
{
    public partial class Form1 : Form
    {

        public static string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=abiturient.mdb;";
        private OleDbConnection myConnetion;

        public Form1()
        {
            InitializeComponent();
            myConnetion = new OleDbConnection(connectionString);
            myConnetion.Open();
            add_ComboBox();
            add_TreeView();
        }

        List<User> people = new List<User>();
        List<Direction> directions = new List<Direction>();

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(comboBox1.SelectedItem != null && treeView1.SelectedNode.Parent == null)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            myConnetion.Close();
        }

        private void add_ComboBox() //добавление пользователей в комбобокс
        {
            try
            {
                string query = "SELECT id_user, user_name, user_surname FROM [user] ORDER BY id_user";
                OleDbCommand command = new OleDbCommand(query, myConnetion);
                OleDbDataReader reader = command.ExecuteReader();

                comboBox1.Items.Clear();

                while (reader.Read())
                {
                    //reader[0].ToString() + " " + 
                    comboBox1.Items.Add(reader[1].ToString() + " " + reader[2].ToString());
                    people.Add(new User() { id_user = Convert.ToInt32(reader[0].ToString()), user_name = reader[1].ToString(), user_surname = reader[2].ToString() });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        void add_TreeView() 
        {
            try
            {
                string query = "SELECT id_direction, direction_name FROM [direction]"; 
                OleDbCommand command = new OleDbCommand(query, myConnetion);
                OleDbDataReader reader = command.ExecuteReader();
                

                treeView1.Nodes.Clear();
                int nodenumber = 0;

                while (reader.Read())// добавление напрвалений
                {

                    directions.Add(new Direction() { id_direction = Convert.ToInt32(reader[0].ToString()), direction_name = reader[1].ToString(),treeNode = nodenumber });
                    treeView1.Nodes.Add(reader[1].ToString());
                    nodenumber++;
                }

                reader.Close();

                query = "SELECT id_profile, profile_name, id_direction FROM [profile]";
                command = new OleDbCommand(query, myConnetion);
                reader = command.ExecuteReader();
            //    List<Profile> profiles = new List<Profile>();

                while (reader.Read())
                {
                    /*   profiles.Add(new Profile()
                       {
                           id_profile = Convert.ToInt32(reader[0].ToString()),
                           profile_name = reader[1].ToString(),
                           id_direction = Convert.ToInt32(reader[2].ToString())
                       });*/

                    for (int i = 0; i < directions.Count; i++)//добавление обьектов profile каждому обьекту direction
                            {
                        if (Convert.ToInt32(reader[2].ToString()) == directions[i].id_direction)
                        {
                            directions[i].profiles.Add(new Profile() {
                                              id_profile = Convert.ToInt32(reader[0].ToString()),
                                              profile_name = reader[1].ToString(),
                                              id_direction = Convert.ToInt32(reader[2].ToString())
                                                                    });
                            treeView1.Nodes[i].Nodes.Add(reader[1].ToString());
                        }
                            }
                    

                    //foreach(Direction i in directions)
                 /*   for (int i = 0; i < directions.Count; i++)
                    {
                        if (directions[i].id_direction == Convert.ToInt32(reader[2].ToString()))
                        {
                            treeView1.Nodes[i].Nodes.Add(reader[1].ToString());
                        }
                    }*/
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
              TreeNode node = treeView1.SelectedNode;
            //  string name = node.Text;
            //treeView2.Nodes.Add(treeView1.SelectedNode.Text + treeView1.SelectedNode.Index);
            // int i = treeView2.GetNodeCount(false);
            // int i = node.GetNodeCount(false);
            // int k = node.Index;
            List<UserProfile> luserProfiles = new List<UserProfile>();
            for (int i = 0; i < directions[node.Index].profiles.Count; i++)
            {
                luserProfiles.Add(new UserProfile() {id_user = people[comboBox1.SelectedIndex].id_user,
                                                    id_profile = directions[node.Index].profiles[i].id_profile,
                                                    profile_priority = i + 1});
            }


            userDirections.Add(new UserDirection() {id_user = people[comboBox1.SelectedIndex].id_user,
                                                    id_direction = directions[node.Index].id_direction,
                                                    direction_priority = userDirections.Count +1,
                                                    userProfiles = luserProfiles
                                                    });

            treeView2.Nodes.Clear();
            Add_treeview2();
        }


        List<UserDirection> userDirections = new List<UserDirection>();
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            button5.Enabled = true;
            // MessageBox.Show(people[comboBox1.SelectedIndex].user_name);
            treeView2.Nodes.Clear();
            string query = "SELECT id_user, id_direction, direction_priority FROM [user_direction] WHERE id_user =" + people[comboBox1.SelectedIndex].id_user + " ORDER BY direction_priority";
            OleDbCommand command = new OleDbCommand(query, myConnetion);
            OleDbDataReader reader = command.ExecuteReader();

            userDirections.Clear();
            while (reader.Read())
            {
                userDirections.Add(new UserDirection()
                {
                    id_user = Convert.ToInt32(reader[0].ToString()),
                    id_direction = Convert.ToInt32(reader[1].ToString()),
                    direction_priority = Convert.ToInt32(reader[2].ToString())
                });
            }

            query = "SELECT id_user, id_profile, profile_priority FROM [user_profile] WHERE id_user =" + people[comboBox1.SelectedIndex].id_user + " ORDER BY profile_priority";
            command = new OleDbCommand(query, myConnetion);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < userDirections.Count; i++)
                {
                    for (int y = 0; y < directions.Count; y++)
                    {
                        if (userDirections[i].id_direction == directions[y].id_direction)
                        {
                            for(int l = 0; l< directions[y].profiles.Count; l++)
                            {
                                if(directions[y].profiles[l].id_profile == Convert.ToInt32(reader[1].ToString()))
                                    {
                                    userDirections[i].userProfiles.Add(new UserProfile() {  id_user = Convert.ToInt32(reader[0].ToString()),
                                                                                            id_profile = Convert.ToInt32(reader[1].ToString()),
                                                                                            profile_priority = Convert.ToInt32(reader[2].ToString())
                                                                                            });
                                    }
                            }
                            
                        }
                    }

                }
            }



            Add_treeview2();

            




        }

        void Add_treeview2()
        {
            int k = 0;
            for (int i = 0; i < userDirections.Count; i++) //Добавление узлов и подузлов в treeView2
            {
                for (int y = 0; y < directions.Count; y++)
                {
                    if (userDirections[i].id_direction == directions[y].id_direction)
                    {
                        treeView2.Nodes.Add(directions[y].direction_name);

                        for (int l = 0; l < userDirections[i].userProfiles.Count; l++)
                        {
                            for (int p = 0; p < directions[y].profiles.Count; p++)
                            {
                                if (userDirections[i].userProfiles[l].id_profile == directions[y].profiles[p].id_profile)
                                {
                                    treeView2.Nodes[k].Nodes.Add(directions[y].profiles[p].profile_name);

                                }
                            }
                        }
                        /*  while (reader.Read())
                          {
                              for(int l = 0; l< directions[y].profiles.Count; l++)
                              {
                                  if (directions[y].profiles[l].id_profile == Convert.ToInt32(reader[1].ToString()))
                                  {
                                      userDirections[i].userProfiles.Add(new UserProfile() {
                                                                                             id_user = Convert.ToInt32(reader[0].ToString()),
                                                                                             id_profile = Convert.ToInt32(reader[1].ToString()),
                                                                                             profile_priority = Convert.ToInt32(reader[2].ToString())
                                                                                              });
                                      treeView2.Nodes[k].Nodes.Add(directions[y].profiles[l].ToString());
                                  }
                              }

                          }*/
                        // treeView2.Nodes[k].Nodes.Add("SALAM");
                        k++;
                    }
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            userDirections.RemoveAt(treeView2.SelectedNode.Index);
            treeView2.Nodes.Clear();
            Add_treeview2();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PriorityCorrect("-");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            PriorityCorrect("+");
        }


        void PriorityCorrect(string key)
        {
            int i = treeView2.SelectedNode.Index;
            //поменять приоритеты
            UserDirection userDirection_Swap;
            UserProfile userProfile_Swap;

            /*  userDirections[i].direction_priority--;
              userDirections[i - 1].direction_priority++;
              userDirections.RemoveAt(i);*/

            if (treeView2.SelectedNode.Parent == null)
            {
                userDirection_Swap = userDirections[i];
                if (key == "-")
                {
                    userDirections[i].direction_priority--;
                    userDirections[i - 1].direction_priority++;
                    userDirections.RemoveAt(i);
                    userDirections.Insert(i - 1, userDirection_Swap);
                    treeView2.Nodes.Clear();
                    Add_treeview2();
                    treeView2.SelectedNode = treeView2.Nodes[i - 1];
                    treeView2.Select();
                }

                if (key == "+")
                {
                    userDirections[i].direction_priority++;
                    userDirections[i + 1].direction_priority--;
                    userDirections.RemoveAt(i);
                    userDirections.Insert(i + 1, userDirection_Swap);
                    treeView2.Nodes.Clear();
                    Add_treeview2();
                    treeView2.SelectedNode = treeView2.Nodes[i + 1];
                    treeView2.Select();
                }

            }
            else
            {
                userProfile_Swap = userDirections[treeView2.SelectedNode.Parent.Index].userProfiles[i];
                int parrent = treeView2.SelectedNode.Parent.Index;
                if (key == "-")
                {
                    userDirections[parrent].userProfiles[i].profile_priority--;
                    userDirections[parrent].userProfiles[i - 1].profile_priority++;
                    userDirections[parrent].userProfiles.RemoveAt(i);
                    userDirections[parrent].userProfiles.Insert(i-1, userProfile_Swap);
                    treeView2.Nodes.Clear();
                    Add_treeview2();
                    treeView2.SelectedNode = treeView2.Nodes[parrent].Nodes[i - 1];
                    treeView2.Select();
                }

                if (key == "+")
                {
                    userDirections[parrent].userProfiles[i].profile_priority++;
                    userDirections[parrent].userProfiles[i + 1].profile_priority--;
                    userDirections[parrent].userProfiles.RemoveAt(i);
                    userDirections[parrent].userProfiles.Insert(i + 1, userProfile_Swap);
                    treeView2.Nodes.Clear();
                    Add_treeview2();
                    treeView2.SelectedNode = treeView2.Nodes[parrent].Nodes[i + 1];
                    treeView2.Select();
                }
                
                
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM user_direction WHERE id_user = " + people[comboBox1.SelectedIndex].id_user;
            OleDbCommand command = new OleDbCommand(query, myConnetion);
            command.ExecuteNonQuery();

            query = "DELETE FROM user_profile WHERE id_user = " + people[comboBox1.SelectedIndex].id_user;
            command = new OleDbCommand(query, myConnetion);
            command.ExecuteNonQuery();


            for(int i = 0; i<userDirections.Count; i++)
            {
                query = "INSERT INTO user_direction (id_user, id_direction, direction_priority) VALUES (" + userDirections[i].id_user + "," + userDirections[i].id_direction + ", " + userDirections[i].direction_priority + ")";
                command = new OleDbCommand(query, myConnetion);
                command.ExecuteNonQuery();
                for(int k = 0; k<userDirections[i].userProfiles.Count; k++)
                {
                    query = "INSERT INTO user_profile (id_user, id_profile, profile_priority) VALUES (" + userDirections[i].userProfiles[k].id_user + "," + userDirections[i].userProfiles[k].id_profile + ", " + userDirections[i].userProfiles[k].profile_priority + ")";
                    command = new OleDbCommand(query, myConnetion);
                    command.ExecuteNonQuery();
                }
            }


        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(treeView2.SelectedNode.Index == 0)
            {
                button3.Enabled = false;
            }
            else
            {
                button3.Enabled = true;
            }

            if (treeView2.SelectedNode.Parent == null)
            {
                if (treeView2.SelectedNode.Index + 1 == treeView2.Nodes.Count)
                {
                    button4.Enabled = false;
                }
                else
                {
                    button4.Enabled = true;
                }
            }
            else
            {
                if(treeView2.SelectedNode.Index + 1 == treeView2.Nodes[treeView2.SelectedNode.Parent.Index].Nodes.Count)
                {
                    button4.Enabled = false;
                }
                else
                {
                    button4.Enabled = true;
                }
            }
                
            //button3.Enabled = true;
          //  button4.Enabled = true;
         //   button2.Enabled = true;
            if (treeView2.SelectedNode.Parent == null)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }
    }
}

