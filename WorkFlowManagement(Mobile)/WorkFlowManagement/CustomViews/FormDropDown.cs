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
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;
using Orientation = Android.Widget.Orientation;

namespace WorkFlowManagement.CustomViews
{
    public class FormDropDown : LinearLayout
    {
        private RelativeLayout theme;
        private Resources resource;
        private RelativeLayout timeFrame;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        private LinearLayout dropDownFrame;
        private bool isArcheived;
        private List<KeyValue> dropDownValues;
        private Context contextx;
        private ImageView indicatorImage;
        private ReportElement elementThis;
        MultiSelectSpinner multiSelectionSpinner;
        private bool firstTime;
        private ArrayAdapter<string> adapterSpinner;
        private List<string> spinnerItemsList;
        private Spinner spinner;
        private int OwnerID;
        private int VerifierID;
        private InformationPopup Popup;
        private ReportStatus reportStatus;
        private int selection;

        public FormDropDown(Context context, ReportElement element, int userID, int ownerID, int verifiedID, ReportStatus ReportStatus)
            : base(context)
        {
            contextx = context;
            resource = context.Resources;
            OwnerID = ownerID;
            VerifierID = verifiedID;
            elementThis = element;
            theme = new FormTheme(context, element.Title);
            Orientation = Orientation.Vertical;
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            firstTime = true;
            Popup = new InformationPopup(context);
            reportStatus = ReportStatus;
            selection = 0;

            isArcheived = sharedPreferences.GetBoolean(Resources.GetString(Resource.String.is_archived), false);

            dropDownValues = new List<KeyValue>();

            dropDownFrame = new LinearLayout(context);

            dropDownFrame.Orientation = Orientation.Vertical;

            indicatorImage = (ImageView)theme.GetChildAt(1);
            //activateElementInfo(element);
            Popup.activateElementInfo(theme, element);


            if (element.IsMultiSelect)
            {
                dropDownValues = element.Values;
            }

            else
            {
                KeyValue x = new KeyValue();
                x.Name = "Choose One";
                x.Value = "false";
                dropDownValues.Add(x);
                dropDownValues.AddRange(element.Values);
            }


            if (element.IsMultiSelect)
            {
                CreateMultiDropDown(context, indicatorImage, userID);
            }
            else
            {
                CreateDropDown(context, indicatorImage, element, userID);
            }

            AddView(theme);
            AddView(dropDownFrame);
            SetPadding(45, 10, 45, 20);
        }

        private void CreateDropDown(Context context, ImageView indicatorImage, ReportElement element, int userID)
        {
            spinner = new Spinner(context);
            RelativeLayout addMoreLayout = new RelativeLayout(context);
            ImageButton addItem = new ImageButton(context);

            RelativeLayout.LayoutParams paramsForSpinner = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForSpinner.AddRule(LayoutRules.AlignParentStart);
            paramsForSpinner.AddRule(LayoutRules.StartOf, addItem.Id);
            paramsForSpinner.AddRule(LayoutRules.AlignBottom, addItem.Id);
            paramsForSpinner.SetMargins(0, 0, 70, 0);
            spinner.LayoutParameters = paramsForSpinner;

            RelativeLayout.LayoutParams paramsForAddItem = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForAddItem.AddRule(LayoutRules.AlignParentEnd);
            addItem.LayoutParameters = paramsForAddItem;
            addItem.SetImageResource(Resource.Drawable.addbutton);
            addItem.SetBackgroundResource(0);

            spinnerItemsList = new List<string>();

            for (int i = 0; i < dropDownValues.Count; i++)
            {
                spinnerItemsList.Add(dropDownValues[i].Name);
                if (dropDownValues[i].Value == "true")
                {
                    selection = i;
                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                }
                else
                {
                    indicatorImage.SetImageResource(0);
                }
            }

            if (dropDownValues.Count == 0)
            {
                spinner.SetSelection(0);
            }

            adapterSpinner = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem, spinnerItemsList);
            //adapterSpinner = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerDropDownItem, spinnerItemsList);
            adapterSpinner.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapterSpinner;
            spinner.SetSelection(selection);

            addItem.Click += (sender3, e) => add_item_click(sender3, e);

            spinner.ItemSelected += spinner_ItemSelected;

            if (isArcheived)
            {
                spinner.Enabled = false;
                addItem.Enabled = false;
                addItem.Clickable = false;
            }

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    spinner.Enabled = false;
                    addItem.Enabled = false;
                    addItem.Clickable = false;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        spinner.Enabled = true;
                        addItem.Enabled = true;
                        addItem.Clickable = true;
                    }
                }

                else
                {
                    spinner.Enabled = true;
                    addItem.Enabled = true;
                    addItem.Clickable = true;
                }
            }
            else
            {
                spinner.Enabled = false;
                addItem.Enabled = false;
                addItem.Clickable = false;
            }

            if (element.AddNew)
            {
                addMoreLayout.AddView(spinner);
                addMoreLayout.AddView(addItem);
                dropDownFrame.AddView(addMoreLayout);
            }
            else
            {
                dropDownFrame.AddView(spinner);
            }

        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {

            if (!firstTime)
            {
                if (itemSelectedEventArgs.Position != 0)
                {
                    for (int j = 0; j < dropDownValues.Count; j++)
                    {
                        dropDownValues[j].Value = "false";
                    }
                    dropDownValues[itemSelectedEventArgs.Position].Value = "true";

                    indicatorImage.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                    sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                    sharedPreferencesEditor.Commit();
                }
            }
            else
            {
                firstTime = false;
            }

        }

        private void CreateMultiDropDown(Context context, ImageView indicatorImage, int userID)
        {
            List<string> array = arrayofNames();
            multiSelectionSpinner = new MultiSelectSpinner(context, indicatorImage);
            RelativeLayout addMoreLayout = new RelativeLayout(context);
            ImageButton addItem = new ImageButton(context);

            multiSelectionSpinner.setItems(array);
            multiSelectionSpinner.setSelection(arrayofSelection().ToArray());

            RelativeLayout.LayoutParams paramsForSpinner = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            paramsForSpinner.SetMargins(0, 0, 70, 0);
            multiSelectionSpinner.LayoutParameters = paramsForSpinner;

            RelativeLayout.LayoutParams paramsForAddItem = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            paramsForAddItem.AddRule(LayoutRules.AlignParentRight);
            paramsForAddItem.AddRule(LayoutRules.RightOf, multiSelectionSpinner.Id);
            addItem.LayoutParameters = paramsForAddItem;
            //addItem.SetImageResource(Resource.Drawable.addmore);
            addItem.SetImageResource(Resource.Drawable.addbutton);
            addItem.SetBackgroundResource(0);
            addItem.Click += (sender3, e) => add_multi_item_click(sender3, e, multiSelectionSpinner);

            if (isArcheived)
            {
                multiSelectionSpinner.Enabled = false;
            }

            if (OwnerID == 0 || OwnerID == userID)
            {
                if (VerifierID != 0)
                {
                    multiSelectionSpinner.Enabled = false;
                    addItem.Enabled = false;
                    addItem.Clickable = false;

                    if (reportStatus == ReportStatus.Rejected)
                    {
                        multiSelectionSpinner.Enabled = true;
                        addItem.Enabled = true;
                        addItem.Clickable = true;
                    }
                }

                else
                {
                    multiSelectionSpinner.Enabled = true;
                    addItem.Enabled = true;
                    addItem.Clickable = true;
                }
            }
            else
            {
                multiSelectionSpinner.Enabled = false;
                addItem.Enabled = false;
                addItem.Clickable = false;
            }


            if (elementThis.AddNew)
            {
                addMoreLayout.AddView(multiSelectionSpinner);
                addMoreLayout.AddView(addItem);
                dropDownFrame.AddView(addMoreLayout);
            }
            else
            {
                dropDownFrame.AddView(multiSelectionSpinner);
            }
        }

        private void add_multi_item_click(object sender, EventArgs args, MultiSelectSpinner multispinner)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(contextx);
            builder.SetTitle("Add Item");
            EditText input = new EditText(contextx);
            builder.SetView(input);

            builder.SetPositiveButton("Add", (senderAlert, args1) =>
            {
                KeyValue newValue = new KeyValue();
                newValue.Value = "false";
                newValue.Id = dropDownValues.Count;
                newValue.Name = input.Text;

                dropDownValues.Add(newValue);
                multispinner.setItems(arrayofNames());
                builder.Dispose();
            });
            builder.SetNegativeButton("Cancel", (senderAlert, args1) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }

        private void add_item_click(object sender, EventArgs args)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(contextx);
            builder.SetTitle("Add Item");
            EditText input = new EditText(contextx);
            builder.SetView(input);

            builder.SetPositiveButton("Add", (senderAlert, args1) =>
            {
                KeyValue newValue = new KeyValue();
                newValue.Value = "false";
                newValue.Id = dropDownValues.Count;
                newValue.Name = input.Text;

                dropDownValues.Add(newValue);
                spinnerItemsList.Clear();

                for (int i = 0; i < dropDownValues.Count; i++)
                {
                    spinnerItemsList.Add(dropDownValues[i].Name);
                }

                adapterSpinner = new ArrayAdapter<string>(contextx, Android.Resource.Layout.SimpleSpinnerItem, spinnerItemsList);
                adapterSpinner.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinner.Adapter = adapterSpinner;

                adapterSpinner.NotifyDataSetChanged();

                builder.Dispose();
            });
            builder.SetNegativeButton("Cancel", (senderAlert, args1) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }


        private List<string> arrayofNames()
        {

            int arraysize = dropDownValues.Count;

            List<string> names = new List<string>();

            for (int i = 0; i < arraysize; i++)
            {
                names.Add(dropDownValues[i].Name);
            }

            return names;

        }

        private List<int> arrayofSelection()
        {

            int arraysize = dropDownValues.Count;
            List<int> selections = new List<int>();

            for (int i = 0; i < arraysize; i++)
            {
                if (dropDownValues[i].Value == "true")
                    selections.Add(i);
            }

            return selections;

        }

        public List<KeyValue> DropDownBoxValues()
        {
            if (elementThis.IsMultiSelect)
            {
                List<int> selected = multiSelectionSpinner.getSelectedIndices();

                for (int i = 0; i < dropDownValues.Count; i++)
                {
                    dropDownValues[i].Value = "false";
                }

                for (int i = 0; i < selected.Count; i++)
                {
                    for (int j = 0; j < dropDownValues.Count; j++)
                    {
                        if (j == selected[i])
                        {
                            dropDownValues[j].Value = "true";
                        }
                    }
                }
            }

            else
            {
                if (dropDownValues.Count != 0)
                {
                    if (dropDownValues[0].Name == "Choose One")
                    {
                        dropDownValues.RemoveAt(0);
                    }
                }
            }

            return dropDownValues;
        }

        private void activateElementInfo(ReportElement element)
        {
            LinearLayout descriptionHolder = (LinearLayout)theme.GetChildAt(0);
            ImageButton info = (ImageButton)descriptionHolder.GetChildAt(1);

            if (string.IsNullOrEmpty(element.Info))
            {
                info.Visibility = ViewStates.Invisible;
                //info.Visibility = ViewStates.Gone;
            }
            else
            {
                info.Click += (sender2, e) => showInfo(sender2, e, element.Info);
            }
        }

        private void showInfo(object sender, EventArgs eventArgs, string information)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(contextx);
            builder.SetMessage(information);

            builder.SetPositiveButton("Ok", (senderAlert, args) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }
    }
}
