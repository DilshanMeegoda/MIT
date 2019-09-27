using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Assist;
using WorkFlowManagement.Common;
using WorkFlowManagement.Enum;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.Adapters
{
    public class ReportListArchiveAdapter : BaseAdapter<Report>
    {
        private const int TYPE_ITEM = 0;
        private const int TYPE_SEPARATOR = 1;

        private readonly Context Context;
        private List<Report> MatchedList;
        private List<Report> HeaderList;
        private TreeSet sectionHeaderTreeSet = new TreeSet();
        private ImageLoader imageLoader;
        private DisplayImageOptions defaultOptions;

        public ReportListArchiveAdapter(Context context, List<Report> resultList, TreeSet sectionHeaders)
        {
            Context = context;
            MatchedList = resultList;
            sectionHeaderTreeSet = sectionHeaders;
            InitImageLoader();
        }

        public override int GetItemViewType(int position)
        {
            return sectionHeaderTreeSet.Contains(position) ? TYPE_SEPARATOR : TYPE_ITEM;
        }

        public override int ViewTypeCount => 2;

        public void FilterList(List<Report> filteredList, TreeSet sectionHeaders)
        {
            MatchedList = filteredList;
            sectionHeaderTreeSet = sectionHeaders;
            NotifyDataSetChanged();
        }

        public override Report this[int position] => MatchedList[position];

        public override int Count => MatchedList.Count;

        public override long GetItemId(int position)
        {
            return position;
        }
        public override Object GetItem(int position)
        {
            return MatchedList[position].ToJavaObject();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            ViewHolder holder;
            int rowType = GetItemViewType(position);
            Report item = this[position];

            if (rowType == TYPE_SEPARATOR)
            {
                view = LayoutInflater.From(Context).Inflate(Resource.Layout.item_report_list_section, parent, false);
                view.FindViewById<TextView>(Resource.Id.sectiontextView).Text = item.ReportName;
                return view;
            }

            view = convertView;
            if (view == null)
            {
                holder = new ViewHolder();

                view = LayoutInflater.From(Context).Inflate(Resource.Layout.item_report_template_list, parent, false);
                holder.ReportName = view.FindViewById<TextView>(Resource.Id.reportNametextView);
                holder.ReportDescription = view.FindViewById<TextView>(Resource.Id.reportDescriptiontextView);
                holder.ReportUserImage = view.FindViewById<RoundedImageView>(Resource.Id.imgUser);
                holder.ReportUserCount = view.FindViewById<TextView>(Resource.Id.txtUserCount);
                holder.notSyncedIndicator = view.FindViewById<ImageView>(Resource.Id.imageViewFormNotSynced);
                holder.rejectedIndicator = view.FindViewById<ImageView>(Resource.Id.imageViewFormRejected);

                view.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)view.Tag;
            }

            var layoutParams = new RelativeLayout.LayoutParams(50, 50);
            holder.ReportName.Text = string.IsNullOrEmpty(item.ReportName) ? Context.Resources.GetString(Resource.String.NotAvailable) : item.ReportName + " " + item.Number;
            holder.ReportDescription.Text = string.IsNullOrEmpty(item.Description) ? Context.Resources.GetString(Resource.String.NotAvailable) : item.Description;

            //if (!string.IsNullOrEmpty(item.DisplayUserImageUrl))
            //{
            //    imageLoader.DisplayImage(Configuration.AzureImagePath + "uploads/" + item.DisplayUserImageUrl, holder.ReportUserImage, defaultOptions);
            //}

            if (item.AssignUsers != null)
            {
                switch (item.AssignUsers.Count)
                {
                    case 0:
                        holder.ReportUserCount.Visibility = ViewStates.Gone;
                        holder.ReportUserImage.Visibility = ViewStates.Gone;
                        break;
                    case 1:
                        layoutParams.AddRule(LayoutRules.AlignParentRight);
                        layoutParams.AddRule(LayoutRules.CenterVertical);
                        holder.ReportUserImage.LayoutParameters = layoutParams;
                        holder.ReportUserImage.Visibility = ViewStates.Visible;
                        holder.ReportUserCount.Visibility = ViewStates.Gone;
                        break;
                    default:
                        layoutParams.SetMargins(0, 0, 5, 0);
                        layoutParams.AddRule(LayoutRules.LeftOf, holder.ReportUserCount.Id);
                        layoutParams.AddRule(LayoutRules.CenterVertical);
                        holder.ReportUserImage.LayoutParameters = layoutParams;
                        holder.ReportUserImage.Visibility = ViewStates.Visible;
                        holder.ReportUserCount.Visibility = ViewStates.Visible;
                        holder.ReportUserCount.Text = "+" + (item.AssignUsers.Count - 1);
                        break;
                }
            }

            holder.rejectedIndicator.SetImageResource(item.ReportStatus == ReportStatus.Rejected ? Resource.Drawable.rejected : 0);

            return view;
        }

        public class ViewHolder : Object
        {
            public TextView ReportName { get; set; }
            public TextView ReportDescription { get; set; }
            public ImageView ReportUserImage { get; set; }
            public TextView ReportUserCount { get; set; }
            public ImageView notSyncedIndicator { get; set; }
            public ImageView rejectedIndicator { get; set; }
        }

        private void InitImageLoader()
        {
            defaultOptions = new DisplayImageOptions.Builder()
                .CacheOnDisk(true)
                .CacheInMemory(true)
                .ImageScaleType(ImageScaleType.ExactlyStretched)
                .BitmapConfig(Bitmap.Config.Rgb565)
                .ResetViewBeforeLoading(true)
                .Build();

            ImageLoaderConfiguration.Builder builder =
                new ImageLoaderConfiguration.Builder(Application.Context).DefaultDisplayImageOptions(defaultOptions);

            ImageLoaderConfiguration config = builder.Build();
            imageLoader = ImageLoader.Instance;
            imageLoader.Init(config);

        }
    }
}
