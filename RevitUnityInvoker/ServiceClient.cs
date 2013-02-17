//this is for c#
using System;//this is required for any .net application
//using System.Collections.Generic;//this library is used to use collection variable such Vector. this is not an 3D vector, but a vector of variable, you can think it as an array
using System.Linq;////this is also required to run  c# application
using System.Text;//several text function is in this namespace
//this is for sdk
using Autodesk.Revit.UI;//all revit plugin require this
using Autodesk.Revit.DB;//all revit plugin require this
//this is my lib
using CrawlerLib.Net;//this is my library. I implemented some useful functions to help me do the things faster
//this is for network
//using System.Collections.Specialized;//this is supported classes for network function        
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Media.Imaging;//this is network namespace (POST and GET in HTTP protocol)

namespace RevitUnityInvoker   //define new namespace,it is not neccessary to use the namespace which we are building. We use the "using...." to reuse the built library.
{


    /*
     From revit sdk, it said that any plugin has to have the attribute:
     * [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
     * and the entry class ServiceClient always inherited from IExternalCommand.you can change the ServiceClient name, but can not change IExternalCommand since it have been fixed by SDK.
     * and the entry function is always be 
     *  public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
     *  where the return is always:
     *  return Autodesk.Revit.UI.Result.Succeeded;
     */
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
    public class ServiceClient : IExternalCommand, IServiceClient //DerivedClass : BaseClass, Interface
      
    {

        /*public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Revit Unity");
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("RevitUnity", "Revit Unity", @"C:\Users\user\Desktop\TestProject11\RevitUnity\RevitUnity\RevitUnityInvoker\bin\Debug\RevitUnityInvoker.dll", "RevitUnityInvoker.ServiceClient")) as PushButton;
            Uri uriImage = new Uri(@"C:\universitylogo.jpg");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }*/


        private string selectedServer = "";//this is to store server IP
        private string selectedSid = "";//this is to store server ID. I use this to know that if this plugin has connected to server or not
        JoinForm joinform = new JoinForm(); //we create class varies, more specifically,we create a class instance. 
        //Class JoinForm is defined class in JoinForm.cs file, inherit from form


        //SyncForm syncform = new SyncForm();
        //SyncForm is defined class in SyncForm.cs file

        UIDocument uidoc = null; //this is used to access property and objects in 3d model. I get this parameter at line uidoc = revit.Application.ActiveUIDocument;
        WebclientX client = new WebclientX();//this is HTTP protocol class which is used to request to PHP server.

        public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            //this is our modification
            run();//is my main function which is exporting model and semantic information
            uidoc = revit.Application.ActiveUIDocument;//get uidoc, what is sentence of revit.Application.ActiveUIDocument for??
            //I don't understand this line, but I know that I need to get uidoc to access component in revit for example at line "collector = new FilteredElementCollector(uidoc.Document);", I get collection of walls.
            //and in order to get uidoc, we need to use "revit.Application.ActiveUIDocument". I got this in manual
            //end modification

            //we cannot change the return
            return Autodesk.Revit.UI.Result.Succeeded;//page 26 in Revit SDK Manual
        } // the first execute function?? yes


        //in OOP, object oriented programming
        private void run() //why use it as private?
            //private cannot be called from other classes
            //public can be called from other classes
            //default (without any privacy or public) is privacy
        {
            /*if (selectedServer == "") // this is the first sentence when we run plugin in Revit, as default, selectedServer = "", so it will go into the bracket.
            {*/
                joinform.main = this; // what is this here mean? "this" means current instance of ServiceClient class
                //I defined interface instance of ServiceClient: "main" in JoinForm to hold ServiceClient instance. what is ServiceClient instance?? 
                //joinform is a form to let us choose host from a list.
               
                //After we choose which host we want to connect to, we need to callback to the main process  (click the "join button"), and let us to see the option form (Sync form). So that we need a way to callback.
                //In order to callback we need to assign this process to that form. That why I use "joinform.main = this". The variable main in joinform is standed for this process.
                //In JoinForm.cs, at line " main.FinishedJoinForm();", you can see how I called back to this form.

                joinform.Show(); // call the joinform which is a instance of Class JoinForm in JoinForm.cs, show the joinform and use the code in JoinForm.cs. 
                // What is show() mean here?? how to go into JoinForm.cs? show the form,  joinform load function work automatically

            /*}
            else
            {
                ShowSyncForm();// call the ShowSyncForm function below, it would not be used and can be deleted cause it shows in function FinishJoinForm!
            }*/
        }

        public void SetServer(string server, string sid)// In JoinForm.cs: main.SetServer(ips[comboBoxServers.SelectedIndex], sids[comboBoxServers.SelectedIndex]);
        {
            //this is where we are backed from JoinForm to assign host ip and host id.
            selectedServer = server;
            selectedSid = sid;
        }

        /*public void ShowSyncForm() 
        {
            syncform.SetServerForSync(selectedServer, selectedSid); // SetServerForSync function is defined in SyncForm.cs. is the selectedServer and selectedSid same as in SetServer??? 
            syncform.main = this;//  simlilar to joinform.main = this
            syncform.Show();// similar to joinform.Show(). click button go into SyncForm.load, setted in event
        }*/


        public void FinishedJoinForm() // in JoinForm.cs:  main.FinishedJoinForm();
        {
            //this is where we are back after the host selection in JoinForm
            //if we do not select anything or because of any error make selected ip is empty, we show the error message
            if (selectedServer == "")
            {
                TaskDialog.Show("Error", "Please choose server from list");// what is difference from connectToServerForm.main = this?
            }
            else //if we selected server, we will be go through this statement
                
            {
                //we want to send selected server id to server and let server know that revit client has connected to unity host
                
                //set POST parameter
                NameValueCollection paras = new NameValueCollection();// /create class varies,NameValueCollection is defined by .net. it contains POST variable to post to server
                paras.Add("s_id", selectedSid);//?? this means we want to post to server a server id by selectedSid which is filled by SetServer function after we join a form

                client.PostMethod(AppConst.SERVER_DOMAIN + AppConst.SERVER_PATH + "?act=ClientJoin", paras);
                //elseif ($_GET["act"] == 'ClientJoin'	ClientJoin($_POST["s_id"]) in php :unity sever/index.php


                //ShowSyncForm();// call function above
            }
        }

        public void SyncModel()//  in SyncForm.cs
        {
            ExportToFbx(); // call the function below
        }

        private void ExportToFbx()
        {
            try
            {
                ElementCategoryFilter eleFilter;//the class varies from Autodesk.Revit.DB? y
                FilteredElementCollector collector;//the class varies from Autodesk.Revit.DB? y
                IList<Element> filteredObject;// the interface of System.Collections.Generic? y where did it state it is the interface? in .net lib
                string semanticInfo = "";//store all semantic information
                string commentParameter = "";//store the comment parameter in left column revit

                //get ground objects
				//read semantic information from revit file
				
				//for floor
                eleFilter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);// get all floor objects in revit project
                collector = new FilteredElementCollector(uidoc.Document);//the same way to get objects in revit
                filteredObject = collector.WherePasses(eleFilter).WhereElementIsNotElementType().ToElements();// the same way to get objects in revit
                //we create filter instance and all objects instance. then we pass filter into all objects to get filteredbojects.
                foreach (Element e in filteredObject)//go throught each item/objects in revit model
                {
                    commentParameter = "";// reset semantic information in every object every time
                    foreach (Parameter para in e.Parameters)//loop each property or parameters on the left column revit 
                    {
                        try
                        {
                            commentParameter = GetParameterInformation(para, uidoc.Document).ToLower();// right of left column
                            if (commentParameter == "emergency")
                                break; // if yes, go to the if emergency, if no, go to else
                        }
                        catch (Exception ex) // 
                        {
                            //cannot get info
                        }
                    }

                    string level = "";// reset semantic information in every object every time
                    foreach (Parameter para in e.Parameters)//loop each property or parameters on the left column revit 
                    {
                        try
                        {
                            level = GetParameterInformation(para, uidoc.Document).ToLower();// right of left column
                            if (level.StartsWith("level"))
                                break; // if yes, go to the if emergency, if no, go to else
                        }
                        catch (Exception ex) // 
                        {
                            //cannot get info
                        }
                    }

                    if (commentParameter == "emergency")
                        semanticInfo += e.Id + "-emergency,"+level+"\n";// explain this please. put id and emergency into the whole semantic information list with different line.
                    else
                        semanticInfo += e.Id + "-ground," + level + "\n";
                }

				//for stairs
                eleFilter = new ElementCategoryFilter(BuiltInCategory.OST_Stairs);
                collector = new FilteredElementCollector(uidoc.Document);
                filteredObject = collector.WherePasses(eleFilter).WhereElementIsNotElementType().ToElements();
                foreach (Element e in filteredObject)
                {
                    semanticInfo += e.Id + "-stair\n";
                }

                //get obstacle objects///for walls
                eleFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);//I get all walls
                collector = new FilteredElementCollector(uidoc.Document);
                filteredObject = collector.WherePasses(eleFilter).WhereElementIsNotElementType().ToElements();
                foreach (Element e in filteredObject)//I loop each wall. I want to check the COMMENT property of each wall
                {
                    /*//this block is used to get Comment property in revit
                    commentParameter = "";//this parameter store Comment property value
                    foreach (Parameter para in e.Parameters)//loop each property
                    {
                        try
                        {
                            commentParameter = GetParameterInformation(para, uidoc.Document).ToLower();
                            if (commentParameter == "wood")
                                break;
                        }
                        catch (Exception ex)
                        {
                            //cannot get info, we know it and we don't need to write it
                      
                        }
                    }

                    //if it is wood, we set wood to the semantic file, otherwise it is obstable (brick)
                    if (commentParameter == "wood")
                        semanticInfo += e.Id + "-wood\n";
                    //else if (status == "somethingelse")
                    //content += e.Id + "-somethingelse\n";
                    else*/
                        semanticInfo += e.Id + "-obstacle\n";
                }
			
				//for doors
                eleFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
                collector = new FilteredElementCollector(uidoc.Document);
                filteredObject = collector.WherePasses(eleFilter).WhereElementIsNotElementType().ToElements();
                foreach (Element e in filteredObject)
                {
                    foreach (Parameter para in e.Parameters)
                    {
                        try
                        {
                            commentParameter = GetParameterInformation(para, uidoc.Document).ToLower();
                            if (commentParameter == "closed" || commentParameter == "opened")
                                break;
                        }
                        catch (Exception ex)
                        {
                            //cannot get info
                        }
                    }
					
                    if (commentParameter == "closed")
                        semanticInfo += e.Id + "-obstacle\n";
                    else if (commentParameter == "opened")
                        semanticInfo += e.Id + "-openeddoor\n";
                    else
                        semanticInfo += e.Id + "-openeddoor\n"; // else mean the default status??? yes
                }



                string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);


                CreateTempFolder(path + "\\revitfbx\\");
                //output
                //System.IO.Directory.CreateDirectory("revitfbx");
                System.IO.File.Delete(path + "\\revitfbx\\building.fbx");
                System.IO.File.Delete(path + "\\revitfbx\\semantic.txt");
				
				//we write new fbx
				////////////////////////////
                ViewSet views = new ViewSet();
                views.Insert(uidoc.ActiveView);
                FBXExportOptions exportoption = new FBXExportOptions();
                //here, revit forces us to use a folder. I forgot that. so we have to use a folder.
                uidoc.Document.Export(path + "\\revitfbx", "building.fbx", views, exportoption);//generate fbx file
				/////////////////////////////
				
				//we write new semantic
                Utility.WriteFile(path + "\\revitfbx\\semantic.txt", semanticInfo, false);

				
				//send semantic and 3d fbx model to PHP server
                
                NameValueCollection paras = new NameValueCollection();
                paras.Add("sid", selectedSid);
                paras.Add("act", "ClientSendModelFile");
                client.UploadFileEx(path + "\\revitfbx\\building.fbx", AppConst.SERVER_DOMAIN + AppConst.SERVER_PATH, "building", null, paras);
				
                paras = new NameValueCollection();
                paras.Add("sid", selectedSid);
                paras.Add("act", "ClientSendSemanticFile");
                client.UploadFileEx(path + "\\revitfbx\\semantic.txt", AppConst.SERVER_DOMAIN + AppConst.SERVER_PATH, "semantic", null, paras);

            }
			///////////////////////////
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        private void CreateTempFolder(string subPath)
        {
            bool IsExists = System.IO.Directory.Exists(subPath);

            if (!IsExists)
                System.IO.Directory.CreateDirectory(subPath);
        }

		//this function will get parameter information
        String GetParameterInformation(Parameter para, Document document)
        {
            string defName = para.Definition.Name;
            if (defName != "Comments" && defName != "Level")
                return "";

            // Use different method to get parameter data according to the storage type
            switch (para.StorageType)
            {
                case StorageType.Double:
                    //covert the number into Metric
                    return para.AsValueString();
                case StorageType.ElementId:
                    //find out the name of the element
                    Autodesk.Revit.DB.ElementId id = para.AsElementId();
                    if (id.IntegerValue >= 0)
                    {
                        return document.get_Element(id).Name;
                    }
                    else
                    {
                        return id.IntegerValue.ToString();
                    }
                case StorageType.Integer:
                    if (ParameterType.YesNo == para.Definition.ParameterType)
                    {
                        if (para.AsInteger() == 0)
                        {
                            return "False";
                        }
                        else
                        {
                            return "True";
                        }
                    }
                    else
                    {
                        return para.AsInteger().ToString();
                    }
                case StorageType.String:
                    return para.AsString();
                default:
                    return "Unexposed parameter.";
            }
        }
    }
}
