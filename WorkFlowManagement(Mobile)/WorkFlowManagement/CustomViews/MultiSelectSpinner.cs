using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using WorkFlowManagement.Model;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;

namespace WorkFlowManagement.CustomViews
{
    public class MultiSelectSpinner : Spinner, IDialogInterfaceOnMultiChoiceClickListener
    {

        String[] _items = null;
        bool[] mSelection = null;
        bool[] mSelectionAtStart = null;
        String _itemsAtStart = null;
        private Context contexT;
        private List<KeyValue> dropDownValues;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;
        ArrayAdapter<String> simple_adapter;
        private ImageView myView;

        //public MultiSelectSpinner(Context context): base(context)
        //{
        //    simple_adapter = new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleSpinnerItem);
        //    sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
        //    sharedPreferencesEditor = sharedPreferences.Edit();
        //    contexT = context;
        //    Adapter = simple_adapter;
        //}

        public MultiSelectSpinner(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            simple_adapter = new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleSpinnerItem);
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            contexT = context;
            Adapter = simple_adapter;
        }

        public MultiSelectSpinner(Context context, ImageView indicatorImage)
            : base(context)
        {
            simple_adapter = new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleSpinnerItem);
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            sharedPreferencesEditor = sharedPreferences.Edit();
            contexT = context;
            myView = indicatorImage;
            Adapter = simple_adapter;
        }

        public void OnClick(IDialogInterface dialog, int which, bool isChecked)
        {
            if (mSelection != null && which < mSelection.Length)
            {
                mSelection[which] = isChecked;
                simple_adapter.Clear();
                simple_adapter.Add(buildSelectedItemString());
                myView.SetImageResource(Resource.Drawable.checked_forms_create_project_medium);
                sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                sharedPreferencesEditor.Commit();
            }
            else
            {
                throw new IllegalArgumentException("Argument 'which' is out of bounds.");
            }
        }

        public override bool PerformClick()
        {
            if (_items != null)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(contexT);
                builder.SetMultiChoiceItems(_items, mSelection, this);
                _itemsAtStart = getSelectedItemsAsString();

                builder.SetPositiveButton("Submit", (senderAlert, args) =>
                {
                    Array.Copy(mSelection, 0, mSelectionAtStart, 0,
                        mSelection.Length);
                    sharedPreferencesEditor.PutBoolean("ReportEditFlag", true);
                    sharedPreferencesEditor.Commit();
                    builder.Dispose();
                });

                builder.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    simple_adapter.Clear();
                    simple_adapter.Add(_itemsAtStart);
                    Array.Copy(mSelectionAtStart, 0, mSelection, 0,
                        mSelectionAtStart.Length);
                    builder.Dispose();
                });

                builder.Show();

            }

            else
            {
                Toast.MakeText(contexT, "There are no items to show, add you can add an item by pressing the '+' button", ToastLength.Short).Show();
            }

            return true;
        }

        public void setItems(String[] items)
        {

            if (items.Count() != 0)
            {
                _items = items;
                mSelection = new bool[_items.Length];
                mSelectionAtStart = new bool[_items.Length];
                simple_adapter.Clear();
                simple_adapter.Add(_items[0]);
                Arrays.Fill(mSelection, false);
                mSelection[0] = true;
                mSelectionAtStart[0] = true;
            }
        }

        public void setItems(List<String> items)
        {

            if (items.Count != 0)
            {
                _items = items.ToArray();
                mSelection = new bool[_items.Length];
                mSelectionAtStart = new bool[_items.Length];
                simple_adapter.Clear();
                simple_adapter.Add(_items[0]);
                Arrays.Fill(mSelection, false);
                mSelection[0] = true;
            }
        }

        public void setSelection(String[] selection)
        {
            if (_items.Count() != 0)
            {
                foreach (String cell in selection)
                {
                    for (int j = 0; j < _items.Length; ++j)
                    {
                        if (_items[j].Equals(cell))
                        {
                            mSelection[j] = true;
                            mSelectionAtStart[j] = true;
                        }
                    }
                }
            }
        }

        public void setSelection(List<String> selection)
        {

            if (selection.Count != 0)
            {
                for (int i = 0; i < mSelection.Length; i++)
                {
                    mSelection[i] = false;
                    mSelectionAtStart[i] = false;
                }
                foreach (String sel in selection)
                {
                    for (int j = 0; j < _items.Length; ++j)
                    {
                        if (_items[j].Equals(sel))
                        {
                            mSelection[j] = true;
                            mSelectionAtStart[j] = true;
                        }
                    }
                }
                simple_adapter.Clear();
                simple_adapter.Add(buildSelectedItemString());
            }
        }

        public void setSelection(int index)
        {
            if (mSelection != null)
            {
                for (int i = 0; i < mSelection.Length; i++)
                {
                    mSelection[i] = false;
                    mSelectionAtStart[i] = false;
                }
                if (index >= 0 && index < mSelection.Length)
                {
                    mSelection[index] = true;
                    mSelectionAtStart[index] = true;
                }
                else
                {
                    throw new IllegalArgumentException("Index " + index + " is out of bounds.");
                }
                simple_adapter.Clear();
                simple_adapter.Add(buildSelectedItemString());
            }
        }

        public void setSelection(int[] selectedIndices)
        {

            if (mSelection != null)
            {
                for (int i = 0; i < mSelection.Length; i++)
                {
                    mSelection[i] = false;
                    mSelectionAtStart[i] = false;
                }
                foreach (int index in selectedIndices)
                {
                    if (index >= 0 && index < mSelection.Length)
                    {
                        mSelection[index] = true;
                        mSelectionAtStart[index] = true;
                    }
                    else
                    {
                        throw new IllegalArgumentException("Index " + index + " is out of bounds.");
                    }
                }
                simple_adapter.Clear();
                simple_adapter.Add(buildSelectedItemString());
            }

        }

        public List<String> getSelectedStrings()
        {
            List<String> selection = new List<String>();
            for (int i = 0; i < _items.Length; ++i)
            {
                if (mSelection[i])
                {
                    selection.Add(_items[i]);
                }
            }
            return selection;
        }

        public List<int> getSelectedIndices()
        {
            List<int> selection = new List<int>();
            for (int i = 0; i < _items.Length; ++i)
            {
                if (mSelection[i])
                {
                    selection.Add(i);
                }
            }
            return selection;
        }

        private String buildSelectedItemString()
        {
            StringBuilder sb = new StringBuilder();
            bool foundOne = false;

            for (int i = 0; i < _items.Length; ++i)
            {
                if (mSelection[i])
                {
                    if (foundOne)
                    {
                        sb.Append(", ");
                    }
                    foundOne = true;

                    sb.Append(_items[i]);
                }
            }
            return sb.ToString();
        }

        public String getSelectedItemsAsString()
        {
            StringBuilder sb = new StringBuilder();
            bool foundOne = false;

            if (mSelection != null)
            {
                for (int i = 0; i < _items.Length; ++i)
                {
                    if (mSelection[i])
                    {
                        if (foundOne)
                        {
                            sb.Append(", ");
                        }
                        foundOne = true;
                        sb.Append(_items[i]);
                    }
                }

            }
            return sb.ToString();
        }


    }

}