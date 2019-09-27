using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using WorkFlowManagement.Model;
using Object = Java.Lang.Object;
using String = Java.Lang.String;

namespace WorkFlowManagement.Adapters
{
    public class ReportTemplateListAdapter : BaseAdapter<string>, IFilterable
    {
        private readonly Context Context;
        private string[] nameList;
        private DateTime[] dateList;
        private int searchType;
        private Filter filter;
        private string[] MatchNames;
        private DateTime[] MatchDates;

        public ReportTemplateListAdapter(Context context, List<Template> templateList)
        {
            Context = context;
            filter = new ReportTemplateFilter(this);
            nameList = new string[templateList.Count];
            dateList = new DateTime[templateList.Count];

            for (int i = 0; i < templateList.Count; i++)
            {
                nameList[i] = templateList[i].ReportName;
                dateList[i] = templateList[i].CreatedDateTime;
            }

            MatchNames = new string[templateList.Count];
            MatchDates = new DateTime[templateList.Count];

            MatchNames = nameList;
            MatchDates = dateList;
        }

        public void setSearchType(int seachType)
        {
            this.searchType = seachType;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            ReportTempleteItemViewHolder holder;
            if (view == null)
            {
                holder = new ReportTempleteItemViewHolder();
                view = LayoutInflater.From(Context).Inflate(Resource.Layout.fragment_dialog_report_templete_list_item, parent, false);
                holder.ReportTemplateName = view.FindViewById<TextView>(Resource.Id.txtRemprtTemplateName);
                holder.ReportTemplateDate = view.FindViewById<TextView>(Resource.Id.txtRemprtTemplateDate);
                view.Tag = holder;
            }
            else
            {
                holder = (ReportTempleteItemViewHolder)view.Tag;
            }

            holder.ReportTemplateName.Text = MatchNames[position];
            holder.ReportTemplateDate.Text = "Created Date : " + MatchDates[position].ToShortDateString();
            return view;
        }

        public override Object GetItem(int position)
        {
            return MatchNames[position];
        }

        public override int Count
        {
            get { return MatchNames.Length; }
        }

        public void ResetSearch()
        {
            MatchNames = nameList.ToArray();
            NotifyDataSetChanged();
        }

        public override string this[int position]
        {
            get { return MatchNames[position]; }
        }

        public Filter Filter
        {
            get
            {
                return filter;
            }
        }

        public class ReportTemplateFilter : Filter
        {
            private readonly ReportTemplateListAdapter Adapter;
            private List<string> FilterdList;

            public ReportTemplateFilter(ReportTemplateListAdapter reportTemplateListAdapter)
            {
                Adapter = reportTemplateListAdapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                FilterResults results = new FilterResults();

                if (!System.String.IsNullOrEmpty(constraint.ToString()))
                {
                    FilterdList = Adapter.nameList.Where(a => a.ToLower().Contains(constraint.ToString().ToLower())).ToList();

                    Adapter.MatchNames = FilterdList.ToArray();

                    Object[] matchObjects = new Object[FilterdList.Count];
                    for (int i = 0; i < FilterdList.Count; i++)
                    {
                        matchObjects[i] = new String(FilterdList[i]);
                    }

                    results.Values = matchObjects;
                    results.Count = FilterdList.Count;
                }
                else
                {
                    Adapter.ResetSearch();
                }

                return results;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                Adapter.NotifyDataSetChanged();
            }
        }
    }

    public class ReportTempleteItemViewHolder : Object
    {
        public TextView ReportTemplateName { get; set; }
        public TextView ReportTemplateDate { get; set; }
    }
}