using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.Adapters
{
    public class ProjectNameListAdapter : BaseAdapter<Project>
    {
        private readonly Context context;
        private readonly List<Project> projectNameList;

        public ProjectNameListAdapter(Context context, List<Project> projectNameList)
        {
            this.context = context;
            this.projectNameList = projectNameList;
        }

        public override int Count => projectNameList.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Project this[int position] => projectNameList[position];

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ViewHolder holder;
            if (view == null)
            {
                holder = new ViewHolder();
                view = LayoutInflater.From(context).Inflate(Resource.Layout.item_project_name_list, parent, false);
                holder.ProjectName = view.FindViewById<TextView>(Resource.Id.lblProjectListName);
                view.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)view.Tag;
            }

            var item = this[position];
            holder.ProjectName.Text = item.Title;
            return view;
        }
    }

    public class ViewHolder : Java.Lang.Object
    {
        public TextView ProjectName { get; set; }
        public TextView Location { get; set; }
        public TextView CreatedDate { get; set; }
        public TextView Count { get; set; }
    }

    internal class JavaHolder : Java.Lang.Object
    {
        public readonly object Instance;

        public JavaHolder(object instance)
        {
            this.Instance = instance;
        }
    }

    public static class ObjectExtension
    {
        public static TObject ToNetObject<TObject>(this Java.Lang.Object value)
        {
            if (value == null)
                return default(TObject);

            if (!(value is JavaHolder))
                throw new InvalidOperationException("Unable to convert to .NET object.Only Java.Lang.Object created with .ToJavaObject() can be converted.");

            return
                (TObject)((JavaHolder)value).Instance;
        }

        public static Java.Lang.Object ToJavaObject<TObject>(this TObject value)
        {
            if (value == null)
                return null;

            var holder = new JavaHolder(value);

            return
                (Java.Lang.Object)holder;
        }
    }
}