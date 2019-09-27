using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using WorkFlowManagement.Activities;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;
using String = System.String;

namespace WorkFlowManagement.CustomViews
{
    public class FormTabular : LinearLayout
    {
        private UserSession currentSession;
        private RelativeLayout theme;
        private Resources resource;
        private Button CreateCopyButton;
        private Context context;

        private int OwnerID;
        private int verifierID;
        private int userID;

        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;

        private LinearLayout MainTabularView;
        LinearLayout tabular;

        private List<LinearLayout> listOfTabuarElementList;
        private RelativeLayout.LayoutParams paramsTabularform;

        private int mainDuplicateCount;
        private int internalDuplicate;

        private string mainElementName;

        private ReportElement MainTabularElement;
        private string MainElenetType;

        private int trackChildID;
        public List<String> tabularImages;
        private bool isArcheived;
        private InformationPopup Popup;
        private Enum.ReportStatus reportStatus;

        private List<ReportElement> globalReportElement;

        public FormTabular(Context contextt, ReportElement element, int ownerID, int VerifiedID, string type, Enum.ReportStatus ReportStatus)
            : base(contextt)
        {
            context = contextt;
            resource = context.Resources;
            verifierID = VerifiedID;
            Orientation = Orientation.Vertical;
            MainTabularElement = element;
            OwnerID = ownerID;
            MainElenetType = type;
            Popup = new InformationPopup(context);
            reportStatus = ReportStatus;

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();

            //currentSession = new SessionService(new SQLiteAndriod().GetConnection()).GetCurrentSession();

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            //userID = currentSession.Id;
            userID = 1;
            theme = new FormTheme(context, MainTabularElement.Title);

            Popup.activateElementInfo(theme, element);

            listOfTabuarElementList = new List<LinearLayout>();
            tabularImages = new List<string>();

            AddView(theme);

            TabularAddButton addButtonLayer = new TabularAddButton(Context);
            ImageButton MainTemplateButton = (ImageButton)addButtonLayer.GetChildAt(1);

            mainElementName = MainTabularElement.Title;
            //MainTemplateButton.Click += (sender1, e) => copyTabular(sender1, e, MainTabularElement.Child[0]);
            //globalReportElement = MainTabularElement.Child[0];

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (verifierID != 0)
                {
                    MainTemplateButton.Enabled = false;
                    MainTemplateButton.Clickable = false;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        MainTemplateButton.Enabled = true;
                        MainTemplateButton.Clickable = true;
                    }
                }

                else
                {
                    MainTemplateButton.Enabled = true;
                    MainTemplateButton.Clickable = true;
                }
            }
            else
            {
                MainTemplateButton.Enabled = false;
                MainTemplateButton.Clickable = false;
            }

            if (isArcheived)
            {
                MainTemplateButton.Enabled = false;
                MainTemplateButton.Clickable = false;
            }


            AddView(addButtonLayer);

            //for (int i = 1; i < MainTabularElement.Child.Count; i++)
            //{
            //    RelativeLayout NewElementbutton = new TabularItemButton(Context);
            //    TextView nameTabular = (TextView)NewElementbutton.GetChildAt(0);
            //    TextView numberTabular = (TextView)NewElementbutton.GetChildAt(1);
            //    nameTabular.Text = MainTabularElement.Title;
            //    numberTabular.Text = i + "";
            //    NewElementbutton.Tag = i;

            //    int i1 = i;
            //    NewElementbutton.Click += (sender2, e) => openTabularActivity(sender2, e, MainTabularElement.Child[i1], i1, MainTabularElement.Title);
            //    AddView(NewElementbutton);
            //}

            SetPadding(45, 10, 45, 20);
        }

        //private void copyTabular(object sender, EventArgs eventArgs, List<ReportElement> elements)
        //{

        //    MainTabularElement.Child.Add(globalReportElement);

        //    RelativeLayout NewElementbutton = new TabularItemButton(Context);
        //    TextView nameTabular = (TextView)NewElementbutton.GetChildAt(0);
        //    TextView numberTabular = (TextView)NewElementbutton.GetChildAt(1);

        //    nameTabular.Text = mainElementName;
        //    numberTabular.Text = (MainTabularElement.Child.Count - 1) + "";

        //    NewElementbutton.Tag = MainTabularElement.Child.Count - 1;

        //    string no = NewElementbutton.Tag.ToString();
        //    int convertedTag = Integer.ParseInt(no);
        //    NewElementbutton.Click += (sender3, e) => openTabularActivity(sender3, e, MainTabularElement.Child[convertedTag], convertedTag, MainTabularElement.Title + " " + no);

        //    AddView(NewElementbutton);

        //    sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
        //    sharedPreferencesEditor.Commit();
        //}

        private void openTabularActivity(object sender, EventArgs eventArgs, List<ReportElement> element, int id, string name)
        {
            trackChildID = id;
            String json = JsonConvert.SerializeObject(element);
            String repStatus = JsonConvert.SerializeObject(reportStatus);

            Intent intent = new Intent(context, typeof(TabularActivity));
            intent.PutExtra("JasonTabularData", json);
            intent.PutExtra("ReportStatus", repStatus);
            intent.PutExtra("TabularType", MainElenetType);
            intent.PutExtra("OwnerID", OwnerID);
            intent.PutExtra("VerifierID", verifierID);
            intent.PutExtra("NameOfTheTabular", name);


            sharedPreferencesEditor.PutString("MainTabularID", MainTabularElement.Id + "");
            sharedPreferencesEditor.PutString("MainTabularType", MainElenetType);
            sharedPreferencesEditor.PutBoolean("FromTabular", true);
            sharedPreferencesEditor.Commit();

            context.StartActivity(intent);
        }

        public void UpdateMainTabular(string jsonObject)
        {
            List<ReportElement> reportElementList = JsonConvert.DeserializeObject<List<ReportElement>>(jsonObject);
            //MainTabularElement.Child[trackChildID] = reportElementList;
            int test = 0;
        }

        //public List<List<ReportElement>> getTabularData()
        //{
        //    //return MainElementList;
        //    //return MainTabularElement.Child;
        //}

        public void UpDateImagess(List<String> images)
        {
            tabularImages = images;
            //return images;
        }

        public List<String> getImages()
        {
            List<String> images = tabularImages;
            return images;
        }
    }
}