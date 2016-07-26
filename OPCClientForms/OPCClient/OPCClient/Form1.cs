using Opc;
using Opc.Da;
using OpcCom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCClient
{
    public partial class Form1 : Form
    {

        Opc.Da.Server server = null;
        public Form1()
        {
            InitializeComponent();
            OpcCom.ServerEnumerator se = new ServerEnumerator();
            Opc.Server[] Servers;
            String sErrFunc = "";

            Opc.ConnectData cd = new Opc.ConnectData(new System.Net.NetworkCredential());
            sErrFunc = "GetAvailableServers";
            Servers = se.GetAvailableServers(Opc.Specification.COM_DA_20, "localhost", cd);
            Console.WriteLine(Servers[1].Name.ToString());


            Opc.URL url = new Opc.URL("opcda://localhost/" + Servers[0].Name);
            
            OpcCom.Factory fact = new OpcCom.Factory();
            server = new Opc.Da.Server(fact, null);
            server.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
        }

        private void getTree(Opc.Da.Server server)
        {
            BrowsePosition bp;
            ItemIdentifier itemID = new ItemIdentifier();
            BrowseFilters filters = new BrowseFilters();
            BrowseElement[] be = server.Browse(itemID, filters, out bp);
            TreeNode kepServ = null;
            int i = be.Length;
          //  MessageBox.Show(i.ToString());
            while (i != 0)
            {
                --i;
                kepServ = treeView1.Nodes.Add(be[i].ItemName);
            }
          //  treeView1.Nodes.Add(kepServ);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            getTree(server);
        }

        private Object getOPCValue(String ItemName, BrowseElement[] be)
        {
            // Create a group
            Opc.Da.Subscription group;
            Opc.Da.SubscriptionState groupState = new Opc.Da.SubscriptionState();
            groupState.Name = ItemName;
            groupState.Active = true;
            group = (Opc.Da.Subscription)server.CreateSubscription(groupState);
            //Console.WriteLine(group.);


            // add items to the group.
            Opc.Da.Item[] items = new Opc.Da.Item[1];

                items[0] = new Opc.Da.Item();
                items[0].ItemName = ItemName;

            items = group.AddItems(items);

            ItemValueResult[] ir = group.Read(items);
            return ir[0].Value;
        }



        private void DisplayChildren(TreeNode ParentNode, TreeNodeMouseClickEventArgs e)
        {
          
            BrowsePosition bp;
            ItemIdentifier itemID = new ItemIdentifier(e.Node.FullPath.Replace("\\","."), e.Node.FullPath.Replace("\\","."));
            BrowseFilters filters = new BrowseFilters();

            BrowseElement[] be = server.Browse(itemID, filters, out bp);
           // MessageBox.Show(bp.MaxElementsReturned.ToString());

            if (be != null)
            {
                
                int i = be.Length;
                // Вначале выводим все листья на данном уровне
                while (i != 0)
                {
                    --i;
                    TreeNode tvNode;

                    if (ParentNode == null)
                    { 
                        tvNode = treeView1.Nodes.Add(be[i].Name);
                        if (be[i].IsItem)
                        {
                            listView1.Items.Add(be[i].Name);
                            Object irr = getOPCValue(e.Node.FullPath.Replace("\\", ".") + "." + be[i].Name, be);

                        }

                    }
                    else
                    {
                        if (be[i].IsItem)
                        {
                            ListViewItem lvItem = new ListViewItem();
                            ListViewItem.ListViewSubItem[] lvSubItem = new ListViewItem.ListViewSubItem[3];

                            Object irr = getOPCValue(e.Node.FullPath.Replace("\\", ".") + "." + be[i].Name, be);

                            lvItem.Text = e.Node.FullPath.Replace("\\", ".") + "." + be[i].Name;
                            //lvItem.SubItems.Add(getOPCValue(be[i].ItemName).ToString());
                            lvItem.SubItems.Add(irr.ToString());

                            listView2.Items.Add(lvItem);
                        }
                        else {
                            tvNode = ParentNode.Nodes.Add(be[i].Name);
                        }
                     
                    }
                }
            }
            else
            {
                MessageBox.Show("be null");
            }


           //// BrowsePosition bp2;
           //// ItemIdentifier itemID2 = new ItemIdentifier(e.Node.NextNode.Text, e.Node.NextNode.Text);
           //// BrowseFilters filters2 = new BrowseFilters();
           //// BrowseElement[] be2 = server.Browse(itemID, filters, out bp);
        //    while (cnt != 0)
        //    {
        //        TreeNode tvNode;
        //        if (ParentNode == null)
        //            tvNode = treeView1.Nodes.Add(be[i].ItemName);
        //        else
        //            tvNode = ParentNode.Nodes.Add(strName);
        //       // pParent.ChangeBrowsePosition(tagOPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_DOWN, strName);
        //        DisplayChildren(tvNode, pParent); //Рекурсивно вызываем метод
        //        //После того как все дочерние узлы показаны постепенно поднимаемся
        //        //вверх
        //        pParent.ChangeBrowsePosition(tagOPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_UP,
        //        strName);
        //        pEnum.RemoteNext(1, out strName, out cnt);
        //    }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DisplayChildren(e.Node,e);
             
          //  MessageBox.Show(e.Node.FullPath);
          //  BrowsePosition bp;
          //  ItemIdentifier itemID = new ItemIdentifier(e.Node.Text, e.Node.Text);
          //  BrowseFilters filters = new BrowseFilters();
          //  BrowseElement[] be = server.Browse(itemID, filters, out bp);

          //  TreeNode kepServ = null;
          //  //MessageBox.Show( treeView1.GetNodeCount(true).ToString());
            
          //  int i = be.Length;
          ////  MessageBox.Show(be.Length.ToString());
          //  while (i != 0)
          //  {
          //      if (be[i-1].HasChildren)
          //      {
          //          kepServ = new TreeNode(be[i - 1].ItemName);

          //          treeView1.Nodes[treeView1.GetNodeCount(false) - 1].Nodes.Add(kepServ);
                    
          //      }

          //      else
          //      {
          //        //  MessageBox.Show("The tree is finishe there");
          //      }

          
               
          //      i--;
          //  }



           // e.Node.TreeView

        }


    }
}
